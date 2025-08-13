using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using StarlightVigil;  // ������namespace��

namespace StarlightVigil.Managers
{
    /// <summary>
    /// ս��ϵͳ������ - ����ս�����̡��˺����㡢������
    /// </summary>
    public class CombatManager : Singleton<CombatManager>
    {
        [Header("ս��״̬")]
        public bool isInBattle = false;
        public int currentWave = 0;
        public int enemiesRemaining = 0;

        [Header("����ϵͳ")]
        public int comboCount = 0;
        public float comboTimer = 0f;
        public float comboWindow = 2f;

        [Header("ս������")]
        public float damageMultiplier = 1f;
        public float defenseMultiplier = 1f;

        [Header("���˹���")]
        private List<GameObject> activeEnemies = new List<GameObject>();

        [Header("�¼�")]
        public UnityEvent<int> OnComboChanged;
        public UnityEvent<int> OnEnemyKilled;
        public UnityEvent OnBattleStart;
        public UnityEvent OnBattleEnd;
        public UnityEvent<int> OnWaveComplete;

        protected override void Awake()
        {
            base.Awake();
            GameDebug.Log("CombatManager Initialized");
        }

        void Update()
        {
            // ����������ʱ��
            if (comboCount > 0 && comboTimer > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0)
                {
                    ResetCombo();
                }
            }
        }

        /// <summary>
        /// ��ʼս��
        /// </summary>
        public void StartBattle()
        {
            isInBattle = true;
            currentWave = 0;
            ResetCombo();
            OnBattleStart?.Invoke();
            GameDebug.Log("Battle started");

            // ֪ͨ����ϵͳ
            PlayerManager.Instance.isInCombat = true;
        }

        /// <summary>
        /// ����ս��
        /// </summary>
        public void EndBattle(bool victory)
        {
            isInBattle = false;

            if (victory)
            {
                // ���㽱��
                int reward = CalculateBattleReward();
                EconomyManager.Instance.AddMoney(reward);
            }

            OnBattleEnd?.Invoke();
            GameDebug.Log($"Battle ended - Victory: {victory}");

            // ֪ͨ����ϵͳ
            PlayerManager.Instance.isInCombat = false;
        }

        /// <summary>
        /// ע�����
        /// </summary>
        public void RegisterEnemy(GameObject enemy)
        {
            if (!activeEnemies.Contains(enemy))
            {
                activeEnemies.Add(enemy);
                enemiesRemaining++;
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void OnEnemyDeath(GameObject enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
                enemiesRemaining--;

                // ��������
                AddCombo();
                OnEnemyKilled?.Invoke(enemiesRemaining);

                // ����Ƿ����굱ǰ����
                if (enemiesRemaining == 0 && isInBattle)
                {
                    OnWaveComplete?.Invoke(currentWave);
                }
            }
        }

        /// <summary>
        /// �����˺�
        /// </summary>
        public float CalculateDamage(float baseDamage, DamageType type)
        {
            float finalDamage = baseDamage * damageMultiplier;

            // �����ӳ�
            if (comboCount > 10)
            {
                finalDamage *= 1.5f;
            }
            else if (comboCount > 5)
            {
                finalDamage *= 1.2f;
            }

            return finalDamage;
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void AddCombo()
        {
            comboCount++;
            comboTimer = comboWindow;
            OnComboChanged?.Invoke(comboCount);
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void ResetCombo()
        {
            comboCount = 0;
            comboTimer = 0;
            OnComboChanged?.Invoke(0);
        }

        /// <summary>
        /// ����ս������
        /// </summary>
        private int CalculateBattleReward()
        {
            int baseReward = currentWave * 100;

            // ��������
            if (comboCount > 20)
            {
                baseReward = Mathf.RoundToInt(baseReward * 1.5f);
            }

            return baseReward;
        }
    }

    /// <summary>
    /// �˺�����
    /// </summary>
    public enum DamageType
    {
        Physical,
        Energy,
        Fire,
        Heal
    }
}