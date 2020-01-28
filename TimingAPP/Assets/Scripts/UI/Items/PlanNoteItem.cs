using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FinishType
{
    Finish,
    OutTime,
    WaitFinish,
    OutTimeFinish,
}

public class PlanNoteItem : MonoBehaviour
{
    public Color FinishColor;
    public Color OutTimeColor;
    public Color WaitFinishColor;
    public Color OutTimeFinishColor;

    private Text m_TitleTxt;
    private Text m_TimeTxt;
    private Text m_StateTxt;
    private Text m_StartTimeTxt;
    private Button m_Btn;
    private Toggle m_FinishToggle;

    private PlanNote planNote;

    private void Awake()
    {
        m_TitleTxt = transform.Find("TitleTxt").GetComponent<Text>();
        m_TimeTxt = transform.Find("TimeTxt").GetComponent<Text>();
        m_StateTxt = transform.Find("StateTxt").GetComponent<Text>();
        m_StartTimeTxt = transform.Find("StartTimeTxt").GetComponent<Text>();
        m_FinishToggle = transform.Find("Toggle").GetComponent<Toggle>();
        m_Btn = GetComponent<Button>();

        m_FinishToggle.onValueChanged.AddListener(FinishToggleClick);
        m_Btn.onClick.AddListener(BtnClick);
    }

    public void Initialize(PlanNote inNote)
    {
        planNote = inNote;

        Invoke("UpdateUI",0.01f);
    }

    private void UpdateUI()
    {
        m_TitleTxt.text = "标题:"+planNote.Title;
        m_StartTimeTxt.text = "开始时间:" + Tools.GetTimeStringDay(planNote.Date) + " "+Tools.GetTimeStringMin(planNote.Date);
        m_TimeTxt.text = "结束时间:" + Tools.GetTimeStringDay(planNote.Timer) + " " + Tools.GetTimeStringMin(planNote.Timer);
        FinishToggleClick(planNote.IsFinish);
        m_FinishToggle.isOn = planNote.IsFinish;
    }

    private void BtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AddPlanNotePanel, planNote);
    }

    private void FinishToggleClick(bool inEnable)
    {
        planNote.IsFinish = inEnable;
        m_StateTxt.text = "状态:";
        if (inEnable)
        {
            if (planNote.Timer >= System.DateTime.Now)
            {
                m_StateTxt.text += "已完成";
                m_StateTxt.color = FinishColor;
            }
            else
            {
                m_StateTxt.text += "超时完成";
                m_StateTxt.color = OutTimeFinishColor;
            }
        }
        else
        {
            if (planNote.Timer >= System.DateTime.Now)
            {
                m_StateTxt.text += "未完成";
                m_StateTxt.color = WaitFinishColor;
            }
            else
            {
                m_StateTxt.text += "超时!";
                m_StateTxt.color = OutTimeColor;
            }
        }
    }
}
