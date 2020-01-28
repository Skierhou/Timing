using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateItem : MonoBehaviour
{
    public Color m_SelectColor;
    public Color m_DisableColor;
    public Color m_EnableColor;
    private Color m_UnSelectColor = new Color(0,0,0,0);

    private Button m_Btn;
    private Image m_Img;
    private Text m_Txt;

    private bool bHide;
    private bool bSelect;

    public DateTime dateTime;
    private int curMonth;
    private SelectTimePanel timePanel;

    private void Awake()
    {
        m_Btn = GetComponent<Button>();
        m_Img = transform.Find("Image").GetComponent<Image>();
        m_Txt = GetComponentInChildren<Text>();

        m_Btn.onClick.AddListener(BtnClick);
    }

    public void Initialize(SelectTimePanel inPanel,DateTime inData,int inMonth)
    {
        dateTime = inData;
        timePanel = inPanel;
        //curMonth = inMonth;
        Invoke("UpdateUI", 0.01f);
    }
    private void UpdateUI()
    {
        if (dateTime != null)
        {
            m_Txt.text = dateTime.Day.ToString();
            m_Txt.color = m_EnableColor;
            //if (dateTime.Month == curMonth)
            //    m_Txt.color = m_EnableColor;
            //else
            //    m_Txt.color = m_DisableColor;
        }
    }

    public void UnSelect()
    {
        m_Img.color = m_UnSelectColor;
    }

    private void BtnClick()
    {
        if (!bHide)
        {
            m_Img.color = m_SelectColor;
            timePanel.SelectDay(this, dateTime);
        }
    }

    public void SetHide(bool inEnable)
    {
        bHide = inEnable;
        if (inEnable)
        {
            m_Txt.text = "";
        }
    }
}
