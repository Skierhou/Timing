using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class WealthLinePanel : BasePanel
{
    private GridLayoutGroup m_Grid;
    private Text m_TimeTxt;
    private Button m_TimeBtn;
    private Button m_CancelBtn;
    private Dropdown m_TypeSelect;
    private GameObject m_Tip;
    private Text m_TotalMoneyTxt;

    private DateTime curDate;

    private void Awake()
    {
        m_TimeTxt = transform.Find("ScrollView/TimeBtn/TimeTxt").GetComponent<Text>();
        m_TimeBtn = transform.Find("ScrollView/TimeBtn").GetComponent<Button>();
        m_TypeSelect = transform.Find("ScrollView/TypeSelect").GetComponent<Dropdown>();
        m_CancelBtn = transform.Find("CancelBtn").GetComponent<Button>();
        m_Grid = GetComponentInChildren<GridLayoutGroup>();
        m_Tip = transform.Find("Tip").gameObject;
        m_TotalMoneyTxt = transform.Find("Total/MoneyTxt").GetComponent<Text>();

        m_TimeBtn.onClick.AddListener(OnTimeBtnClick);
        m_CancelBtn.onClick.AddListener(OnCancelBtnClick);
        m_TypeSelect.onValueChanged.AddListener(OnTypeSelectChanged);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        curDate = DateTime.Now;

        UpdateType();
        OnSelectTimeCallBack(curDate);
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
        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        List<WealthNote> notes = new List<WealthNote>();
        if (m_TypeSelect.value == 0)
        {
            notes.AddRange(WealthManager.Instance.GetWealthNotesByType());
        }
        else
        {
            TypeData typeData = WealthManager.Instance.GetTypeByName(m_TypeSelect.captionText.text);
            notes.AddRange(WealthManager.Instance.GetWealthNotesByType(typeData.typeId));
        }

        int day = DateTime.DaysInMonth(inDate.Year, inDate.Month);
        DateTime startDate = new DateTime(inDate.Year, inDate.Month, 1);
        DateTime endDate = new DateTime(inDate.Year,inDate.Month,day);

        notes.RemoveAll((note) => { return note.Date.Ticks < startDate.Ticks || note.Date.Ticks > endDate.Ticks; });

        m_Tip.gameObject.SetActive(notes.Count == 0);
        m_TotalMoneyTxt.transform.parent.gameObject.SetActive(notes.Count != 0);

        if (notes.Count > 0)
        {
            float maxMoney = notes.Max((n) => Mathf.Abs(n.Money));

            float rate = maxMoney / 140.0f;

            float money = 0;
            int targetW = 0;        //计算线的高度
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                targetW = (int)(Mathf.Abs(notes[i].Money) / rate);
                GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("AccountLineItem"), m_Grid.transform);
                go.GetComponent<AccountLineItem>().Initialize(notes[i], targetW);
                money += notes[i].Money;
            }
            m_TotalMoneyTxt.text = Tools.CheckMoney(money);
        }

        //更新grid
        float width = (m_Grid.cellSize.x + m_Grid.spacing.x) * notes.Count + m_Grid.padding.left + m_Grid.padding.right;
        Rect rect = ((RectTransform)m_Grid.transform.parent).rect;
        ((RectTransform)m_Grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Horizontal, width);
        rect.position = Vector2.zero;
    }

    public override void OnPop()
    {
        gameObject.SetActive(false);
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
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel,new SelectTimeData { startYear = startYear,endYear=endYear,callback = OnSelectTimeCallBack,bCantShowDaySelect = true });
    }
    public void OnSelectTimeCallBack(DateTime inDate)
    {
        m_TimeTxt.text = inDate.Year + "-" + Tools.SuppleTime(inDate.Month);
        curDate = inDate;
        UpdateUI(inDate);
    }

    private void OnCancelBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
    private void OnTypeSelectChanged(int inValue)
    {
        UpdateUI(curDate);
    }
}
