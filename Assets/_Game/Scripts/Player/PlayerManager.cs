using UnityEngine;
using UnityEngine.Events;
using StarlightVigil;  // ������namespace��

namespace StarlightVigil.Managers
{
    /// <summary>
    /// ���ϵͳ������ - �������״̬�����ԡ��浵��
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("�������")]
        public GameObject playerPrefab;
        private GameObject currentPlayer;

        [Header("���״̬")]
        public bool isAlive = true;
        public bool isInCombat = false;
        public bool hasMecha = false;

        [Header("�������")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float maxStamina = 100f;
        public float currentStamina = 100f;

        [Header("�¼�")]
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
        /// �������
        /// </summary>
        public void SpawnPlayer(Vector3 position, Quaternion rotation)
        {
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
            }

            currentPlayer = Instantiate(playerPrefab, position, rotation);
            currentPlayer.name = "Player";

            // ����״̬
            isAlive = true;
            currentHealth = maxHealth;
            currentStamina = maxStamina;

            OnPlayerSpawn?.Invoke();
            GameDebug.Log("Player spawned");
        }

        /// <summary>
        /// ��ȡ��ǰ���GameObject
        /// </summary>
        public GameObject GetPlayer()
        {
            return currentPlayer;
        }

        /// <summary>
        /// �������Ѫ��
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
        /// ��������ֵ
        /// </summary>
        public void UpdateStamina(float amount)
        {
            currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina / maxStamina);
        }

        /// <summary>
        /// �����������
        /// </summary>
        private void PlayerDeath()
        {
            isAlive = false;
            OnPlayerDeath?.Invoke();
            GameDebug.Log("Player died");

            // ֪ͨGameManager
            GameManager.Instance.OnPlayerDeath();
        }

        /// <summary>
        /// ��ȡ����Ƿ������������
        /// </summary>
        public bool CanUseStamina(float cost)
        {
            return currentStamina >= cost;
        }
    }
}