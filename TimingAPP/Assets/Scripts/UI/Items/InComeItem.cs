using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InComeItem : MonoBehaviour
{
    public Color IncomeColor;
    public Color OutcomeColor;

    private Text m_TimeTxt;
    private Text m_MoneyTxt;

    private void Awake()
    {
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_MoneyTxt = transform.Find("MoneyTxt").GetComponent<Text>();
    }

    public void UpdateUI(DateTime inDate,float inMoney)
    {
        gameObject.SetActive(true);

        m_TimeTxt.text = Tools.GetTimeStringDay(inDate) + "\n" + Tools.GetTimeStringMin(inDate);
        if (inMoney >= 0)
        {
            m_MoneyTxt.text = "+" + inMoney.ToString("f1");
            m_MoneyTxt.color = IncomeColor;
        }
        else
        {
            m_MoneyTxt.text = inMoney.ToString("f1");
            m_MoneyTxt.color = OutcomeColor;
        }

        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3(0, -50, 0);
        Tools.FlyTo(m_MoneyTxt);
        Tools.FlyTo(m_TimeTxt);

        Invoke("DestroySelf", 4);
    }
    private void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }
}
