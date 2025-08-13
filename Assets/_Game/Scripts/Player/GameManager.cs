using UnityEngine;
using StarlightVigil;  // ������namespace��

namespace StarlightVigil.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("��Ϸ״̬")]
        public GameState currentState = GameState.Menu;

        [Header("��Ϸ����")]
        public int currentDay = 1;
        public DayOfWeek currentDayOfWeek = DayOfWeek.Monday;

        [Header("���Ĺ���������")]
        private PlayerManager playerManager;
        private CombatManager combatManager;
        private EconomyManager economyManager;
        private AudioManager audioManager;
        private UIManager uiManager;
        private SaveManager saveManager;
        private CraftingManager craftingManager;
        private SceneFlowManager sceneFlowManager;

        protected override void Awake()
        {
            base.Awake();
            InitializeGame();
        }

        void InitializeGame()
        {
            // ��ȡ����Manager����
            playerManager = PlayerManager.Instance;
            combatManager = CombatManager.Instance;
            economyManager = EconomyManager.Instance;
            audioManager = AudioManager.Instance;
            uiManager = UIManager.Instance;
            saveManager = SaveManager.Instance;
            craftingManager = CraftingManager.Instance;
            sceneFlowManager = SceneFlowManager.Instance;

            GameDebug.Log("[StarlightVigil] GameManager Initialized");
            GameDebug.Log($"Current Day: {currentDay} ({currentDayOfWeek})");
        }

        void Start()
        {
            // ��ʼ��Ϸʱ�ĳ�ʼ��
            ChangeState(GameState.Playing);

            // ���ŵ�һ�����֣����ң�
            audioManager.SwitchMusicLayer(0);
        }

        void Update()
        {
            // ��ͣ��Ϸ
            if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// �ı���Ϸ״̬
        /// </summary>
        public void ChangeState(GameState newState)
        {
            currentState = newState;

            switch (newState)
            {
                case GameState.Menu:
                    Time.timeScale = 1;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;

                case GameState.GameOver:
                    Time.timeScale = 1;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }

            GameDebug.Log($"Game state changed to: {newState}");
        }

        /// <summary>
        /// �л���ͣ״̬
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
                uiManager.TogglePauseMenu();
            }
            else if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
                uiManager.TogglePauseMenu();
            }
        }

        /// <summary>
        /// ������һ��
        /// </summary>
        public void NextDay()
        {
            currentDay++;
            currentDayOfWeek = (DayOfWeek)(((int)currentDayOfWeek + 1) % 7);

            GameDebug.Log($"New day: Day {currentDay} ({currentDayOfWeek})");

            // ���ս���
            if (currentDayOfWeek == DayOfWeek.Sunday)
            {
                economyManager.WeeklySettlement();
                uiManager.ShowTooltip("���ս�����ɣ�", 3f);
            }

            // ������Ϸ
            saveManager.SaveGame();
        }

        /// <summary>
        /// �����������
        /// </summary>
        public void OnPlayerDeath()
        {
            // ������ǰս��
            if (combatManager.isInBattle)
            {
                combatManager.EndBattle(false);
            }

            // ���ͻ�����
            uiManager.ShowTooltip("�㱻�����ˣ����ͻ�����...", 3f);

            // TODO: Week 14 ʵ�ֳ����л�
            GameDebug.Log("Player respawn at bedroom - Coming in Week 14");
        }

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        public void GameOver(GameOverReason reason)
        {
            ChangeState(GameState.GameOver);

            switch (reason)
            {
                case GameOverReason.Bankrupt:
                    uiManager.ShowTooltip("�Ʋ��ˣ���Ϸ����...", 5f);
                    // TODO: ��ʾ����»��Ի�
                    break;

                case GameOverReason.Victory:
                    uiManager.ShowTooltip("��ϲͨ�أ�", 5f);
                    // TODO: Week 24 ʵ�ֽ�ֶ���
                    break;
            }

            GameDebug.Log($"Game Over: {reason}");
        }

        /// <summary>
        /// ��ȡ��ǰ�Ƿ����ս��
        /// </summary>
        public bool CanStartBattle()
        {
            // ���ղ���ս��
            if (currentDayOfWeek == DayOfWeek.Sunday)
            {
                uiManager.ShowTooltip("��������Ϣ�գ�����ս��", 2f);
                return false;
            }

            // ������״̬
            if (!playerManager.isAlive)
            {
                return false;
            }

            // ����Ƿ��Ѿ���ս����
            if (combatManager.isInBattle)
            {
                return false;
            }

            return true;
        }
    }

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public enum GameOverReason
    {
        Bankrupt,
        PlayerDeath,
        Victory
    }
}