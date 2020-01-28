using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthCenterPanel : BasePanel
{
    private Button m_TypeBtn;
    private Button m_AccountBtn;

    //资产管理
    private Text m_AllMoneyTxt;
    private Text m_YearIncomeTxt;
    private Text m_YearOutcomeTxt;
    private Text m_MonthIncomeTxt;
    private Text m_MonthOutcomeTxt;
    private Text m_DayIncomeTxt;
    private Text m_DayOutcomeTxt;

    //最近提示
    private List<WealthCenterItem> currentIncomeItems = new List<WealthCenterItem>();
    private List<WealthCenterItem> currentOutComeItems = new List<WealthCenterItem>();

    private void Awake()
    {
        m_TypeBtn = transform.Find("TypeBtn").GetComponent<Button>();
        m_AccountBtn = transform.Find("AccountBtn").GetComponent<Button>();

        Transform grid = transform.Find("MyAccountBg/Grid");
        m_AllMoneyTxt = transform.Find("MyAccountBg/AllMoneyTxt").GetComponent<Text>();
        m_YearIncomeTxt = grid.Find("YearIncome").GetComponent<Text>();
        m_YearOutcomeTxt = grid.Find("YearOutcome").GetComponent<Text>();
        m_MonthIncomeTxt = grid.Find("MonthIncome").GetComponent<Text>();
        m_MonthOutcomeTxt = grid.Find("MonthOutcome").GetComponent<Text>();
        m_DayIncomeTxt = grid.Find("DayIncome").GetComponent<Text>();
        m_DayOutcomeTxt = grid.Find("DayOutcome").GetComponent<Text>();

        WealthCenterItem[] wealthCenterItems = transform.GetComponentsInChildren<WealthCenterItem>();
        for (int i = 0; i < wealthCenterItems.Length; i++)
        {
            if (wealthCenterItems[i].isInCome)
                currentIncomeItems.Add(wealthCenterItems[i]);
            else
                currentOutComeItems.Add(wealthCenterItems[i]);
        }

        m_TypeBtn.onClick.AddListener(OnTypeBtnClick);
        m_AccountBtn.onClick.AddListener(OnAccountBtnClick);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        UpdateUI();
    }

    private void UpdateUI()
    {
        List<WealthNote> wealthList = WealthManager.Instance.GetWealthNotesByType();
        float allMoney = 0;
        float yearIncome = 0;
        float monthIncome = 0;
        float dayIncome = 0;
        float yearOutcome = 0;
        float monthOutcome = 0;
        float dayOutcome = 0;

        if (wealthList != null && wealthList.Count > 0)
        {
            for (int i = 0; i < wealthList.Count; i++)
            {
                allMoney += wealthList[i].Money;
                if (wealthList[i].Date.Year == DateTime.Now.Year)
                {
                    yearIncome += (wealthList[i].Money >= 0 ? wealthList[i].Money : 0);
                    yearOutcome += (wealthList[i].Money >= 0 ? 0 : wealthList[i].Money);
                }
                if (wealthList[i].Date.Month == DateTime.Now.Month)
                {
                    monthIncome += (wealthList[i].Money >= 0 ? wealthList[i].Money : 0);
                    monthOutcome += (wealthList[i].Money >= 0 ? 0 : wealthList[i].Money);
                }
                if (wealthList[i].Date.Day == DateTime.Now.Day)
                {
                    dayIncome += (wealthList[i].Money >= 0 ? wealthList[i].Money : 0);
                    dayOutcome += (wealthList[i].Money >= 0 ? 0 : wealthList[i].Money);
                }
            }
        }
        FindCurrentWealth(wealthList, 0);
        FindCurrentWealth(wealthList, 1);

        m_AllMoneyTxt.text = "总资产：" + allMoney;
        m_YearIncomeTxt.text = "年收益：" + CheckMoney(yearIncome);
        m_YearOutcomeTxt.text = "年支出：" + CheckMoney(yearOutcome);
        m_MonthIncomeTxt.text = "月收益：" + CheckMoney(monthIncome);
        m_MonthOutcomeTxt.text = "月支出：" + CheckMoney(monthOutcome);
        m_DayIncomeTxt.text = "日收益：" + CheckMoney(dayIncome);
        m_DayOutcomeTxt.text = "日支出：" + CheckMoney(dayOutcome);

        CheckFontStyle(yearIncome, yearOutcome, m_YearIncomeTxt, m_YearOutcomeTxt);
        CheckFontStyle(monthIncome, monthOutcome, m_MonthIncomeTxt, m_MonthOutcomeTxt);
        CheckFontStyle(dayIncome, monthOutcome, m_DayIncomeTxt, m_DayOutcomeTxt);
    }

    private void CheckFontStyle(float income, float outcome, Text incomeTxt, Text outcomeTxt)
    {
        float res = income + outcome;

        incomeTxt.fontStyle = FontStyle.Normal;
        outcomeTxt.fontStyle = FontStyle.Normal;
        if (res > 0)
        {
            incomeTxt.fontStyle = FontStyle.Bold;
        }
        else if (res < 0)
        {
            outcomeTxt.fontStyle = FontStyle.Bold;
        }
        else
        {
            outcomeTxt.fontStyle = FontStyle.Normal;
            incomeTxt.fontStyle = FontStyle.Normal;
        }
    }

    private string CheckMoney(float inMoney)
    {
        string res = "";
        if (inMoney > 0)
            res = "<color=green> +" + inMoney + "</color>";
        else if (inMoney < 0)
            res = "<color=red> " + inMoney + "</color>";
        else
            res = "0";
        return res;
    }

    /// <summary>
    /// 找到最近的支出收入信息
    /// </summary>
    private void FindCurrentWealth(List<WealthNote> inWealthList,int inIndex)
    {
        WealthNote incomeWealth = null;
        WealthNote outcomeWealth = null;

        for (int i = 0; i < inWealthList.Count; i++)
        {
            if (inWealthList[i].Money > 0 && (incomeWealth == null || inWealthList[i].Date.Ticks > incomeWealth.Date.Ticks))
                incomeWealth = inWealthList[i];
            if (inWealthList[i].Money < 0 && (outcomeWealth == null || inWealthList[i].Date.Ticks > outcomeWealth.Date.Ticks))
                outcomeWealth = inWealthList[i];
        }
        if (currentIncomeItems.Count > inIndex)
        {
            if (incomeWealth != null)
            {
                inWealthList.Remove(incomeWealth);
                currentIncomeItems[inIndex].gameObject.SetActive(true);
                currentIncomeItems[inIndex].UpdateUI(incomeWealth);
            }
            else
            {
                currentIncomeItems[inIndex].gameObject.SetActive(false);
            }
        }
        if (currentOutComeItems.Count > inIndex)
        {
            if (outcomeWealth != null)
            {
                inWealthList.Remove(outcomeWealth);
                currentOutComeItems[inIndex].gameObject.SetActive(true);
                currentOutComeItems[inIndex].UpdateUI(outcomeWealth);
            }
            else
            {
                currentOutComeItems[inIndex].gameObject.SetActive(false);
            }
        }
    }

    public override void OnPop()
    {
        gameObject.SetActive(false);
    }
    public override void OnResume()
    {
        gameObject.SetActive(true);
    }

    private void OnTypeBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.WealthTypePanel);
    }
    private void OnAccountBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AccountPanel);
    }
}
