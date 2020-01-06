using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyScrollRect : ScrollRect
{
    private SelectTimePanel timePanel;
    private bool bYear;
    private Coroutine m_Coroutine;

    public void Initialize(SelectTimePanel inTimePanel, bool inYear)
    {
        timePanel = inTimePanel;
        bYear = inYear;
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
            if (bYear)
                timePanel.YearEndDrag();
            else
                timePanel.MonthEndDrag();
        }
    }
}
