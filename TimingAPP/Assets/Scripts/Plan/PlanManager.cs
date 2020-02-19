using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanManager : Singleton<PlanManager>
{
    private List<PlanNote> m_PlanList = new List<PlanNote>();
    int planId;

    public List<PlanNote> PlanList { get => m_PlanList; }

    public void StoreData()
    {
        PlayerPrefs.SetInt("PlanCount", m_PlanList.Count);
        for (int i = 0; i < m_PlanList.Count; i++)
        {
            //DateTime,IsFinish,Title,Content,TimeType,Timer,TimerCount,Interval
            string str = string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}", m_PlanList[i].DateStr, m_PlanList[i].IsFinish, m_PlanList[i].Title
                , m_PlanList[i].Content, (int)m_PlanList[i].TimeType, Tools.GetTimeString(m_PlanList[i].Timer), m_PlanList[i].TimerCount, m_PlanList[i].Interval);
            PlayerPrefs.SetString("PlanNote_" + i, str);
        }
    }

    public void ReadData()
    {
        int count = PlayerPrefs.GetInt("PlanCount");
        for (int i = 0; i < count; i++)
        {
            string[] strs = PlayerPrefs.GetString("PlanNote_" + i).Split('#');
            if (strs.Length == 8)
            {
                AddNote(strs[2],strs[3],Tools.GetTime(strs[0]), (ETimeType)int.Parse(strs[4]),Tools.GetTime(strs[5]),int.Parse(strs[6]),int.Parse(strs[7]),Color.white,bool.Parse(strs[1]));
            }
        }
    }

    public override void Initialize()
    {
        planId = 0;
        m_PlanList.Clear();
        ReadData();
    }

    public void AddNote(string inTitle, string inContent, DateTime inDateTime, ETimeType inTimeType, DateTime inTimer, int inTimerCount, int inInterval, Color inColor, bool inFinish = false)
    {
        PlanNote note = new PlanNote(planId++, inTitle ,inContent, inDateTime, inTimeType, inTimer, inTimerCount, inInterval, inColor, inFinish);
        m_PlanList.Add(note);
    }

    public bool RemoveNote(PlanNote inNote)
    {
        return m_PlanList.Remove(inNote);
    }
}
