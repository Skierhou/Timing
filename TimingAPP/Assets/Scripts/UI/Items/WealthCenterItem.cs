using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthCenterItem : MonoBehaviour
{
    public Color IncomeColor;
    public Color OutcomeColor;

    private Text m_TimeTxt;
    private Text m_TypeTxt;
    private Text m_DesTxt;
    private Text m_IncomeTxt;
    private Button m_Btn;

    private WealthNote wealthNote;
    public bool isInCome = true;

    private void Awake()
    {
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_TypeTxt = transform.Find("TypeTxt").GetComponent<Text>();
        m_DesTxt = transform.Find("DesTxt").GetComponent<Text>();
        m_IncomeTxt = transform.Find("IncomeTxt").GetComponent<Text>();
        m_Btn = GetComponent<Button>();

        m_Btn.onClick.AddListener(OnBtnClick);
    }

    public void UpdateUI(WealthNote inNote)
    {
        wealthNote = inNote;
        if (wealthNote != null)
        {
            m_TimeTxt.text = "时间："+wealthNote.DateStr;
            m_TypeTxt.text = "类型："+wealthNote.PayTypeName;
            m_TypeTxt.color = wealthNote.Color;
            m_DesTxt.text = "描述："+wealthNote.Content;
            CheckMoney();
        }
    }

    private void CheckMoney()
    {
        if (wealthNote.Money >= 0)
        {
            m_IncomeTxt.text = "收益：+" + wealthNote.Money;
            m_IncomeTxt.color = IncomeColor;
        }
        else
        {
            m_IncomeTxt.text = "支出：" + wealthNote.Money;
            m_IncomeTxt.color = OutcomeColor;
        }
    }

    private void OnBtnClick()
    {
        
    }
}
