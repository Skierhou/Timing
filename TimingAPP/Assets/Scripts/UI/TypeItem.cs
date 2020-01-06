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

    private TypeData typeData;
    private void Awake()
    {
        m_Txt = transform.Find("Text").GetComponent<Text>();
        m_Btn = GetComponent<Button>();
        m_Img = GetComponent<Image>();

        m_Btn.onClick.AddListener(BtnClick);
    }

    public void Initialize(TypeData inData)
    {
        typeData = inData;
        Invoke("UpdateUI",0.01f);
    }
    private void UpdateUI()
    {
        m_Txt.text = typeData.name;
    }

    private void BtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.DailyPanel, typeData);
    }
}
