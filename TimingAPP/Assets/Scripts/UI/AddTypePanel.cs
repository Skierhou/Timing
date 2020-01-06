using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddTypePanel : BasePanel
{
    private InputField m_InputField;
    private Button m_SureBtn;
    private Button m_CancelBtn;

    private EOperateType operateType;

    private void Awake()
    {
        m_InputField = GetComponentInChildren<InputField>();
        m_SureBtn = transform.Find("Bg/SureBtn").GetComponent<Button>();
        m_CancelBtn = transform.Find("Bg/CancelBtn").GetComponent<Button>();

        m_SureBtn.onClick.AddListener(SureBtnClick);
        m_CancelBtn.onClick.AddListener(CancelBtnClick);
    }

    public override void OnPush(object inPara)
    {
        if (inPara != null)
        {
            operateType = (EOperateType)inPara;
        }
        gameObject.SetActive(true);
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    private void SureBtnClick()
    {
        if (string.IsNullOrEmpty(m_InputField.text)) return;

        switch (operateType)
        {
            case EOperateType.Daily:
                DailyNoteManager.Instance.AddType(m_InputField.text);
                break;
            case EOperateType.Wealth:
                WealthManager.Instance.AddType(m_InputField.text);
                break;
            case EOperateType.Plan:
                break;
        }
        UIManager.Instance.PopPanel();
    }
    private void CancelBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
}
