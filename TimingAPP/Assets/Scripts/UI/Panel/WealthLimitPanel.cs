using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthLimitPanel : BasePanel
{
    private Dropdown m_TypeDrop;
    private Button m_StartBtn;
    private Text m_StartTxt;
    private Button m_EndBtn;
    private Text m_EndTxt;
    private Button m_SureBtn;
    private Button m_CancelBtn;
    private Button m_Btn;

    private int startYear;
    private int endYear;

    private DateTime startTime;
    private DateTime endTime;

    private Action<List<WealthNote>> sureCallBack;

    private void Awake()
    {
        m_StartBtn = transform.Find("StartBtn").GetComponent<Button>();
        m_EndBtn = transform.Find("EndBtn").GetComponent<Button>();
        m_StartTxt = transform.Find("StartBtn/Text").GetComponent<Text>();
        m_EndTxt = transform.Find("EndBtn/Text").GetComponent<Text>();
        m_SureBtn = transform.Find("SureBtn").GetComponent<Button>();
        m_CancelBtn = transform.Find("CancelBtn").GetComponent<Button>();
        m_Btn = GetComponent<Button>();
        m_TypeDrop = transform.Find("Dropdown").GetComponent<Dropdown>();

        m_StartBtn.onClick.AddListener(OnStartBtnClick);
        m_EndBtn.onClick.AddListener(OnEndBtnClick);
        m_SureBtn.onClick.AddListener(OnSureBtnClick);
        m_Btn.onClick.AddListener(OnCancelBtnClick);
        m_CancelBtn.onClick.AddListener(OnCancelBtnClick);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        if (inPara != null)
            sureCallBack = (Action<List<WealthNote>>)inPara;

        List<WealthNote> noteList = WealthManager.Instance.GetWealthNotesByType();
        startYear = noteList[noteList.Count - 1].Date.Year - DateTime.Now.Year;
        endYear = 1;

        m_TypeDrop.options.Clear();
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        optionDatas.Add(new Dropdown.OptionData("全部类型"));
        List<TypeData> typeDatas = WealthManager.Instance.GetWealthTypes();
        for (int i = 0; i < typeDatas.Count; i++)
        {
            optionDatas.Add(new Dropdown.OptionData(typeDatas[i].name));
        }
        m_TypeDrop.AddOptions(optionDatas);
        m_TypeDrop.value = 0;
        m_TypeDrop.captionText.text = m_TypeDrop.options[0].text;

        //默认选择
        OnStartTimeCallBack(noteList[noteList.Count - 1].Date);
        OnEndTimeCallBack(System.DateTime.Now);
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }
    public override void OnResume()
    {

    }
    private void OnStartBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel,new SelectTimeData { callback= OnStartTimeCallBack,startYear = this.startYear,endYear = this.endYear });
    }
    public void OnStartTimeCallBack(DateTime inDateTime)
    {
        startTime = inDateTime;
        m_StartTxt.text = Tools.GetTimeStringDay(inDateTime) + " " + Tools.GetTimeStringMin(inDateTime);
    }
    private void OnEndBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel, new SelectTimeData { callback = OnEndTimeCallBack, startYear = startYear, endYear = endYear });
    }
    public void OnEndTimeCallBack(DateTime inDateTime)
    {
        endTime = inDateTime;
        m_EndTxt.text = Tools.GetTimeStringDay(inDateTime) + " " + Tools.GetTimeStringMin(inDateTime);
    }
    private void OnSureBtnClick()
    {
        List<WealthNote> noteList = new List<WealthNote>();

        if (m_TypeDrop.value == 0)
        {
            noteList.AddRange(WealthManager.Instance.GetWealthNotesByType());
        }
        else
        {
            TypeData typeData = WealthManager.Instance.GetTypeByName(m_TypeDrop.captionText.text);
            noteList.AddRange(WealthManager.Instance.GetWealthNotesByType(typeData.typeId));
        }
        noteList.RemoveAll((note) => { return note.Date.Ticks < startTime.Ticks || note.Date.Ticks > endTime.Ticks; });

        sureCallBack?.Invoke(noteList);
        UIManager.Instance.PopPanel();
    }
    private void OnCancelBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
}
