using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.RectTransform;
using System;

public struct WealthData
{
    public bool bSelectByType;  //是否使用type查看
    public int id;
    public string name;

    public long startTicks;
    public long endTicks;
};

public class WealthPanel : BasePanel
{
    private GridLayoutGroup m_Grid;
    private Toggle m_CenterBtn;
    private Toggle m_AccountBtn;
    private Toggle m_SelectBtn;

    private Button m_GotoAddBtn;

    private WealthData wealthData;

    private bool bShowImgAccount;

    private void Awake()
    {
        m_Grid = transform.Find("ScrollView/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
        m_CenterBtn = transform.Find("BtnBg/CenterBtn").GetComponent<Toggle>();
        m_AccountBtn = transform.Find("BtnBg/AccountBtn").GetComponent<Toggle>();
        m_SelectBtn = transform.Find("BtnBg/SelectBtn").GetComponent<Toggle>();
        m_GotoAddBtn = transform.Find("GotoAddBtn").GetComponent<Button>();

        m_CenterBtn.onValueChanged.AddListener(OnCenterChanged);
        m_AccountBtn.onValueChanged.AddListener(OnAccountChanged);
        m_SelectBtn.onValueChanged.AddListener(OnSelectChanged);
        m_GotoAddBtn.onClick.AddListener(OnGotoAddBtnClick);
    }

    public override void OnPush(object inPara)
    {
        if (inPara != null)
        {
            wealthData = (WealthData)inPara;
        }
        gameObject.SetActive(true);

        UpdateUI();
    }

    private void UpdateUI()
    {
        List<WealthNote> notes;

        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        if (wealthData.bSelectByType)
        {
            notes = WealthManager.Instance.GetWealthNotesByType(wealthData.id);
        }
        else
        {
            notes = WealthManager.Instance.GetWealthNotesByType();
            notes.RemoveAll((note) => note.Date.Ticks > wealthData.startTicks && note.Date.Ticks <= wealthData.endTicks);
        }
        if (notes.Count == 0)
            m_GotoAddBtn.gameObject.SetActive(true);
        else
            m_GotoAddBtn.gameObject.SetActive(false);

        for (int i = 0; i < notes.Count; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("WealthNoteItem"),m_Grid.transform);
            go.GetComponent<WealthNoteItem>().Initialize(notes[i]);
        }

        float height = m_Grid.cellSize.y * notes.Count + m_Grid.padding.top + m_Grid.padding.bottom + m_Grid.spacing.y;
        Rect rect = ((RectTransform)m_Grid.transform.parent).rect;
        ((RectTransform)m_Grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
        rect.position = Vector2.zero;
    }

    public override void OnPop()
    {
        gameObject.SetActive(false);
    }
    public override void OnResume()
    {
        gameObject.SetActive(true);
    }

    private void OnCenterChanged(bool inEnable)
    {
        if (inEnable)
        {
            UIManager.Instance.PushPanel(EPanelType.WealthCenterPanel);
        }
    }
    private void OnAccountChanged(bool inEnable)
    {
        if (inEnable)
        {
            while (UIManager.Instance.GetCurPanelType() != (EPanelType.WealthPanel | EPanelType.MainPanel))
                UIManager.Instance.PopPanel();
            if (!bShowImgAccount)
            {
                //显示正常账单
                UpdateUI();
            }
            else
            {
                //显示线性图片
            }
            bShowImgAccount = !bShowImgAccount;
        }
        else
        {
            bShowImgAccount = false;
        }
    }
    private void OnSelectChanged(bool inEnable)
    {
        if (inEnable)
        {
            
        }
    }
    private void OnGotoAddBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AccountPanel);
    }
}
