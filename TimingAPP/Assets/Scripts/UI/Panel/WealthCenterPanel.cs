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

    private Dropdown m_TypeSelect;
    private Button m_TimeBtn;
    private Text m_TimeTxt;

    private DateTime curDate;

    private Dropdown m_IncomeAvgSelect;
    private Dropdown m_OutcomeAvgSelect;
    private Text m_IncomeAvgTxt;
    private Text m_OutcomeAvgTxt;

    private Transform m_CurOutcomeBg;
    private Transform m_CurIncomeBg;

    //最近提示
    private List<WealthCenterItem> currentIncomeItems = new List<WealthCenterItem>();
    private List<WealthCenterItem> currentOutComeItems = new List<WealthCenterItem>();

    private List<WealthNote> curIncomeNotes = new List<WealthNote>();
    private List<WealthNote> curOutcomeNotes = new List<WealthNote>();

    private void Awake()
    {
        m_TypeBtn = transform.Find("TypeBtn").GetComponent<Button>();
        m_AccountBtn = transform.Find("AccountBtn").GetComponent<Button>();

        m_AllMoneyTxt = transform.Find("MyAccountBg/AllMoneyTxt").GetComponent<Text>();
        m_YearIncomeTxt = transform.Find("MyAccountBg/Income/YearIncome").GetComponent<Text>();
        m_YearOutcomeTxt = transform.Find("MyAccountBg/Outcome/YearOutcome").GetComponent<Text>();
        m_MonthIncomeTxt = transform.Find("MyAccountBg/Income/MonthIncome").GetComponent<Text>();
        m_MonthOutcomeTxt = transform.Find("MyAccountBg/Outcome/MonthOutcome").GetComponent<Text>();
        m_DayIncomeTxt = transform.Find("MyAccountBg/Income/DayIncome").GetComponent<Text>();
        m_DayOutcomeTxt = transform.Find("MyAccountBg/Outcome/DayOutcome").GetComponent<Text>();

        m_TypeSelect = transform.Find("MyAccountBg/TypeSelect").GetComponent<Dropdown>();
        m_TimeBtn = transform.Find("MyAccountBg/TimeBtn").GetComponent<Button>();
        m_TimeTxt = transform.Find("MyAccountBg/TimeBtn/TimeTxt").GetComponent<Text>();

        m_IncomeAvgSelect = transform.Find("AvgIncomeBg/Dropdown").GetComponent<Dropdown>();
        m_OutcomeAvgSelect = transform.Find("AvgOutcomeBg/Dropdown").GetComponent<Dropdown>();
        m_IncomeAvgTxt = transform.Find("AvgIncomeBg/MoneyTxt").GetComponent<Text>();
        m_OutcomeAvgTxt = transform.Find("AvgOutcomeBg/MoneyTxt").GetComponent<Text>();

        m_CurOutcomeBg = transform.Find("CurOutcomeBg");
        m_CurIncomeBg = transform.Find("CurIncomeBg");

        m_TimeBtn.onClick.AddListener(OnTimeBtnClick);
        m_TypeBtn.onClick.AddListener(OnTypeBtnClick);
        m_AccountBtn.onClick.AddListener(OnAccountBtnClick);
        m_TypeSelect.onValueChanged.AddListener(OnTypeSelectChanged);
        m_IncomeAvgSelect.onValueChanged.AddListener(OnIncomeSelectChanged);
        m_OutcomeAvgSelect.onValueChanged.AddListener(OnOutcomeSelectChanged);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        UpdateType();
        UpdateUI(DateTime.Now);

        m_IncomeAvgSelect.value = 0;
        m_IncomeAvgSelect.captionText.text = m_IncomeAvgSelect.options[0].text;
        OnIncomeSelectChanged(0);
        m_OutcomeAvgSelect.value = 0;
        m_OutcomeAvgSelect.captionText.text = m_OutcomeAvgSelect.options[0].text;
        OnOutcomeSelectChanged(0);

        StartCoroutine(StartFlyTxt());
    }

    IEnumerator StartFlyTxt()
    {
        int curIncomeIndex = 0;
        int curOutcomeIndex = 0;

        curIncomeNotes.Clear();
        curOutcomeNotes.Clear();
        List<WealthNote> wealthNotes = WealthManager.Instance.GetWealthNotesByType();
        for (int i = 0; i < wealthNotes.Count; i++)
        {
            if (wealthNotes[i].Money > 0)
            {
                if (curIncomeNotes.Count < 3)
                    curIncomeNotes.Add(wealthNotes[i]);
            }
            else
            {
                if (curOutcomeNotes.Count < 3)
                    curOutcomeNotes.Add(wealthNotes[i]);
            }
        }
        GameObject go = null;
        while (true)
        {
            if (curIncomeNotes.Count > 0)
            {
                go = GameObject.Instantiate(Resources.Load<GameObject>("InComeItem"), m_CurIncomeBg);
                go.GetComponent<InComeItem>().UpdateUI(curIncomeNotes[curIncomeIndex].Date, curIncomeNotes[curIncomeIndex].Money);

                curIncomeIndex++;
                curIncomeIndex %= curIncomeNotes.Count;
            }
            if (curOutcomeNotes.Count > 0)
            {
                go = GameObject.Instantiate(Resources.Load<GameObject>("InComeItem"), m_CurOutcomeBg);
                go.GetComponent<InComeItem>().UpdateUI(curOutcomeNotes[curOutcomeIndex].Date, curOutcomeNotes[curOutcomeIndex].Money);

                curOutcomeIndex++;
                curOutcomeIndex %= curOutcomeNotes.Count;
            }
            yield return new WaitForSeconds(2);
        }
    }

    private void UpdateType()
    {
        m_TypeSelect.options.Clear();
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        optionDatas.Add(new Dropdown.OptionData("全部类型"));
        List<TypeData> typeDatas = WealthManager.Instance.GetWealthTypes();
        for (int i = 0; i < typeDatas.Count; i++)
        {
            optionDatas.Add(new Dropdown.OptionData(typeDatas[i].name));
        }
        m_TypeSelect.AddOptions(optionDatas);
        m_TypeSelect.value = 0;
        m_TypeSelect.captionText.text = m_TypeSelect.options[0].text;
    }
    private void UpdateUI(DateTime inDate)
    {
        curDate = inDate;
        float allMoney = 0;
        float yearIncome = 0;
        float monthIncome = 0;
        float dayIncome = 0;
        float yearOutcome = 0;
        float monthOutcome = 0;
        float dayOutcome = 0;

        m_TimeTxt.text = Tools.GetTimeStringDay(inDate);

        List<WealthNote> wealthList = WealthManager.Instance.GetWealthNotesByType();

        if (wealthList != null && wealthList.Count > 0)
        {
            for (int i = 0; i < wealthList.Count; i++)
            {
                allMoney += wealthList[i].Money;
                if (m_TypeSelect.value == 0 || m_TypeSelect.captionText.text == wealthList[i].PayTypeName)
                {
                    if (wealthList[i].Date.Year == inDate.Year)
                    {
                        yearIncome += (wealthList[i].Money >= 0 ? wealthList[i].Money : 0);
                        yearOutcome += (wealthList[i].Money >= 0 ? 0 : wealthList[i].Money);

                        if (wealthList[i].Date.Month == inDate.Month)
                        {
                            monthIncome += (wealthList[i].Money >= 0 ? wealthList[i].Money : 0);
                            monthOutcome += (wealthList[i].Money >= 0 ? 0 : wealthList[i].Money);

                            if (wealthList[i].Date.Day == inDate.Day)
                            {
                                dayIncome += (wealthList[i].Money >= 0 ? wealthList[i].Money : 0);
                                dayOutcome += (wealthList[i].Money >= 0 ? 0 : wealthList[i].Money);
                            }
                        }
                    }
                }
            }
        }
        //FindCurrentWealth(wealthList, 0);
        //FindCurrentWealth(wealthList, 1);

        m_AllMoneyTxt.text = "总资产：" + allMoney;
        m_YearIncomeTxt.text = "年收益：" + Tools.CheckMoney(yearIncome);
        m_YearOutcomeTxt.text = "年支出：" + Tools.CheckMoney(yearOutcome);
        m_MonthIncomeTxt.text = "月收益：" + Tools.CheckMoney(monthIncome);
        m_MonthOutcomeTxt.text = "月支出：" + Tools.CheckMoney(monthOutcome);
        m_DayIncomeTxt.text = "日收益：" + Tools.CheckMoney(dayIncome);
        m_DayOutcomeTxt.text = "日支出：" + Tools.CheckMoney(dayOutcome);

        //CheckFontStyle(yearIncome, yearOutcome, m_YearIncomeTxt, m_YearOutcomeTxt);
        //CheckFontStyle(monthIncome, monthOutcome, m_MonthIncomeTxt, m_MonthOutcomeTxt);
        //CheckFontStyle(dayIncome, monthOutcome, m_DayIncomeTxt, m_DayOutcomeTxt);
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
        StopAllCoroutines();
    }
    public override void OnResume()
    {
        gameObject.SetActive(true);
        UpdateUI(curDate);
        StartCoroutine(StartFlyTxt());
    }
    public override void OnPending()
    {
        StopAllCoroutines();
    }

    private void OnTypeBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.WealthTypePanel);
    }
    private void OnAccountBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AccountPanel);
    }
    private void OnTimeBtnClick()
    {
        int startYear = 0;
        int endYear = 0;

        List<WealthNote> noteList = null;
        noteList = WealthManager.Instance.GetWealthNotesByType();
        if (noteList.Count > 0)
        {
            startYear = noteList[noteList.Count - 1].Date.Year - DateTime.Now.Year;
        }
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel, new SelectTimeData { callback = UpdateUI, startYear = startYear, endYear = endYear });
    }
    private void OnTypeSelectChanged(int inValue)
    {
        UpdateUI(curDate);
    }

    private void GetMoneyAvg(int inValue,bool inIsIncome)
    {
        List<WealthNote> notes = WealthManager.Instance.GetWealthNotesByType();
        int count = 1;
        float incomeMoney = 0;
        float outcomeMoney = 0;

        for (int i = 0; i < notes.Count; i++)
        {
            if (notes[i].Money > 0)
            {
                incomeMoney += notes[i].Money;
            }
            else
            {
                outcomeMoney += notes[i].Money;
            }
        }

        if (notes.Count > 0)
        {
            switch (inValue)
            {
                case 0:
                    DateTime start = Convert.ToDateTime(notes[notes.Count - 1].Date.ToShortDateString());
                    DateTime end = Convert.ToDateTime(notes[0].Date.ToShortDateString());
                    TimeSpan sp = end.Subtract(start);
                    count = sp.Days + 1;
                    break;
                case 1:
                    count = ((notes[0].Date.Year - notes[notes.Count - 1].Date.Year) * 12) + notes[0].Date.Month - notes[notes.Count - 1].Date.Month + 1;
                    break;
                case 2:
                    count = notes[notes.Count - 1].Date.Year - notes[0].Date.Year + 1;
                    break;
                default:
                    break;
            }
        }
        if(inIsIncome)
            m_IncomeAvgTxt.text =  "+" + (incomeMoney / count).ToString("f1");
        else
            m_OutcomeAvgTxt.text = (outcomeMoney / count).ToString("f1");
    }
    private void OnIncomeSelectChanged(int inValue)
    {
        GetMoneyAvg(inValue,true);
    }
    private void OnOutcomeSelectChanged(int inValue)
    {
        GetMoneyAvg(inValue,false);
    }
}
