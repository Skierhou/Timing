using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyNoteItem : MonoBehaviour
{
    private Text m_TitleTxt;
    private Text m_ContentTxt;
    private Text m_TimeTxt;
    private Button m_Btn;

    private DailyNote dailyNote;

    private void Awake()
    {
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_ContentTxt = transform.Find("ContentTxt").GetComponent<Text>();
        m_TitleTxt = transform.Find("TitleTxt").GetComponent<Text>();
        m_Btn = GetComponent<Button>();

        m_Btn.onClick.AddListener(BtnClick);
    }

    public void Initialize(DailyNote inNote)
    {
        if (m_TimeTxt == null)
        {
            Awake();
        }
        dailyNote = inNote;

        m_TimeTxt.text = string.Format("{0}年{1}月{2}日 {3}:{4} | {5}字", inNote.Date.Year, inNote.Date.Month
            , inNote.Date.Day, inNote.Date.Hour, inNote.Date.Minute, inNote.Content.Length);
        m_TitleTxt.text = inNote.Title;
        m_ContentTxt.text = inNote.Content;
    }

    private void BtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.DailyNotePanel, dailyNote);
    }
}
