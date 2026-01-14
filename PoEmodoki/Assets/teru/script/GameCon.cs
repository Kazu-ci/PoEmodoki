using Fungus;
using UnityEngine;

public class GameCon : MonoBehaviour
{
    // 外部から唯一アクセスするための静的プロパティ
    public static GameCon Instance { get; private set; }

    public enum GameState { Talk, Combat, End }
    public GameState currentState = GameState.Talk;

    [Header("コンポーネント参照")]
    [SerializeField] public PlayerAnchor player;
    [SerializeField] public Flowchart Flowchart;
    [SerializeField] private bool isTutorial;
    private int progressStep = 0; // 進行度
    StateMachine<GameCon> stateMachine;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        stateMachine.OnUpdate();
        if (Input.GetKeyDown(KeyCode.H)) { TriggerNextConversation();  }
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
            blockName = GetMainGameBlock(progressStep);
        }

        if (!string.IsNullOrEmpty(blockName))
        {
            // 状態をTalkに変えてからFungus起動
            Flowchart.SendFungusMessage(blockName);
            stateMachine.ChangeState((int)(GameState.Talk));
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
        1 => "Main_Op",
        2 => "Main_BossDead",
        _ => ""
    };

}
