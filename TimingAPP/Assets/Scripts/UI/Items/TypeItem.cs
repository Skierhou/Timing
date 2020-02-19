using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeItem : MonoBehaviour
{
    private Text m_Txt;
    private Button m_Btn;
    private Image m_Img;
    private Button m_DeleteBtn;

    private TypeData typeData;
    private void Awake()
    {
        m_Txt = transform.Find("Text").GetComponent<Text>();
        m_Btn = GetComponent<Button>();
        m_DeleteBtn = transform.Find("DeleteBtn").GetComponent<Button>();
        m_Img = GetComponent<Image>();

        m_Btn.onClick.AddListener(BtnClick);
        m_DeleteBtn.onClick.AddListener(OnDeleteBtnClick);
    }

    public void Initialize(TypeData inData)
    {
        typeData = inData;
        UpdateUI();
    }
    private void UpdateUI()
    {
        m_Txt.text = typeData.name;
    }

    private void BtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.DailyPanel, typeData);
    }
    private void OnDeleteBtnClick()
    {
        DailyNoteManager.Instance.RemoveType(typeData.typeId);
        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel(EPanelType.TypePanel, EOperateType.Daily);
    }
}
