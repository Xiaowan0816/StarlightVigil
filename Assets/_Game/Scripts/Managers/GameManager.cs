using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("游戏状态")]
    public GameState currentState = GameState.Menu;

    [Header("核心管理器引用")]
    public PlayerManager playerManager;
    public CombatManager combatManager;
    public EconomyManager economyManager;

    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    void InitializeGame()
    {
        Debug.Log("[StarlightVigil] GameManager Initialized");
        // 后续添加初始化逻辑
    }
}

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}