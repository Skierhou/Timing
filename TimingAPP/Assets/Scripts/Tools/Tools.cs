using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using DG.Tweening;

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

    public static string CheckMoney(float inMoney)
    {
        string res = "";
        if (inMoney > 0)
            res = "<color=green> +" + inMoney.ToString("f1") + "</color>";
        else if (inMoney < 0)
            res = "<color=red> " + inMoney.ToString("f1") + "</color>";
        else
            res = "0";
        return res;
    }

    // Unity调用安卓的土司
    public static void MakeToast(string info)
    {
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            Toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, info, Toast.GetStatic<int>("LENGTH_LONG")).Call("show");
        }));
#endif
        /*
        // 匿名方法中第二个参数是安卓上下文对象，除了用currentActivity，还可用安卓中的GetApplicationContext()获得上下文。
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        */
    }

    public static void FlyTo(Graphic graphic)
    {
        RectTransform rt = graphic.rectTransform;
        Color c = graphic.color;
        c.a = 0;
        graphic.color = c;
        Sequence mySequence = DOTween.Sequence();
        Tweener move1 = rt.DOLocalMoveY(rt.localPosition.y + 50, 1f);
        Tweener move2 = rt.DOLocalMoveY(rt.localPosition.y + 100, 1f);
        Tweener alpha1 = graphic.DOColor(new Color(c.r, c.g, c.b, 1), 1f);
        Tweener alpha2 = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 1f);
        mySequence.Append(move1);
        mySequence.Join(alpha1);
        mySequence.AppendInterval(1);
        mySequence.Append(move2);
        mySequence.Join(alpha2);
    }
}