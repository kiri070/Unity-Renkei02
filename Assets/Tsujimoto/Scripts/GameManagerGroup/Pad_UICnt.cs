using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Pad_UICnt : MonoBehaviour
{
    [Header("�ŏ��Ƀt�H�[�J�X�����{�^��")]
    [Tooltip("�Ȃ��ꍇ�͖����ɐݒ肵�Ȃ��Ă���")]
    [SerializeField] GameObject firstButton;

    [Header("�ݒ��ʂōŏ��Ƀt�H�[�J�X�����{�^��")]
    [SerializeField] GameObject settingFirstButton;

    [Header("���C����UI���i�[")]
    [Tooltip("��CanvasGroup���K�v")]
    [SerializeField] CanvasGroup MainCanvasGroup;

    GameObject previousSelected; //�ύX�����I�u�W�F�N�g���ꎞ�i�[����
    Vector3 buttonScale;         //�{�^���̌��̃T�C�Y

    InputCnt pad_UICnt; //�A�N�V�����}�b�v

    bool isControllerInputActive = false; //�R���g���[���[�����삳�ꂽ���ǂ���
    SettingManager settingManager;
    private void Start()
    {
        //�ŏ��Ƀt�H�[�J�X�����{�^����ݒ�
        // null����Ȃ��Ƃ������t�H�[�J�X�𓖂Ă�
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton);

        settingManager = FindObjectOfType<SettingManager>(); // �Q�Ǝ擾

        pad_UICnt = new InputCnt();
        pad_UICnt.UICnt.Enable();

        //�ݒ��ʂ��J���{�^����o�^
        pad_UICnt.UICnt.OpenSetting.performed += ctx =>
        {
            settingManager.Pad_OnOffSettingUI();
        };
    }

    void Update()
    {

        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            // �X�e�B�b�N��{�^���������ꂽ��
            bool isInput =
                gamepad.leftStick.ReadValue().magnitude > 0.1f ||   //���X�e�B�b�N
                gamepad.rightStick.ReadValue().magnitude > 0.1f ||  //�E�X�e�B�b�N 
                gamepad.buttonSouth.wasPressedThisFrame ||          //A�{�^��
                gamepad.buttonNorth.wasPressedThisFrame ||          //Y�{�^��
                gamepad.buttonEast.wasPressedThisFrame ||           //B�{�^��
                gamepad.buttonWest.wasPressedThisFrame ||           //X�{�^��
                gamepad.leftShoulder.wasPressedThisFrame ||         //LB
                gamepad.rightShoulder.wasPressedThisFrame ||        //RB
                gamepad.dpad.ReadValue().magnitude > 0.1f;          //D�p�b�h(�\���L�[)

            //���͂��ꂽ��
            if (isInput)
            {
                isControllerInputActive = true;
            }
        }

        //�t�H�[�J�X����Ă�{�^�����Ȃ����,�t�H�[�J�X����{�^����ݒ�
        if (isControllerInputActive && EventSystem.current.currentSelectedGameObject == null)
        {
            if (firstButton != null)
                EventSystem.current.SetSelectedGameObject(firstButton);
            isControllerInputActive = false;
        }

        //�t�H�[�J�X����Ă���{�^���̃T�C�Y��ύX
        Change_FocusButtonScale();
    }

    //�t�H�[�J�X����Ă���{�^���̃T�C�Y��ύX
    void Change_FocusButtonScale()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject; //�I�𒆂̃I�u�W�F�N�g���i�[
        if (selectedObj == previousSelected)
            return; // �I��ς���ĂȂ���Ή������Ȃ�

        // �O�̑I���̃T�C�Y��߂�
        if (previousSelected != null)
        {
            var rtPrev = previousSelected.GetComponent<RectTransform>();
            if (rtPrev != null)
                rtPrev.localScale = buttonScale; // ���̃T�C�Y�ɖ߂�
        }

        if (selectedObj != null)
        {
            //�{�^����RectTransform���擾
            RectTransform rt = selectedObj.GetComponent<RectTransform>();
            if (rt != null)
            {
                //���̃T�C�Y��ۑ�
                buttonScale = rt.localScale;
                //�T�C�Y��ύX
                rt.localScale += new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        previousSelected = selectedObj; //�ύX�����I�u�W�F�N�g���ꎞ�i�[
    }

    //�ݒ���J�����ɌĂԊ֐�
    public void OpenSetting()
    {
        // ���C����ʂ̑���𖳌���
        MainCanvasGroup.interactable = false;
        MainCanvasGroup.blocksRaycasts = false;

        EventSystem.current.SetSelectedGameObject(null); // ��x�t�H�[�J�X���N���A
        EventSystem.current.SetSelectedGameObject(settingFirstButton); // �ݒ��ʂ̃{�^����I��
    }

    //�ݒ����鎞�ɌĂԊ֐�
    public void CloseSetting()
    {
        // ���C����ʂ̑����L���ɖ߂�
        MainCanvasGroup.interactable = true;
        MainCanvasGroup.blocksRaycasts = true;

        EventSystem.current.SetSelectedGameObject(null); // ��x�t�H�[�J�X���N���A
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton); // ���C��UI�̃{�^����I��
    }
}
