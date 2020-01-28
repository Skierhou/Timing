using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EScrollType
{
    YearSelect,
    MonthSelect,
    HourSelect,
    MinSelect,
};

public class MyScrollRect : ScrollRect
{
    private SelectTimePanel timePanel;
    private EScrollType scrollType;

    private Coroutine m_Coroutine;

    public void Initialize(SelectTimePanel inTimePanel, EScrollType inScrollType)
    {
        timePanel = inTimePanel;
        scrollType = inScrollType;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if(m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
        if (timePanel != null)
        {
            switch (scrollType)
            {
                case EScrollType.YearSelect:
                    timePanel.YearEndDrag(false);
                    break;
                case EScrollType.MonthSelect:
                    timePanel.MonthEndDrag(false);
                    break;
                case EScrollType.HourSelect:
                    timePanel.HourEndDrag(false);
                    break;
                case EScrollType.MinSelect:
                    timePanel.MinEndDrag(false);
                    break;
                default:
                    break;
            }
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
        m_Coroutine = StartCoroutine(EndDragDelay());
    }

    IEnumerator EndDragDelay()
    {
        yield return new WaitForSeconds(0.2f);
        if (timePanel != null)
        {
            switch (scrollType)
            {
                case EScrollType.YearSelect:
                    timePanel.YearEndDrag();
                    break;
                case EScrollType.MonthSelect:
                    timePanel.MonthEndDrag();
                    break;
                case EScrollType.HourSelect:
                    timePanel.HourEndDrag();
                    break;
                case EScrollType.MinSelect:
                    timePanel.MinEndDrag();
                    break;
                default:
                    break;
            }
        }
    }
}
