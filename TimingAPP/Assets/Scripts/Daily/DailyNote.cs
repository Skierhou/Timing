using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyNote
{
    private int m_Id;
    private DateTime m_DateTime;
    private string m_Title;
    private string m_Content;
    private string m_TypeName;
    private int m_TypeId;
    private Color m_Color;

    public int Id { get => m_Id; set => m_Id = value; }
    public string DateStr { get => Tools.GetTimeString(m_DateTime);  }
    public DateTime Date { get => m_DateTime; set => m_DateTime = value; }
    public string Content { get => m_Content; set => m_Content = value; }
    public string TypeName { get => m_TypeName; set => m_TypeName = value; }
    public Color Color { get => m_Color; set => m_Color = value; }
    public int TypeId { get => m_TypeId; set => m_TypeId = value; }
    public string Title { get => m_Title; set => m_Title = value; }

    public DailyNote(int inId,string inTitle, string inContent,DateTime inDateTime , string inTypeName, int inTypeId, Color inColor)
    {
        m_Id = inId;
        Change(inContent, inTitle,inDateTime, inTypeName, inTypeId, inColor);
    }

    public void Change(string inContent, string inTitle, DateTime inDateTime, string inTypeName, int inTypeId, Color inColor)
    {
        m_Content = inContent;
        m_Title = inTitle;
        m_TypeName = inTypeName;
        m_TypeId = inTypeId;
        m_Color = inColor;
        m_DateTime = inDateTime;
    }
}
