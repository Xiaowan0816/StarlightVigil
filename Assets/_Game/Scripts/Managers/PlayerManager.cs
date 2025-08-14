using UnityEngine;
using UnityEngine.Events;
using StarlightVigil;  // 必须在namespace外

namespace StarlightVigil.Managers
{
    /// <summary>
    /// 玩家系统管理器 - 管理玩家状态、属性、存档等
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("玩家引用")]
        public GameObject playerPrefab;
        private GameObject currentPlayer;

        [Header("玩家状态")]
        public bool isAlive = true;
        public bool isInCombat = false;
        public bool hasMecha = false;

        [Header("玩家属性")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float maxStamina = 100f;
        public float currentStamina = 100f;

        [Header("事件")]
        public UnityEvent<float> OnHealthChanged;
        public UnityEvent<float> OnStaminaChanged;
        public UnityEvent OnPlayerDeath;
        public UnityEvent OnPlayerSpawn;

        protected override void Awake()
        {
            base.Awake();
            GameDebug.Log("PlayerManager Initialized");
        }

        /// <summary>
        /// 生成玩家
        /// </summary>
        public void SpawnPlayer(Vector3 position, Quaternion rotation)
        {
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
            }

            currentPlayer = Instantiate(playerPrefab, position, rotation);
            currentPlayer.name = "Player";

            // 重置状态
            isAlive = true;
            currentHealth = maxHealth;
            currentStamina = maxStamina;

            OnPlayerSpawn?.Invoke();
            GameDebug.Log("Player spawned");
        }

        /// <summary>
        /// 获取当前玩家GameObject
        /// </summary>
        public GameObject GetPlayer()
        {
            return currentPlayer;
        }

        /// <summary>
        /// 更新玩家血量
        /// </summary>
        public void UpdateHealth(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth / maxHealth);

            if (currentHealth <= 0 && isAlive)
            {
                PlayerDeath();
            }
        }

        /// <summary>
        /// 更新体力值
        /// </summary>
        public void UpdateStamina(float amount)
        {
            currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina / maxStamina);
        }

        /// <summary>
        /// 玩家死亡处理
        /// </summary>
        private void PlayerDeath()
        {
            isAlive = false;
            OnPlayerDeath?.Invoke();
            GameDebug.Log("Player died");

            // 通知GameManager
            GameManager.Instance.OnPlayerDeath();
        }

        /// <summary>
        /// 获取玩家是否可以消耗体力
        /// </summary>
        public bool CanUseStamina(float cost)
        {
            return currentStamina >= cost;
        }
    }
}