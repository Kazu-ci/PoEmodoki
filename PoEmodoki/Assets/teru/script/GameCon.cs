using Fungus;
using UnityEngine;

public class GameCon : MonoBehaviour
{
    public static class TutorialBlocks
    {
        public const string First = "FirstBlock";
        public const string Second = "SecondBlock";
        public const string Third = "ThirdBlock";
        public const string Fourth = "FourthBlock";
    }
    StateMachine<GameCon> stateMachine;
    // 外部から唯一アクセスするための静的プロパティ
    public static GameCon Instance { get; private set; }

    public enum GameState { Talk, Combat, End }
    public GameState currentState = GameState.Talk;
    private int stage = 0;

    [Header("コンポーネント参照")]
    [SerializeField] public PlayerController playerController;
    [SerializeField] public Flowchart mainFlowchart;
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
        stage = 0;
        stateMachine.Add<TalkState>((int)GameState.Talk);
        stateMachine.Add<CombatState>((int)GameState.Combat);
        stateMachine.Add<EndState>((int)GameState.End);
        stateMachine.Onstart((int)GameState.Talk);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.OnUpdate();
    }

    public void ChangeTalk()
    {
        stateMachine.ChangeState((int)GameState.Talk);
    }
    public void ChangeCombat()
    {
        stateMachine.ChangeState((int)GameState.Combat);
    }
    public void StartNextTalkBlock()
    {
        if (mainFlowchart == null)
        {
            Debug.LogError("mainFlowchartが設定されていません。Fungusブロックを起動できません。", this);
            return;
        }

        stage++; // チュートリアルステージを進行

        string blockName = "";

        switch (stage)
        {
            case 1:
                blockName = TutorialBlocks.First;
                break;
            case 2:
                blockName = TutorialBlocks.Second;
                break;
            case 3:
                blockName = TutorialBlocks.Third;
                break;
            case 4:
                blockName = TutorialBlocks.Fourth;
                break;
            default:
                Debug.LogWarning("全チュートリアルステージが完了しました。");
                return;
        }

        // 汎用的に Fungus のブロックを実行
        mainFlowchart.ExecuteBlock(blockName);
    }

}
