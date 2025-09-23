using UnityEngine;

public class NewBlueCubePlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // �v���C���[1 �܂��� �v���C���[2 �̏ꍇ
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            // �v���C���[��L���[�u�̎q�ɂ���
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // �v���C���[�����ꂽ��e�q�֌W����
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            collision.transform.SetParent(null);
        }
    }
}
