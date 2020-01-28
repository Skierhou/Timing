using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using static UnityEngine.RectTransform;

public class SelectTimePanel : BasePanel
{
    public Color DayHighLightColor;
    public Color DayLowLightColor;

    //选择小时/分界面
    private Button m_MorningBtn;
    private Button m_AfternoonBtn;
    private Text m_MorningTxt;
    private Text m_AfternoonTxt;
    private MyScrollRect m_HourScroll;
    private MyScrollRect m_MinScroll;
    private GridLayoutGroup m_MinGrid;
    private GridLayoutGroup m_HourGrid;

    //选择日期界面
    private GameObject m_DateSelectPanel;
    private Text m_YearTxt;
    private Button m_YearBtn;
    private Text m_NextDayTxt;
    private Button m_SureBtn;
    private Button m_CancelBtn;
    private DateItem[] dateItems;

    //选年月界面
    private GameObject m_YearPanel;
    private GridLayoutGroup m_YearGrid;
    private GridLayoutGroup m_MonthGrid;
    private MyScrollRect m_YearScroll;
    private MyScrollRect m_MonthScroll;

    private DateSelectItem[] yearItems;
    private DateSelectItem[] monthItems;
    private DateSelectItem[] hourItems;
    private DateSelectItem[] minItems;

    private AddPlanNotePanel addPlanNotePanel;

    private Tweener monthTweener;
    private Tweener yearTweener;
    private Tweener hourTweener;
    private Tweener minTweener;

    private bool IsMorning;

    private DateItem curDateItem;

    private int year = 0;
    private int month = 0;
    private int day = 0;
    private int hour = 0;
    private int min = 0;

    public DateTime selectDateTime;

    private void Awake()
    {
        m_DateSelectPanel = transform.Find("Bg").gameObject;
        m_YearTxt = transform.Find("Bg/YearTxt").GetComponent<Text>();
        m_YearBtn = transform.Find("Bg/YearTxt").GetComponent<Button>();
        m_NextDayTxt = transform.Find("Bg/NextDayTxt").GetComponent<Text>();
        m_SureBtn = transform.Find("SureBtn").GetComponent<Button>();
        m_CancelBtn = transform.Find("CancelBtn").GetComponent<Button>();

        dateItems = transform.Find("Bg/DateGrid").GetComponentsInChildren<DateItem>();
        monthItems = transform.Find("YearBg/MonthSelect/Viewport/Content/Grid").GetComponentsInChildren<DateSelectItem>();
        m_YearScroll = transform.Find("YearBg/YearSelect").GetComponent<MyScrollRect>();
        m_MonthScroll = transform.Find("YearBg/MonthSelect").GetComponent<MyScrollRect>();
        m_YearGrid = transform.Find("YearBg/YearSelect/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
        m_MonthGrid = transform.Find("YearBg/MonthSelect/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
        m_YearPanel = transform.Find("YearBg").gameObject;

        m_MorningBtn = transform.Find("Bg/DayTimeSelect/MorningBtn").GetComponent<Button>();
        m_AfternoonBtn = transform.Find("Bg/DayTimeSelect/AfternoonBtn").GetComponent<Button>();
        m_MorningTxt = m_MorningBtn.transform.Find("Text").GetComponent<Text>();
        m_AfternoonTxt = m_AfternoonBtn.transform.Find("Text").GetComponent<Text>();
        m_HourScroll = transform.Find("Bg/DayTimeSelect/HourSelect").GetComponent<MyScrollRect>();
        m_MinScroll = transform.Find("Bg/DayTimeSelect/MinSelect").GetComponent<MyScrollRect>();
        m_MinGrid = m_MinScroll.transform.GetComponentInChildren<GridLayoutGroup>();
        m_HourGrid = m_HourScroll.transform.GetComponentInChildren<GridLayoutGroup>();
        hourItems = m_HourGrid.transform.GetComponentsInChildren<DateSelectItem>();

        m_YearScroll.Initialize(this, EScrollType.YearSelect);
        m_MonthScroll.Initialize(this, EScrollType.MonthSelect);
        m_HourScroll.Initialize(this, EScrollType.HourSelect);
        m_MinScroll.Initialize(this, EScrollType.MinSelect);

        m_YearBtn.onClick.AddListener(YearBtnClick);
        m_SureBtn.onClick.AddListener(SureBtnClick);
        m_CancelBtn.onClick.AddListener(CancelBtnClick);
        m_MorningBtn.onClick.AddListener(MorningBtnClick);
        m_AfternoonBtn.onClick.AddListener(AfternoonBtnClick);
        m_YearScroll.onValueChanged.AddListener(YearChanged);
        m_MonthScroll.onValueChanged.AddListener(MonthChanged);
        m_HourScroll.onValueChanged.AddListener(HourChanged);
        m_MinScroll.onValueChanged.AddListener(MinChanged);
    }

    public void SelectDay(DateItem inItem, DateTime inDate)
    {
        if (curDateItem != null && curDateItem != inItem)
        {
            curDateItem.UnSelect();
        }
        curDateItem = inItem;
        year = inDate.Year;
        month = inDate.Month;
        day = inDate.Day;
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);
        if (inPara != null)
        {
            addPlanNotePanel = (AddPlanNotePanel)inPara;
        }
        //清空
        m_DateSelectPanel.SetActive(false);
        m_YearPanel.SetActive(true);
        curDateItem = null;
        for (int i = 0; i < dateItems.Length; i++)
        {
            dateItems[i].UnSelect();
        }
        year = 0;
        month = 0;
        day = 0;
        hour = 0;
        min = 0;

        //显示选择年份UI
        ShowYearUI(0, 50);
        ShowMinuteUI();
        MonthEndDrag();
        YearEndDrag();
        HourEndDrag();
        MinEndDrag();
        MorningBtnClick();
    }

    private void ShowYearUI(int minYear, int maxYear)
    {
        for (int i = 0; i < monthItems.Length; i++)
        {
            monthItems[i].SetHighLight(false);
        }
        monthItems[4].SetHighLight(true);

        foreach (Transform child in m_YearGrid.transform)
        {
            if (child != m_YearGrid.transform)
                GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_YearGrid.transform);
        }
        List<DateSelectItem> itemList = new List<DateSelectItem>();
        for (int i = minYear; i < maxYear; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_YearGrid.transform);
            DateSelectItem dateItem = go.GetComponent<DateSelectItem>();
            dateItem.Initialize(System.DateTime.Now.Year + i);
            itemList.Add(dateItem);
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_YearGrid.transform);
        }
        yearItems = itemList.ToArray();

        //刷新grid大小
        float height = (m_YearGrid.cellSize.y + m_YearGrid.spacing.y) * (itemList.Count + 8) + m_YearGrid.padding.top + m_YearGrid.padding.bottom + 10;
        Rect rect = ((RectTransform)m_YearGrid.transform.parent).rect;
        ((RectTransform)m_YearGrid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
        rect.position = Vector2.zero;
    }

    private void ShowMinuteUI()
    {
        List<DateSelectItem> itemList = new List<DateSelectItem>();
        foreach (Transform child in m_MinGrid.transform)
        {
            if (child != m_MinGrid.transform)
                GameObject.Destroy(child.gameObject);
        }
        int length = 2;
        for (int i = 0; i < length; i++)
        {
            GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem1"), m_MinGrid.transform);
        }
        for (int i = 0; i < 60; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem1"), m_MinGrid.transform);
            DateSelectItem selectItem = go.GetComponent<DateSelectItem>();
            selectItem.Initialize(i);
            itemList.Add(selectItem);
        }
        for (int i = 0; i < length; i++)
        {
            GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem1"), m_MinGrid.transform);
        }
        minItems = itemList.ToArray();

        //刷新grid大小
        float height = (m_MinGrid.cellSize.y + m_MinGrid.spacing.y) * (itemList.Count + 4) + m_MinGrid.padding.top + m_MinGrid.padding.bottom;
        Rect rect = ((RectTransform)m_MinGrid.transform.parent).rect;
        ((RectTransform)m_MinGrid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
        rect.position = Vector2.zero;
    }

    private void UpdateDateUI()
    {
        m_YearTxt.text = year + "年" + month + "月";
        m_NextDayTxt.text = "今天:" + System.DateTime.Now.Year + "年" + System.DateTime.Now.Month + "月" + System.DateTime.Now.Day + "日";

        DateTime firstDay = System.DateTime.Now;

        if (month != firstDay.Month)
            firstDay = firstDay.AddMonths(month - firstDay.Month);
        if (year != firstDay.Year)
            firstDay = firstDay.AddYears(year - firstDay.Year);

        //获得当前天数
        firstDay = firstDay.AddDays(1 - firstDay.Day);
        //本月第一天是周几
        int weekday = (int)firstDay.DayOfWeek;

        for (int i = 0; i < weekday; i++)
        {
            dateItems[i].SetHide(true);
        }
        int monthDay = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);

        int dayIndex = 1;
        for (int i = weekday; i < monthDay + weekday; i++)
        {
            dateItems[i].SetHide(false);
            dateItems[i].Initialize(this, new DateTime(firstDay.Year, firstDay.Month, dayIndex++), 0);
        }
        for (int i = monthDay + weekday; i < dateItems.Length; i++)
        {
            dateItems[i].SetHide(true);
        }
    }

    public override void OnPop()
    {
        gameObject.SetActive(false);
    }
    public override void OnResume()
    {

    }

    private void YearBtnClick()
    {
        m_YearPanel.SetActive(true);
    }
    private void SureBtnClick()
    {
        if (m_YearPanel.activeSelf)
        {
            m_YearPanel.SetActive(false);
            m_DateSelectPanel.SetActive(true);
            UpdateDateUI();
        }
        else
        {
            if (day == 0)
            {
                //TODO:提示选择日期
            }
            else
            {
                if (!IsMorning)
                    hour += 12;
                hour = hour == 24 ? 0 : hour;
                selectDateTime = new DateTime(year, month, day, hour, min, 0);
                if (addPlanNotePanel != null && selectDateTime != null)
                {
                    addPlanNotePanel.SetEndDate(selectDateTime);
                }
                UIManager.Instance.PopPanel();
            }
        }
    }
    private void CancelBtnClick()
    {
        if (m_YearPanel.activeSelf)
        {
            UIManager.Instance.PopPanel();
        }
        else
        {
            m_YearPanel.SetActive(true);
        }
    }

    private void MorningBtnClick()
    {
        IsMorning = true;
        m_MorningTxt.color = DayHighLightColor;
        m_AfternoonTxt.color = DayLowLightColor;
    }
    private void AfternoonBtnClick()
    {
        IsMorning = false;
        m_MorningTxt.color = DayLowLightColor;
        m_AfternoonTxt.color = DayHighLightColor;
    }

    private void YearChanged(Vector2 inVec)
    {
        if(Mathf.Abs(inVec.y) >= 1)
            if (yearTweener != null)
            {
                yearTweener.Kill();
                yearTweener = null;
            }
        UpdateHighLight(yearItems, m_YearGrid, 40, EScrollType.YearSelect);
    }
    private void MonthChanged(Vector2 inVec)
    {
        if (Mathf.Abs(inVec.y) >= 1)
            if (monthTweener != null)
            {
                monthTweener.Kill();
                monthTweener = null;
            }
        UpdateHighLight(monthItems, m_MonthGrid, 40, EScrollType.MonthSelect);
    }
    private void HourChanged(Vector2 inVec)
    {
        if (Mathf.Abs(inVec.y) >= 1)
            if (hourTweener != null)
            {
                hourTweener.Kill();
                hourTweener = null;
            }
        UpdateHighLight(hourItems, m_HourGrid, 25, EScrollType.HourSelect);
    }
    private void MinChanged(Vector2 inVec)
    {
        if (Mathf.Abs(inVec.y) >= 1)
            if (minTweener != null)
            {
                minTweener.Kill();
                minTweener = null;
            }
        UpdateHighLight(minItems, m_MinGrid, 25, EScrollType.MinSelect);
    }

    private void UpdateHighLight(DateSelectItem[] inSelectItems,GridLayoutGroup inGrid,float inHeight,EScrollType inScrollType)
    {
        for (int i = 0; i < inSelectItems.Length; i++)
        {
            inSelectItems[i].SetHighLight(false);
        }

        int count = (int)(inGrid.transform.parent.localPosition.y / inHeight);
        int value = 0;
        if (inGrid.transform.parent.localPosition.y % inHeight > inHeight / 2)
        {
            if (inSelectItems.Length > count + 1 && count + 1 >= 0)
            {
                inSelectItems[count + 1].SetHighLight(true);
                value = inSelectItems[count + 1].value;
            }
            else
            {
                if (inSelectItems.Length > count)
                {
                    inSelectItems[count].SetHighLight(true);
                    value = inSelectItems[count].value;
                }
            }
        }
        else
        {
            if (inSelectItems.Length > count && count >= 0)
            {
                inSelectItems[count].SetHighLight(true);
                value = inSelectItems[count].value;
            }
            else
            {
                if (inSelectItems.Length > count + 1)
                {
                    inSelectItems[count + 1].SetHighLight(true);
                    value = inSelectItems[count + 1].value;
                }
            }
        }
        switch (inScrollType)
        {
            case EScrollType.YearSelect:
                year = value;
                break;
            case EScrollType.MonthSelect:
                month = value;
                break;
            case EScrollType.HourSelect:
                hour = value;
                break;
            case EScrollType.MinSelect:
                min = value;
                break;
        }
    }

    public void YearEndDrag(bool inIsStopAll = true)
    {
        EndDrag(yearTweener, m_YearScroll, m_YearGrid, EScrollType.YearSelect, 40, inIsStopAll, yearItems);
    }
    public void MonthEndDrag(bool inIsStopAll = true)
    {
        EndDrag(monthTweener, m_MonthScroll, m_MonthGrid, EScrollType.MonthSelect, 40, inIsStopAll, monthItems);
    }

    public void HourEndDrag(bool inIsStopAll = true)
    {
        EndDrag(hourTweener, m_HourScroll, m_HourGrid, EScrollType.HourSelect, 25, inIsStopAll, hourItems);
    }

    public void MinEndDrag(bool inIsStopAll = true)
    {
        EndDrag(minTweener, m_MinScroll, m_MinGrid, EScrollType.MinSelect, 25, inIsStopAll, minItems);
    }

    /// <summary>
    /// 停止拖拽
    /// </summary>
    /// <param name="inTweener">动画</param>
    /// <param name="inScroll">滑动</param>
    /// <param name="inGrid">Grid</param>
    /// <param name="inType">Type</param>
    /// <param name="height">单个GridItem的高度</param>
    /// <param name="inIsStopAll">是否完全停止</param>
    private void EndDrag(Tweener inTweener, MyScrollRect inScroll, GridLayoutGroup inGrid, EScrollType inType, float inHeight, bool inIsStopAll,DateSelectItem[] inSelectItems)
    {
        if (!inIsStopAll)
        {
            if (inTweener != null)
                inTweener.Kill();
        }
        else
        {
            int count = (int)(inGrid.transform.parent.localPosition.y / inHeight);
            inScroll.StopMovement();

            if (inTweener != null)
                inTweener.Kill();

            if (inGrid.transform.parent.localPosition.y % inHeight > inHeight / 2)
            {
                inTweener = inGrid.transform.parent.DOLocalMoveY((count + 1) * inHeight, 0.2f);
            }
            else
            {
                inTweener = inGrid.transform.parent.DOLocalMoveY(count * inHeight, 0.2f);
            }
            switch (inType)
            {
                case EScrollType.YearSelect:
                    yearTweener = inTweener;
                    break;
                case EScrollType.MonthSelect:
                    monthTweener = inTweener;
                    break;
                case EScrollType.HourSelect:
                    hourTweener = inTweener;
                    break;
                case EScrollType.MinSelect:
                    minTweener = inTweener;
                    break;
            }
            UpdateHighLight(inSelectItems, inGrid, inHeight, inType);
        }
    }
}