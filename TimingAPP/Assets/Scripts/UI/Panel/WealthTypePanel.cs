using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class WealthTypePanel : BasePanel
{
    private MyScrollRect m_ScrollRect;
    private GridLayoutGroup m_Grid;
    private InputField m_Input;
    private Button m_AddBtn;
    private Button m_ColorBtn;
    private Image m_ColorImg;
    private Action<Color> m_ColorCallBack;

    private void Awake()
    {
        m_ScrollRect = GetComponentInChildren<MyScrollRect>();
        m_Grid = GetComponentInChildren<GridLayoutGroup>();
        m_Input = GetComponentInChildren<InputField>();
        m_AddBtn = transform.Find("AddBtn").GetComponent<Button>();
        m_ColorBtn = transform.Find("ColorBtn").GetComponent<Button>();
        m_ColorImg = transform.Find("ColorBtn/ColorImg").GetComponent<Image>();

        m_AddBtn.onClick.AddListener(OnAddBtnClick);
        m_ColorBtn.onClick.AddListener(OnColorBtnClick);
        m_ColorCallBack = OnColorCallBack;
    }


    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }
        List<TypeData> typeDataList = WealthManager.Instance.GetWealthTypes();
        if (typeDataList != null && typeDataList.Count >= 1)
        {
            int length = 4;
            for (int i = 0; i < length; i++)
            {
                GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_Grid.transform);
            }
            for (int i = 0; i < typeDataList.Count; i++)
            {
                GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("WealthTypeItem"), m_Grid.transform);
                go.GetComponent<WealthTypeItem>().Initialize(typeDataList[i], this);
            }
            length = typeDataList.Count == 1 ? 4 : 3;
            for (int i = 0; i < length; i++)
            {
                GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_Grid.transform);
            }

            //刷新grid大小
            float height = (m_Grid.cellSize.y + m_Grid.spacing.y) * (typeDataList.Count + 7) + m_Grid.padding.top + m_Grid.padding.bottom + 3;
            Rect rect = ((RectTransform)m_Grid.transform.parent).rect;
            ((RectTransform)m_Grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
            rect.position = Vector2.zero;
        }
    }

    public override void OnPop()
    {
        gameObject.SetActive(false);
    }
    public override void OnResume()
    {
        gameObject.SetActive(true);
    }

    private void OnAddBtnClick()
    {
        if (string.IsNullOrEmpty(m_Input.text) || string.IsNullOrEmpty(m_Input.text.Trim()))
        {
            //TODO：吐司提示
        }
        else
        {
            WealthManager.Instance.AddType(m_Input.text.Trim(), m_ColorImg.color);
            m_Input.text = "";

            UpdateUI();
        }
    }
    private void OnColorBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.ColorSelectPanel, new ColorData { callback = m_ColorCallBack, color = m_ColorImg.color });
    }
    public void OnColorCallBack(Color inColor)
    {
        m_ColorImg.color = inColor;
    }
}
