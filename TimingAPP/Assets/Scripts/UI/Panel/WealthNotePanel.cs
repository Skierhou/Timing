using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthNotePanel : BasePanel
{
    private Button m_ReturnBtn;
    private Button m_SureBtn;
    private Button m_TimeBtn;
    private Button m_SignBtn;
    private Text m_SignTxt;
    private Text m_TimeTxt;
    private InputField m_MoneyInput;
    private Dropdown m_TypeOption;
    private InputField m_DesInput;

    private WealthNote wealthNote;
    private WealthNote newWealthNote;

    private void Awake()
    {
        m_TimeBtn = transform.Find("TimeBtn").GetComponent<Button>();
        m_TimeTxt = transform.Find("TimeBtn/ContentTxt").GetComponent<Text>();
        m_MoneyInput = transform.Find("Money/MoneyInput").GetComponent<InputField>();
        m_TypeOption = GetComponentInChildren<Dropdown>();
        m_DesInput = transform.Find("Des/DesInput").GetComponent<InputField>();
        m_ReturnBtn = transform.Find("ReturnBtn").GetComponent<Button>();
        m_SureBtn = transform.Find("SureBtn").GetComponent<Button>();
        m_SignBtn = transform.Find("Money/SignBtn").GetComponent<Button>();
        m_SignTxt = m_SignBtn.transform.Find("Text").GetComponent<Text>();

        m_TimeBtn.onClick.AddListener(OnTimeBtnClick);
        m_SureBtn.onClick.AddListener(OnSureBtnClick);
        m_ReturnBtn.onClick.AddListener(OnReturnBtnClick);
        m_MoneyInput.onValueChanged.AddListener(OnMoneyInputChanged);
        m_SignBtn.onClick.AddListener(OnSignBtnClick);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);
        if (inPara != null)
            wealthNote = (WealthNote)inPara;

        if (wealthNote != null)
        {
            newWealthNote = new WealthNote(0, wealthNote.Date, wealthNote.Content, wealthNote.Money, wealthNote.PayTypeId, wealthNote.PayTypeName, wealthNote.Color);

            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        m_TimeTxt.text = m_TimeTxt.text = Tools.GetTimeStringDay(wealthNote.Date) + "\n" + Tools.GetTimeStringMin(wealthNote.Date);
        if (wealthNote.Money >= 0)
            m_SignTxt.text = "+";
        else
            m_SignTxt.text = "-";
        m_MoneyInput.text = Math.Abs(wealthNote.Money).ToString();
        m_DesInput.text = wealthNote.Content;

        m_TypeOption.options.Clear();
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        optionDatas.Add(new Dropdown.OptionData(wealthNote.PayTypeName));
        List<TypeData> typeDatas = WealthManager.Instance.GetWealthTypes();
        for (int i = 0; i < typeDatas.Count; i++)
        {
            if(!wealthNote.PayTypeName.Equals(typeDatas[i].name))
                optionDatas.Add(new Dropdown.OptionData(typeDatas[i].name));
        }
        m_TypeOption.AddOptions(optionDatas);
        m_TypeOption.value = 0;
        m_TypeOption.captionText.text = m_TypeOption.options[0].text;
    }

    public override void OnPop()
    {
        gameObject.SetActive(false);
    }
    private void OnTimeBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel, new SelectTimeData { callback = OnSelectTimeCallBack
            , startYear = wealthNote.Date.Year - DateTime.Now.Year, endYear = 0 });
    }
    public void OnSelectTimeCallBack(DateTime inDate)
    {
        newWealthNote.Date = inDate;
        m_TimeTxt.text = Tools.GetTimeStringDay(inDate)+"\n"+Tools.GetTimeStringMin(inDate);
    }
    private void OnSureBtnClick()
    {
        newWealthNote.Content = m_DesInput.text;
        int sign = m_SignTxt.text == "+" ? 1 : -1;
        newWealthNote.Money = int.Parse(m_MoneyInput.text) * sign;

        TypeData typeData = WealthManager.Instance.GetTypeByName(m_TypeOption.captionText.text);
        newWealthNote.PayTypeName = typeData.name;
        newWealthNote.PayTypeId = typeData.typeId;
        newWealthNote.Color = typeData.color;

        WealthManager.Instance.RemoveNote(wealthNote.PayTypeId, wealthNote.Id);
        WealthManager.Instance.AddNote(newWealthNote);

        UIManager.Instance.PopPanel();
        EPanelType panelType = UIManager.Instance.GetCurPanelType();
        UIManager.Instance.PopPanel();
        UIManager.Instance.PushPanel(panelType);
    }
    private void OnReturnBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
    private void OnMoneyInputChanged(string inValue)
    {
        //安全校验
        if (!Tools.IsNumeric(inValue))
        {
            if (inValue.Length > 1)
                m_MoneyInput.text = inValue.Substring(0, inValue.Length - 1);
            else
                m_MoneyInput.text = "";
        }
    }
    private void OnSignBtnClick()
    {
        if (m_SignTxt.text == "+")
            m_SignTxt.text = "-";
        else
            m_SignTxt.text = "+";
    }
}
