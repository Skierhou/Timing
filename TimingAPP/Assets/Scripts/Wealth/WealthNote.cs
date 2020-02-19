using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WealthNote
{
    private int m_Id;
    private DateTime m_DateTime;
    private string m_Content;
    private float m_Money;
    private int m_PayTypeId;
    private string m_PayTypeName;
    private Color m_Color;

    public int Id { get => m_Id; set => m_Id = value; }
    public DateTime Date { get => m_DateTime; set => m_DateTime = value; }
    public string DateStr { get => Tools.GetTimeStringDay(m_DateTime) + "\n" + Tools.GetTimeStringMin(m_DateTime); }
    public string DataStrForStroe { get => Tools.GetTimeString(m_DateTime); }
    public string Content { get => m_Content; set => m_Content = value; }
    public float Money { get => m_Money; set => m_Money = value; }
    public int PayTypeId { get => m_PayTypeId; set => m_PayTypeId = value; }
    public string PayTypeName { get => m_PayTypeName; set => m_PayTypeName = value; }
    public Color Color { get => m_Color; set => m_Color = value; }
    public string ColorStr { get => Color.r + "-" + Color.g + "-" + Color.b + "-" + Color.a; }

    public WealthNote(int inId,DateTime inDateTime,string inContent,float inMoney,int inPayType,string inPayTypeName,Color inColor)
    {
        Id = inId;
        Change(inDateTime, inContent, inMoney, inPayType, inPayTypeName, inColor);
    }

    public void Change(DateTime inDateTime, string inContent, float inMoney, int inPayType, string inPayTypeName, Color inColor)
    {
        Date = inDateTime;
        Content = inContent;
        Money = inMoney;
        PayTypeId = inPayType;
        PayTypeName = inPayTypeName;
        Color = inColor;
    }
}
