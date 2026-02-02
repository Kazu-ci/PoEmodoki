using Fungus;
using UnityEngine;

public class GameCon : MonoBehaviour
{
    // 外部から唯一アクセスするための静的プロパティ
    public static GameCon Instance { get; private set; }

    public enum GameState { Talk, Combat, End }
    public GameState currentState = GameState.Talk;
    private TextObject currentObject;
    [Header("コンポーネント参照")]
    [SerializeField] public PlayerAnchor player;
    [SerializeField] public PlayerStatus pStatus;
    [SerializeField]public EnemyStatus eStatus;
    [SerializeField] public Flowchart Flowchart;
    [SerializeField] private bool isTutorial;
    private int progressStep = 0; // 進行度
    StateMachine<GameCon> stateMachine;
    [SerializeField]bool rei=true;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        stateMachine = new StateMachine<GameCon>(this);
        // DontDestroyOnLoad(gameObject); 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressStep = 0;
        stateMachine.Add<TalkState>((int)GameState.Talk);
        stateMachine.Add<CombatState>((int)GameState.Combat);
        stateMachine.Add<EndState>((int)GameState.End);
        stateMachine.Onstart((int)GameState.Talk);
        TriggerNextConversation();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.OnUpdate();
        if (Input.GetKeyDown(KeyCode.LeftAlt)) { TriggerNextConversation();  }
    }

    public void ChangeTalk()
    {
        stateMachine.ChangeState((int)GameState.Talk);
    }
    public void ChangeCombat()
    {
        stateMachine.ChangeState((int)GameState.Combat);
    }
    public void TriggerNextConversation()
    {
        progressStep++;
        string blockName = "";

        // シーン設定に応じて読み込むリストを変える
        if (isTutorial)
        {
            blockName = GetTutorialBlock(progressStep);
        }
        else
        {
            if (rei) { blockName = GetMainGameBlock_true(progressStep); }
            else { blockName = GetMainGameBlock(progressStep); }
        }

        if (!string.IsNullOrEmpty(blockName))
        {
            // 状態をTalkに変えてからFungus起動
            Flowchart.SendFungusMessage(blockName);
            stateMachine.ChangeState((int)(GameState.Talk));
        }
    }
    public void RegisterInteractable(TextObject obj)
    {
        currentObject = obj;
    }

    // オブジェクトから呼ばれる解除メソッド
    public void UnregisterInteractable(TextObject obj)
    {
        if (currentObject == obj) currentObject = null;
    }
    private void TryExecuteInteraction()
    {
        if (currentObject != null && !Flowchart.HasExecutingBlocks())
        {
            string blockName = currentObject.TargetBlockName;

            if (Flowchart.HasBlock(blockName))
            {
                Flowchart.ExecuteBlock(blockName);
                ChangeTalk();
            }
        }
    }

    private string GetTutorialBlock(int step) => step switch
    {
        1 => "Tut_op",
        2 => "Tut_end",
        _ => ""
    };

    private string GetMainGameBlock(int step) => step switch
    {
        1 => "main_op",
        2 => "main_bossi",
        3 =>"main_bossm",
        4=> "main_bossdeth",
        _=>""
    };
    private string GetMainGameBlock_true(int step) => step switch
    {
        1 => "main_op",
        2 => "main_bossi",
        3 => "main_bossmt",
        4 => "main_bossdetht",
        _ => ""
    };

}
