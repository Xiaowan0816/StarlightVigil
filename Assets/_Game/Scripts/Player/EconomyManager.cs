using UnityEngine;
using UnityEngine.Events;
using System;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    /// <summary>
    /// 经济系统管理器 - 管理金钱、债务、交易等
    /// </summary>
    public class EconomyManager : Singleton<EconomyManager>
    {
        [Header("经济状态")]
        public int currentMoney = 1000;
        public int currentDebt = 0;
        public int weeklyRent = 1000;

        [Header("经济配置")]
        public int maxDebt = 99999;
        public float debtInterestRate = 0.1f;
        public bool canSpendInDebt = false;

        [Header("每日收支记录")]
        private int todayIncome = 0;
        private int todayExpense = 0;

        [Header("事件")]
        public UnityEvent<int> OnMoneyChanged;
        public UnityEvent<int> OnDebtChanged;
        public UnityEvent OnBankrupt;
        public UnityEvent OnWeeklySettlement;

        protected override void Awake()
        {
            base.Awake();
            GameDebug.Log("EconomyManager Initialized");
        }

        /// <summary>
        /// 添加金钱
        /// </summary>
        public void AddMoney(int amount)
        {
            if (amount <= 0) return;

            currentMoney += amount;
            todayIncome += amount;

            OnMoneyChanged?.Invoke(currentMoney);
            GameDebug.Log($"Added money: {amount}, Total: {currentMoney}");
        }

        /// <summary>
        /// 尝试花费金钱
        /// </summary>
        public bool TrySpendMoney(int amount)
        {
            if (amount <= 0) return false;

            // 检查是否允许欠债消费
            if (!canSpendInDebt && currentMoney < amount)
            {
                GameDebug.Log($"Not enough money. Need: {amount}, Have: {currentMoney}");
                return false;
            }

            currentMoney -= amount;
            todayExpense += amount;

            // 如果金钱变为负数，转为债务
            if (currentMoney < 0)
            {
                currentDebt += Math.Abs(currentMoney);
                currentMoney = 0;
                OnDebtChanged?.Invoke(currentDebt);

                // 检查是否破产
                if (currentDebt >= maxDebt)
                {
                    OnBankrupt?.Invoke();
                    GameManager.Instance.GameOver(GameOverReason.Bankrupt);
                }
            }

            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }

        /// <summary>
        /// 获取本周预计扣除金额
        /// </summary>
        public int GetWeeklyDeduction()
        {
            int deduction = weeklyRent;

            // 加上债务利息
            if (currentDebt > 0)
            {
                deduction += Mathf.RoundToInt(currentDebt * debtInterestRate);
            }

            return deduction;
        }

        /// <summary>
        /// 周日结算
        /// </summary>
        public void WeeklySettlement()
        {
            GameDebug.Log("Weekly settlement started");

            // 扣除房租
            TrySpendMoney(weeklyRent);

            // 计算债务利息
            if (currentDebt > 0)
            {
                int interest = Mathf.RoundToInt(currentDebt * debtInterestRate);
                currentDebt += interest;
                OnDebtChanged?.Invoke(currentDebt);

                GameDebug.Log($"Debt interest: {interest}, Total debt: {currentDebt}");
            }

            // 重置每日记录
            todayIncome = 0;
            todayExpense = 0;

            OnWeeklySettlement?.Invoke();
        }

        /// <summary>
        /// 获取经济状态摘要
        /// </summary>
        public string GetEconomySummary()
        {
            return $"存款: {currentMoney} | 债务: {currentDebt} | 本周预计扣除: {GetWeeklyDeduction()}";
        }

        /// <summary>
        /// 检查是否可以购买
        /// </summary>
        public bool CanAfford(int price)
        {
            if (canSpendInDebt)
            {
                return currentDebt < maxDebt;
            }
            else
            {
                return currentMoney >= price;
            }
        }
    }

    // 删除这里的 GameOverReason 枚举定义！
    // 它已经在 GameManager.cs 中定义了
}