using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyNotePanel : BasePanel
{
    private DailyNote m_DailyNote;

    private Button m_ReturnBtn;
    private Button m_SettingBtn;
    private Text m_TimeTxt;
    private InputField m_TitleInput;
    private InputField m_ContentInput;

    private void Awake()
    {
        m_ReturnBtn = transform.Find("ReturnBtn").GetComponent<Button>();
        m_SettingBtn = transform.Find("SettingBtn").GetComponent<Button>();
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_TitleInput = transform.Find("TitleInput").GetComponent<InputField>();
        m_ContentInput = transform.Find("ScrollView/Viewport/Content/ContentInput").GetComponent<InputField>();

        m_ReturnBtn.onClick.AddListener(ReturnBtnClick);
        m_SettingBtn.onClick.AddListener(SettingBtnClick);
    }

    public override void OnPush(object inPara)
    {
        if(inPara != null)
            m_DailyNote = (DailyNote)inPara;
        Invoke("UpdateUI", 0.01f);
        gameObject.SetActive(true);
    }
    public override void OnResume()
    {
        UpdateUI();
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        m_TimeTxt.text = string.Format("{0}年{1}月{2}日 {3}:{4} | {5}字", m_DailyNote.Date.Year, m_DailyNote.Date.Month
            , m_DailyNote.Date.Day, m_DailyNote.Date.Hour, m_DailyNote.Date.Minute,m_DailyNote.Content.Length);
        m_TitleInput.text = m_DailyNote.Title;
        m_ContentInput.text = m_DailyNote.Content;
    }

    private void ReturnBtnClick()
    {
        m_DailyNote.Change(m_ContentInput.text, m_TitleInput.text, System.DateTime.Now, m_DailyNote.TypeName, m_DailyNote.TypeId, m_DailyNote.Color);
        UIManager.Instance.PopPanel();
    }
    private void SettingBtnClick()
    {

    }

    
}
