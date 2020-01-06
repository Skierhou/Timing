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
    private Transform m_YearGrid;
    private Transform m_MonthGrid;
    private MyScrollRect m_YearScroll;
    private MyScrollRect m_MonthScroll;

    private DateSelectItem[] yearItems;
    private DateSelectItem[] monthItems;

    private AddPlanNotePanel addPlanNotePanel;

    private Tweener monthTweener;
    private Tweener yearTweener;

    private DateItem curDateItem;

    private int year = 0;
    private int month = 0;
    private int day = 0;

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
        m_YearGrid = transform.Find("YearBg/YearSelect/Viewport/Content/Grid");
        m_MonthGrid = transform.Find("YearBg/MonthSelect/Viewport/Content/Grid");
        m_YearPanel = transform.Find("YearBg").gameObject;

        m_YearScroll.Initialize(this, true);
        m_MonthScroll.Initialize(this, false);

        m_YearBtn.onClick.AddListener(YearBtnClick);
        m_SureBtn.onClick.AddListener(SureBtnClick);
        m_CancelBtn.onClick.AddListener(CancelBtnClick);
        m_YearScroll.onValueChanged.AddListener(YearChanged);
        m_MonthScroll.onValueChanged.AddListener(MonthChanged);
    }

    public void SelectDay(DateItem inItem,DateTime inDate)
    {
        if (curDateItem != null)
            curDateItem.UnSelect();
        curDateItem = inItem;
        selectDateTime = inDate;
    }

    private void Update()
    {
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);
        if (inPara != null)
        {
            addPlanNotePanel = (AddPlanNotePanel)inPara;
        }

        //显示选择年份UI
        ShowYearUI(0,50);
        MonthEndDrag();
        YearEndDrag();
    }

    private void ShowYearUI(int minYear,int maxYear)
    {
        for (int i = 0; i < monthItems.Length; i++)
        {
            monthItems[i].SetHighLight(false);
        }
        monthItems[4].SetHighLight(true);

        foreach (Transform child in m_YearGrid)
        {
            if (child != m_YearGrid)
                GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_YearGrid);
        }
        List<DateSelectItem> itemList = new List<DateSelectItem>();
        for (int i = minYear; i < maxYear; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"),m_YearGrid);
            DateSelectItem dateItem = go.GetComponent<DateSelectItem>();
            dateItem.Initialize(System.DateTime.Now.Year + i);
            itemList.Add(dateItem);
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_YearGrid);
        }
        yearItems = itemList.ToArray();

        //刷新grid大小
        Rect rect = ((RectTransform)m_YearGrid.parent).rect;
        ((RectTransform)m_YearGrid.parent).SetSizeWithCurrentAnchors(Axis.Vertical, 40 * m_YearGrid.childCount + 10);
        rect.position = Vector2.zero;
    }

    private void UpdateDateUI()
    {
        m_YearTxt.text = year + "年" + month + "月";
        m_NextDayTxt.text = "今天:"+ System.DateTime.Now.Year + "年" + System.DateTime.Now.Month + "月" + System.DateTime.Now.Day + "日";

        DateTime firstDay = System.DateTime.Now;

        if (month != firstDay.Month)
            firstDay = firstDay.AddMonths(month - firstDay.Month);
        if (year != firstDay.Year)
            firstDay = firstDay.AddYears(year - firstDay.Year);

        //获得当前天数
        firstDay = firstDay.AddDays(1 - firstDay.Day);
        //本月第一天是周几
        int weekday = (int)firstDay.DayOfWeek;

        Debug.Log(weekday);
        for (int i = 0; i < weekday; i++)
        {
            dateItems[i].SetHide(true);
        }
        int monthDay = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
        Debug.Log(monthDay);
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
            if (addPlanNotePanel != null && selectDateTime != null)
            {
                addPlanNotePanel.SetEndDate(selectDateTime);
            }
            UIManager.Instance.PopPanel();
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

    private void YearChanged(Vector2 inVec)
    {
        if(Mathf.Abs(inVec.y) >= 1)
            if (yearTweener != null)
            {
                yearTweener.Kill();
                yearTweener = null;
            }
        UpdateYearHighLight();
    }
    private void MonthChanged(Vector2 inVec)
    {
        if (Mathf.Abs(inVec.y) >= 1)
            if (monthTweener != null)
            {
                monthTweener.Kill();
                monthTweener = null;
            }
        UpdateMonthHighLight();
    }
    private void UpdateYearHighLight()
    {
        for (int i = 0; i < yearItems.Length; i++)
        {
            yearItems[i].SetHighLight(false);
        }

        int count = (int)(m_YearGrid.parent.localPosition.y / 40);
        if (m_YearGrid.parent.localPosition.y % 40 > 20)
        {
            if (yearItems.Length > count + 1 && count + 1 >= 0)
            {
                yearItems[count + 1].SetHighLight(true);
                year = yearItems[count + 1].value;
            }
            else
            {
                yearItems[count].SetHighLight(true);
                year = yearItems[count].value;
            }
        }
        else
        {
            if (yearItems.Length > count && count >= 0)
            {
                yearItems[count].SetHighLight(true);
                year = yearItems[count].value;
            }
            else
            {
                yearItems[count + 1].SetHighLight(true);
                year = yearItems[count + 1].value;
            }
        }
    }
    private void UpdateMonthHighLight()
    {
        for (int i = 0; i < monthItems.Length; i++)
        {
            monthItems[i].SetHighLight(false);
        }

        int count = (int)(m_MonthGrid.parent.localPosition.y / 40);
        if (m_MonthGrid.parent.localPosition.y % 40 > 20)
        {
            if (monthItems.Length > count + 1 && count + 1 >= 0)
            {
                monthItems[count + 1].SetHighLight(true);
                month = monthItems[count + 1].value;
            }
            else
            {
                monthItems[count].SetHighLight(true);
                month = monthItems[count].value;
            }
        }
        else
        {
            if (monthItems.Length > count && count >= 0)
            {
                monthItems[count].SetHighLight(true);
                month = monthItems[count].value;
            }
            else
            {
                monthItems[count + 1].SetHighLight(true);
                month = monthItems[count + 1].value;
            }
        }
    }

    public void YearEndDrag()
    {
        m_YearScroll.StopMovement();
        int count = (int)(m_YearGrid.parent.localPosition.y / 40);

        if (yearTweener != null)
            yearTweener.Kill();

        if (m_YearGrid.parent.localPosition.y % 40 > 20)
        {
            yearTweener = m_YearGrid.parent.DOLocalMoveY((count + 1) * 40, 0.3f);
        }
        else
        {
            yearTweener = m_YearGrid.parent.DOLocalMoveY(count * 40, 0.3f);
        }
        UpdateYearHighLight();
    }
    public void MonthEndDrag()
    {
        int count = (int)(m_MonthGrid.parent.localPosition.y / 40);
        m_MonthScroll.StopMovement();

        if (monthTweener != null)
            monthTweener.Kill();

        if (m_MonthGrid.parent.localPosition.y % 40 > 20)
        {
            monthTweener = m_MonthGrid.parent.DOLocalMoveY((count + 1) * 40, 0.2f);
        }
        else
        {
            monthTweener = m_MonthGrid.parent.DOLocalMoveY(count * 40, 0.2f);
        }
        UpdateMonthHighLight();
    }
}