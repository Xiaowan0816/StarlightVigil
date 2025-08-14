using UnityEngine;
using StarlightVigil;  // 必须在namespace外

namespace StarlightVigil.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("游戏状态")]
        public GameState currentState = GameState.Menu;

        [Header("游戏配置")]
        public int currentDay = 1;
        public DayOfWeek currentDayOfWeek = DayOfWeek.Monday;

        [Header("核心管理器引用")]
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
            // 获取所有Manager引用
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
            // 开始游戏时的初始化
            ChangeState(GameState.Playing);

            // 播放第一层音乐（卧室）
            audioManager.SwitchMusicLayer(0);
        }

        void Update()
        {
            // 暂停游戏
            if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// 改变游戏状态
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
        /// 切换暂停状态
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
        /// 进入下一天
        /// </summary>
        public void NextDay()
        {
            currentDay++;
            currentDayOfWeek = (DayOfWeek)(((int)currentDayOfWeek + 1) % 7);

            GameDebug.Log($"New day: Day {currentDay} ({currentDayOfWeek})");

            // 周日结算
            if (currentDayOfWeek == DayOfWeek.Sunday)
            {
                economyManager.WeeklySettlement();
                uiManager.ShowTooltip("周日结算完成！", 3f);
            }

            // 保存游戏
            saveManager.SaveGame();
        }

        /// <summary>
        /// 玩家死亡处理
        /// </summary>
        public void OnPlayerDeath()
        {
            // 结束当前战斗
            if (combatManager.isInBattle)
            {
                combatManager.EndBattle(false);
            }

            // 传送回卧室
            uiManager.ShowTooltip("你被击败了！传送回卧室...", 3f);

            // TODO: Week 14 实现场景切换
            GameDebug.Log("Player respawn at bedroom - Coming in Week 14");
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameOver(GameOverReason reason)
        {
            ChangeState(GameState.GameOver);

            switch (reason)
            {
                case GameOverReason.Bankrupt:
                    uiManager.ShowTooltip("破产了！游戏结束...", 5f);
                    // TODO: 显示警察柯基对话
                    break;

                case GameOverReason.Victory:
                    uiManager.ShowTooltip("恭喜通关！", 5f);
                    // TODO: Week 24 实现结局动画
                    break;
            }

            GameDebug.Log($"Game Over: {reason}");
        }

        /// <summary>
        /// 获取当前是否可以战斗
        /// </summary>
        public bool CanStartBattle()
        {
            // 周日不能战斗
            if (currentDayOfWeek == DayOfWeek.Sunday)
            {
                uiManager.ShowTooltip("周日是休息日，不能战斗", 2f);
                return false;
            }

            // 检查玩家状态
            if (!playerManager.isAlive)
            {
                return false;
            }

            // 检查是否已经在战斗中
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