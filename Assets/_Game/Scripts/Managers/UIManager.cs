using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;  // 添加这个
using System.Collections.Generic;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    /// <summary>
    /// UI管理器 - 管理所有UI界面和HUD
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("HUD元素")]
        public GameObject hudPanel;
        public Image healthBar;
        public Image staminaBar;
        public TextMeshProUGUI moneyText;
        public TextMeshProUGUI comboText;
        public GameObject crosshair;

        [Header("界面面板")]
        public GameObject pauseMenu;
        public GameObject inventoryPanel;
        public GameObject craftingPanel;
        public GameObject shopPanel;

        [Header("提示系统")]
        public GameObject tooltipPrefab;
        public Transform tooltipContainer;
        private Queue<GameObject> tooltipPool = new Queue<GameObject>();

        [Header("对话系统")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI dialogueText;
        public TextMeshProUGUI speakerName;

        protected override void Awake()
        {
            base.Awake();
            InitializeTooltipPool();
            GameDebug.Log("UIManager Initialized");
        }

        void Start()
        {
            // 订阅事件
            PlayerManager.Instance.OnHealthChanged.AddListener(UpdateHealthBar);
            PlayerManager.Instance.OnStaminaChanged.AddListener(UpdateStaminaBar);
            EconomyManager.Instance.OnMoneyChanged.AddListener(UpdateMoneyDisplay);
            CombatManager.Instance.OnComboChanged.AddListener(UpdateComboDisplay);
        }

        /// <summary>
        /// 初始化提示池
        /// </summary>
        void InitializeTooltipPool()
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject tooltip = Instantiate(tooltipPrefab, tooltipContainer);
                tooltip.SetActive(false);
                tooltipPool.Enqueue(tooltip);
            }
        }

        /// <summary>
        /// 更新血量条
        /// </summary>
        public void UpdateHealthBar(float normalizedHealth)
        {
            if (healthBar != null)
            {
                healthBar.DOFillAmount(normalizedHealth, 0.3f);

                // 低血量时变红
                if (normalizedHealth < 0.3f)
                {
                    healthBar.DOColor(Color.red, 0.3f);
                }
                else
                {
                    healthBar.DOColor(Color.white, 0.3f);
                }
            }
        }

        /// <summary>
        /// 更新体力条
        /// </summary>
        public void UpdateStaminaBar(float normalizedStamina)
        {
            if (staminaBar != null)
            {
                staminaBar.DOFillAmount(normalizedStamina, 0.2f);
            }
        }

        /// <summary>
        /// 更新金钱显示
        /// </summary>
        public void UpdateMoneyDisplay(int money)
        {
            if (moneyText != null)
            {
                moneyText.text = $"¥{money:N0}";

                // 金钱变化动画
                moneyText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            }
        }

        /// <summary>
        /// 更新连击显示
        /// </summary>
        public void UpdateComboDisplay(int combo)
        {
            if (comboText != null)
            {
                if (combo > 0)
                {
                    comboText.gameObject.SetActive(true);
                    comboText.text = $"COMBO x{combo}";
                    comboText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f);
                }
                else
                {
                    comboText.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        public void ShowTooltip(string message, float duration = 2f)
        {
            if (tooltipPool.Count > 0)
            {
                GameObject tooltip = tooltipPool.Dequeue();
                TextMeshProUGUI text = tooltip.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = message;
                }

                tooltip.SetActive(true);
                tooltip.transform.localScale = Vector3.zero;
                tooltip.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

                // 自动隐藏
                DOVirtual.DelayedCall(duration, () =>
                {
                    tooltip.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                    {
                        tooltip.SetActive(false);
                        tooltipPool.Enqueue(tooltip);
                    });
                });
            }
        }

        /// <summary>
        /// 显示对话（使用协程替代DOText）
        /// </summary>
        public void ShowDialogue(string speaker, string content)
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            if (speakerName != null)
            {
                speakerName.text = speaker;
            }

            if (dialogueText != null)
            {
                // 方法1：直接显示（临时方案）
                dialogueText.text = content;

                // 方法2：使用协程实现打字机效果
                // StartCoroutine(TypewriterEffect(content));
            }
        }

        /// <summary>
        /// 打字机效果协程（可选）
        /// </summary>
        IEnumerator TypewriterEffect(string content)
        {
            dialogueText.text = "";
            foreach (char letter in content.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
        }

        /// <summary>
        /// 隐藏对话
        /// </summary>
        public void HideDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        /// <summary>
        /// 切换暂停菜单
        /// </summary>
        public void TogglePauseMenu()
        {
            if (pauseMenu != null)
            {
                bool isActive = !pauseMenu.activeSelf;
                pauseMenu.SetActive(isActive);

                Time.timeScale = isActive ? 0 : 1;
                Cursor.visible = isActive;
                Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        /// <summary>
        /// 设置准星可见性
        /// </summary>
        public void SetCrosshairVisible(bool visible)
        {
            if (crosshair != null)
            {
                crosshair.SetActive(visible);
            }
        }

        /// <summary>
        /// 显示伤害数字
        /// </summary>
        public void ShowDamageNumber(Vector3 worldPosition, float damage, Color color)
        {
            // TODO: Week 4实现
            GameDebug.Log($"Show damage: {damage} at {worldPosition}");
        }
    }
}