using Fungus;
using UnityEngine;

public class GameCon : MonoBehaviour
{
    StateMachine<GameCon> stateMachine;
    // 外部から唯一アクセスするための静的プロパティ
    public static GameCon Instance { get; private set; }

    // ゲームの現在の状態 (外部からも読み取り可能にする)
    public enum State { Talk, Combat, End }
    public State currentState = State.Combat;

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
        // シーン遷移しても破棄されないようにする場合 (オプション)
        // DontDestroyOnLoad(gameObject); 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine.Add<TalkState>((int)State.Talk);
        stateMachine.Add<CombatState>((int)State.Combat);
        stateMachine.Add<EndState>((int)State.End);
        stateMachine.Onstart((int)State.Combat);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.OnUpdate();
    }

}
