using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETimeType
{
    EveryDay,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday,
}

public class PlanNote
{
    private int m_Id;
    private DateTime m_DateTime;
    private bool m_IsFinish;
    private string m_Title;
    private string m_Content;
    private ETimeType m_TimeType;
    private DateTime m_Timer;
    private int m_TimerCount;
    private int m_Interval;
    private Color m_Color;
    private List<string> m_Tasks;

    public int Id { get => m_Id; set => m_Id = value; }
    public string DateStr { get => Tools.GetTimeString(m_DateTime); }
    public DateTime Date { get => m_DateTime; set => m_DateTime = value; }
    public string Content { get => m_Content; set => m_Content = value; }
    public ETimeType TimeType { get => m_TimeType; set => m_TimeType = value; }
    public DateTime Timer { get => m_Timer; set => m_Timer = value; }
    public int TimerCount { get => m_TimerCount; set => m_TimerCount = value; }
    public int Interval { get => m_Interval; set => m_Interval = value; }
    public Color Color { get => m_Color; set => m_Color = value; }
    public string Title { get => m_Title; set => m_Title = value; }
    public bool IsFinish { get => m_IsFinish; set => m_IsFinish = value; }
    public List<string> Tasks { get => m_Tasks; set => m_Tasks = value; }

    public PlanNote(int inId,string inTitle, string inContent, DateTime inDateTime, ETimeType inTimeType, DateTime inTimer, int inTimerCount,int inInterval,Color inColor, bool inFinish)
    {
        m_Id = inId;
        Change(inTitle, inContent, inDateTime, inTimeType, inTimer, inTimerCount, inInterval, inColor, inFinish);
    }

    public void Change(string inTitle,string inContent, DateTime inDateTime, ETimeType inTimeType, DateTime inTimer, int inTimerCount, int inInterval, Color inColor, bool inFinish)
    {
        m_Title = inTitle;
        m_Content = inContent;
        m_DateTime = inDateTime;
        m_TimeType = inTimeType;
        m_Timer = inTimer;
        m_TimerCount = inTimerCount;
        m_Interval = inInterval;
        m_Color = inColor;
        m_IsFinish = inFinish;

        string[] strs = m_Content.Split(',');
        m_Tasks = new List<string>();
        for (int i = 0; i < strs.Length; i++)
        {
            m_Tasks.Add(strs[i]);
        }
    }
}
