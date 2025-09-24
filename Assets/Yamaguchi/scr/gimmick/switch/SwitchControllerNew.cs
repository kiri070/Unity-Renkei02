using UnityEngine;

public class SwitchControllerNew : MonoBehaviour
{
    [Tooltip("このスイッチで制御するエネミーたち（EnemyToggle を持っている必要があります）")]
    public EnemyToggle[] controlledEnemies; // このスイッチで制御対象となる敵のリスト

    // プレイヤーがスイッチに触れたときに呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("BringObj"))
        {
            // 制御対象のすべての敵に対して表示/非表示をトグル
            foreach (EnemyToggle enemy in controlledEnemies)
            {
                if (enemy != null)
                {
                    enemy.Toggle();
                }
            }
        }
    }
}

