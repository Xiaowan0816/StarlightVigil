using UnityEngine;
using UnityEngine.Events;
using System;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    /// <summary>
    /// ����ϵͳ������ - �����Ǯ��ծ�񡢽��׵�
    /// </summary>
    public class EconomyManager : Singleton<EconomyManager>
    {
        [Header("����״̬")]
        public int currentMoney = 1000;
        public int currentDebt = 0;
        public int weeklyRent = 1000;

        [Header("��������")]
        public int maxDebt = 99999;
        public float debtInterestRate = 0.1f;
        public bool canSpendInDebt = false;

        [Header("ÿ����֧��¼")]
        private int todayIncome = 0;
        private int todayExpense = 0;

        [Header("�¼�")]
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
        /// ��ӽ�Ǯ
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
        /// ���Ի��ѽ�Ǯ
        /// </summary>
        public bool TrySpendMoney(int amount)
        {
            if (amount <= 0) return false;

            // ����Ƿ�����Ƿծ����
            if (!canSpendInDebt && currentMoney < amount)
            {
                GameDebug.Log($"Not enough money. Need: {amount}, Have: {currentMoney}");
                return false;
            }

            currentMoney -= amount;
            todayExpense += amount;

            // �����Ǯ��Ϊ������תΪծ��
            if (currentMoney < 0)
            {
                currentDebt += Math.Abs(currentMoney);
                currentMoney = 0;
                OnDebtChanged?.Invoke(currentDebt);

                // ����Ƿ��Ʋ�
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
        /// ��ȡ����Ԥ�ƿ۳����
        /// </summary>
        public int GetWeeklyDeduction()
        {
            int deduction = weeklyRent;

            // ����ծ����Ϣ
            if (currentDebt > 0)
            {
                deduction += Mathf.RoundToInt(currentDebt * debtInterestRate);
            }

            return deduction;
        }

        /// <summary>
        /// ���ս���
        /// </summary>
        public void WeeklySettlement()
        {
            GameDebug.Log("Weekly settlement started");

            // �۳�����
            TrySpendMoney(weeklyRent);

            // ����ծ����Ϣ
            if (currentDebt > 0)
            {
                int interest = Mathf.RoundToInt(currentDebt * debtInterestRate);
                currentDebt += interest;
                OnDebtChanged?.Invoke(currentDebt);

                GameDebug.Log($"Debt interest: {interest}, Total debt: {currentDebt}");
            }

            // ����ÿ�ռ�¼
            todayIncome = 0;
            todayExpense = 0;

            OnWeeklySettlement?.Invoke();
        }

        /// <summary>
        /// ��ȡ����״̬ժҪ
        /// </summary>
        public string GetEconomySummary()
        {
            return $"���: {currentMoney} | ծ��: {currentDebt} | ����Ԥ�ƿ۳�: {GetWeeklyDeduction()}";
        }

        /// <summary>
        /// ����Ƿ���Թ���
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

    // ɾ������� GameOverReason ö�ٶ��壡
    // ���Ѿ��� GameManager.cs �ж�����
}