using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDailyNotePanel : BasePanel
{
    private InputField m_InputField;
    private Button m_SureBtn;
    private Button m_CancelBtn;
    private Image m_Img;

    private TypeData typeData;

    private void Awake()
    {
        m_InputField = transform.Find("Bg/InputField").GetComponent<InputField>();
        m_Img = transform.Find("Bg").GetComponent<Image>();
        m_SureBtn = transform.Find("Bg/SureBtn").GetComponent<Button>();
        m_CancelBtn = transform.Find("Bg/CancelBtn").GetComponent<Button>();

        m_SureBtn.onClick.AddListener(SureBtnClick);
        m_CancelBtn.onClick.AddListener(CancelBtnClick);
    }

    public override void OnPush(object inPara)
    {
        if (inPara != null)
            typeData = (TypeData)inPara;
        gameObject.SetActive(true);
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    private void SureBtnClick()
    {
        if (string.IsNullOrEmpty(m_InputField.text)) return;

        DailyNoteManager.Instance.AddNote("", m_InputField.text, System.DateTime.Now, typeData.name, typeData.typeId, m_Img.color);
        UIManager.Instance.PopPanel();
    }
    private void CancelBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
}
