using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WealthTypeItem : MonoBehaviour
{
    public Color HighLightColor;
    public Color LowLightColor;

    private Image m_Img;
    private Button m_DeleteBtn;
    private Text m_Txt;

    private TypeData typeData;

    private WealthTypePanel typePanel;

    private void Awake()
    {
        m_Img = GetComponent<Image>();
        m_DeleteBtn = GetComponentInChildren<Button>();
        m_Txt = GetComponentInChildren<Text>();

        m_DeleteBtn.onClick.AddListener(OnDeleteBtnClick);
    }

    public void Initialize(TypeData inTypeData, WealthTypePanel inPanel)
    {
        typeData = inTypeData;
        typePanel = inPanel;

        m_Txt.text = typeData.name;
        m_Txt.color = typeData.color;
    }
    public void SetHighLight(bool inEnable)
    {
        if (inEnable)
        {
            m_Img.color = HighLightColor;
        }
        else
        {
            m_Img.color = LowLightColor;
        }
    }

    private void OnDeleteBtnClick()
    {
        WealthManager.Instance.RemoveType(typeData.typeId);
        typePanel.UpdateUI();
    }

}
