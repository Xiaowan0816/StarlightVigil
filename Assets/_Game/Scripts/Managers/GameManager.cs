using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("��Ϸ״̬")]
    public GameState currentState = GameState.Menu;

    [Header("���Ĺ���������")]
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
        // ������ӳ�ʼ���߼�
    }
}

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}