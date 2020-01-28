using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthNoteItem : MonoBehaviour
{
    public Color IncomeColor;
    public Color OutcomeColor;

    private Text m_TimeTxt;
    private Text m_TypeTxt;
    private Text m_IncomeTxt;
    private Button m_DeleteBtn;

    public bool IsAccountCenter; //是否账户管理

    private System.Action callback;

    private WealthNote wealthNote;

    private void Awake()
    {
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_TypeTxt = transform.Find("TypeTxt").GetComponent<Text>();
        m_IncomeTxt = transform.Find("IncomeTxt").GetComponent<Text>();
        if (IsAccountCenter)
        {
            m_DeleteBtn = transform.Find("DeleteBtn").GetComponent<Button>();
            m_DeleteBtn.onClick.AddListener(OnDeleteBtnClick);
        }
    }

    public void Initialize(WealthNote inNote,System.Action inCallBack = null)
    {
        wealthNote = inNote;
        callback = inCallBack;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (wealthNote != null)
        {
            m_TimeTxt.text = wealthNote.DateStr;
            m_TypeTxt.text = wealthNote.PayTypeName;
            m_TypeTxt.color = wealthNote.Color;
            CheckMoney();
        }
    }

    private void CheckMoney()
    {
        if (wealthNote.Money >= 0)
        {
            m_IncomeTxt.text = "+" + wealthNote.Money;
            m_IncomeTxt.color = IncomeColor;
        }
        else
        {
            m_IncomeTxt.text = wealthNote.Money.ToString();
            m_IncomeTxt.color = OutcomeColor;
        }
    }

    private void OnDeleteBtnClick()
    {
        WealthManager.Instance.RemoveNote(wealthNote.PayTypeId,wealthNote.Id);
        callback?.Invoke();
    }
}
