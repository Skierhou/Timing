using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateSelectItem : MonoBehaviour
{
    public Color m_HighLightColor;
    public Color m_LowLightColor;

    private Text m_Txt;

    public int value = 0;

    public void Initialize(int inCount)
    {
        if(m_Txt == null)
            m_Txt = transform.Find("Text").GetComponent<Text>();

        value = inCount;
        m_Txt.text = Tools.SuppleTime(inCount);
    }

    public void SetHighLight(bool inEnable)
    {
        if (m_Txt == null)
            m_Txt = GetComponentInChildren<Text>();

        if (inEnable)
            m_Txt.color = m_HighLightColor;
        else
            m_Txt.color = m_LowLightColor;
    }
}
