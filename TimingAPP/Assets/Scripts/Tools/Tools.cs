using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Tools
{
    private static string[] OrderNumList = new string[] {"0","①","②","③","④","⑤","⑥","⑦","⑧","⑨","⑩" };

    public static string GetNowTimeString()
    {
        return DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;
    }

    public static string GetTimeString(DateTime inDateTime)
    {
        return inDateTime.Year + "-" + inDateTime.Month + "-" + inDateTime.Day + " " + inDateTime.Hour + ":" + inDateTime.Minute;
    }
    public static string GetTimeStringDay(DateTime inDateTime)
    {
        return inDateTime.Year + "-" + SuppleTime(inDateTime.Month) + "-" + SuppleTime(inDateTime.Day);
    }
    public static string GetTimeStringMin(DateTime inDateTime)
    {
        return SuppleTime(inDateTime.Hour) + ":" + SuppleTime(inDateTime.Minute);
    }

    public static string SuppleTime(int inValue)
    {
        return inValue / 10 == 0 ? "0" + inValue : inValue.ToString();
    }

    public static string GetOrderNum(int inId)
    {
        return OrderNumList[inId];
    }

    public static DateTime GetTime(string inStr)
    {
        string[] strs = inStr.Split(' ');
        if (strs.Length == 2)
        {
            string[] tempStrs1 = strs[0].Split('-');
            string[] tempStrs2 = strs[1].Split(':');
            return new DateTime(int.Parse(tempStrs1[0]), int.Parse(tempStrs1[1]), int.Parse(tempStrs1[2]), int.Parse(tempStrs2[0]), int.Parse(tempStrs2[1]), 0);
        }
        return System.DateTime.Now;
    }
    /// <summary>
    /// 是否是数字
    /// </summary>
    public static bool IsNumeric(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
    }
}