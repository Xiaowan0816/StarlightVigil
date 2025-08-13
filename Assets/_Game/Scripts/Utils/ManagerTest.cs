using UnityEngine;
using StarlightVigil;
using StarlightVigil.Managers;  // �����һ�У�

public class ManagerTest : MonoBehaviour
{
    void Start()
    {
        // ��������Manager�Ƿ�������ʼ��
        Debug.Log("=== Manager Test Start ===");

        // ���Ծ���ϵͳ
        EconomyManager.Instance.AddMoney(500);
        bool canSpend = EconomyManager.Instance.TrySpendMoney(200);
        Debug.Log($"Economy test: Can spend = {canSpend}");

        // ����UIϵͳ
        UIManager.Instance.ShowTooltip("Managerϵͳ���Գɹ���", 3f);

        // ������Ƶϵͳ������в�����Ч�Ļ���
        // AudioManager.Instance.PlaySFX(testClip);

        Debug.Log("=== Manager Test Complete ===");
    }

    void Update()
    {
        // ���԰���
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerManager.Instance.UpdateHealth(-10);
            UIManager.Instance.ShowTooltip("�۳�10��Ѫ��", 1f);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            CombatManager.Instance.OnEnemyDeath(null);
            UIManager.Instance.ShowTooltip("ģ���ɱ����", 1f);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager.Instance.NextDay();
        }
    }
}