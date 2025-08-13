using UnityEngine;
using StarlightVigil;
using StarlightVigil.Managers;  // 添加这一行！

public class ManagerTest : MonoBehaviour
{
    void Start()
    {
        // 测试所有Manager是否正常初始化
        Debug.Log("=== Manager Test Start ===");

        // 测试经济系统
        EconomyManager.Instance.AddMoney(500);
        bool canSpend = EconomyManager.Instance.TrySpendMoney(200);
        Debug.Log($"Economy test: Can spend = {canSpend}");

        // 测试UI系统
        UIManager.Instance.ShowTooltip("Manager系统测试成功！", 3f);

        // 测试音频系统（如果有测试音效的话）
        // AudioManager.Instance.PlaySFX(testClip);

        Debug.Log("=== Manager Test Complete ===");
    }

    void Update()
    {
        // 测试按键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerManager.Instance.UpdateHealth(-10);
            UIManager.Instance.ShowTooltip("扣除10点血量", 1f);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            CombatManager.Instance.OnEnemyDeath(null);
            UIManager.Instance.ShowTooltip("模拟击杀敌人", 1f);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager.Instance.NextDay();
        }
    }
}