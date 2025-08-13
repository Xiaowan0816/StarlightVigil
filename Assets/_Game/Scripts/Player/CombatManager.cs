using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using StarlightVigil;  // 必须在namespace外

namespace StarlightVigil.Managers
{
    /// <summary>
    /// 战斗系统管理器 - 管理战斗流程、伤害计算、连击等
    /// </summary>
    public class CombatManager : Singleton<CombatManager>
    {
        [Header("战斗状态")]
        public bool isInBattle = false;
        public int currentWave = 0;
        public int enemiesRemaining = 0;

        [Header("连击系统")]
        public int comboCount = 0;
        public float comboTimer = 0f;
        public float comboWindow = 2f;

        [Header("战斗配置")]
        public float damageMultiplier = 1f;
        public float defenseMultiplier = 1f;

        [Header("敌人管理")]
        private List<GameObject> activeEnemies = new List<GameObject>();

        [Header("事件")]
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
            // 更新连击计时器
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
        /// 开始战斗
        /// </summary>
        public void StartBattle()
        {
            isInBattle = true;
            currentWave = 0;
            ResetCombo();
            OnBattleStart?.Invoke();
            GameDebug.Log("Battle started");

            // 通知其他系统
            PlayerManager.Instance.isInCombat = true;
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndBattle(bool victory)
        {
            isInBattle = false;

            if (victory)
            {
                // 计算奖励
                int reward = CalculateBattleReward();
                EconomyManager.Instance.AddMoney(reward);
            }

            OnBattleEnd?.Invoke();
            GameDebug.Log($"Battle ended - Victory: {victory}");

            // 通知其他系统
            PlayerManager.Instance.isInCombat = false;
        }

        /// <summary>
        /// 注册敌人
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
        /// 敌人死亡处理
        /// </summary>
        public void OnEnemyDeath(GameObject enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
                enemiesRemaining--;

                // 增加连击
                AddCombo();
                OnEnemyKilled?.Invoke(enemiesRemaining);

                // 检查是否清完当前波次
                if (enemiesRemaining == 0 && isInBattle)
                {
                    OnWaveComplete?.Invoke(currentWave);
                }
            }
        }

        /// <summary>
        /// 计算伤害
        /// </summary>
        public float CalculateDamage(float baseDamage, DamageType type)
        {
            float finalDamage = baseDamage * damageMultiplier;

            // 连击加成
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
        /// 增加连击
        /// </summary>
        private void AddCombo()
        {
            comboCount++;
            comboTimer = comboWindow;
            OnComboChanged?.Invoke(comboCount);
        }

        /// <summary>
        /// 重置连击
        /// </summary>
        private void ResetCombo()
        {
            comboCount = 0;
            comboTimer = 0;
            OnComboChanged?.Invoke(0);
        }

        /// <summary>
        /// 计算战斗奖励
        /// </summary>
        private int CalculateBattleReward()
        {
            int baseReward = currentWave * 100;

            // 连击奖励
            if (comboCount > 20)
            {
                baseReward = Mathf.RoundToInt(baseReward * 1.5f);
            }

            return baseReward;
        }
    }

    /// <summary>
    /// 伤害类型
    /// </summary>
    public enum DamageType
    {
        Physical,
        Energy,
        Fire,
        Heal
    }
}