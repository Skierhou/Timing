using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AccountLineItem : MonoBehaviour
{
    public float Max = 140;

    private Text m_TimeTxt;
    private Text m_MoneyTxt;
    private RectTransform m_LineRect;

    private WealthNote wealthNote;

    private void Awake()
    {
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_MoneyTxt = transform.Find("MoneyTxt").GetComponent<Text>();
        m_LineRect = transform.Find("Line").GetComponent<RectTransform>();
    }

    public void Initialize(WealthNote inNote,float inWidth)
    {
        wealthNote = inNote;

        if (wealthNote != null)
        {
            m_MoneyTxt.text = Tools.CheckMoney(wealthNote.Money);
            m_TimeTxt.text = wealthNote.Date.Day + "日\n" + Tools.GetTimeStringMin(wealthNote.Date);

            inWidth = Mathf.Min(inWidth, Max);

            m_LineRect.transform.localScale = new Vector3(1, 0.5f, 1);
            m_LineRect.transform.DOScaleX(inWidth, 2);

            m_MoneyTxt.transform.localPosition = new Vector3(0, -65, 0);
            m_MoneyTxt.transform.DOLocalMoveY(-73 + inWidth + 8, 2);
        }
    }
}
