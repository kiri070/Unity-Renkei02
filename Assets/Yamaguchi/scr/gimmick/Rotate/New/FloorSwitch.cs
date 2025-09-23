//using UnityEngine;

///// <summary>
///// �v���C���[�����ނƘA���I�u�W�F�N�g�𓮂������X�C�b�`
///// </summary>
//public class FloorSwitch : MonoBehaviour
//{
//    [Header("���񂾂Ƃ��ɓ��삳����I�u�W�F�N�g")]
//    public NewCubeRotator[] cubeRotators;        // ��]����I�u�W�F�N�g�Q
//    public NewBlueCubePlatform[] bluePlatforms;  // �ړ�����I�u�W�F�N�g�Q

//    [Header("�X�C�b�`�������ꂽ���̉�")]
//    public AudioSource switchSE;

//    private bool isActivated = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        // �v���C���[�����񂾏ꍇ�̂�
//        if (!isActivated && other.CompareTag("Player1")|| other.CompareTag("Player2"))
//        {
//            isActivated = true;

//            // SE�Đ�
//            if (switchSE != null)
//                switchSE.Play();

//            // CubeRotator �̓���J�n
//            foreach (var rotator in cubeRotators)
//            {
//                if (rotator != null)
//                    rotator.StartRotation();
//            }

//            // BlueCubePlatform �̈ړ��J�n
//            foreach (var platform in bluePlatforms)
//            {
//                if (platform != null)
//                    platform.TriggerMovement();
//            }
//        }
//    }

//    // �X�C�b�`�����Z�b�g����ꍇ�ɌĂ�
//    public void ResetSwitch()
//    {
//        isActivated = false;

//        foreach (var rotator in cubeRotators)
//        {
//            if (rotator != null)
//                rotator.ResetRotation();
//        }

//        foreach (var platform in bluePlatforms)
//        {
//            if (platform != null)
//                platform.ResetPlatform();
//        }
//    }
//}
