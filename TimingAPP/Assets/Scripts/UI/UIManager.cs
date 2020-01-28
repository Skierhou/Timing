using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EOperateType
{
    Daily,
    Wealth,
    Plan,
}

public enum EPanelType
{
    None,
    MainPanel,
    TypePanel,
    DailyPanel,
    DailyNotePanel,
    PlanPanel,
    AddTypePanel,
    AddDailyNotePanel,
    AddPlanNotePanel,
    PlanNotePanel,
    SelectTimePanel,
    WealthPanel,
    WealthCenterPanel,
    WealthTypePanel,
    ColorSelectPanel,
    AccountPanel,
}

public class UIManager:SingletonMono<UIManager>
{
    public EOperateType curOperateType;

    private Dictionary<EPanelType, BasePanel> m_PanelDict = new Dictionary<EPanelType, BasePanel>();
    private Dictionary<EPanelType, string> m_PanelPathDict = new Dictionary<EPanelType, string>();

    private Stack<BasePanel> m_PanelStack = new Stack<BasePanel>();

    private Transform m_Canvas;

    private const float OrginalScreenWidth = 288;
    private const float OrginalScreenHeight = 512;

    private Vector3 panelScale;

    private void Awake()
    {
        m_PanelPathDict.Add(EPanelType.MainPanel, "UI/MainPanel");
        m_PanelPathDict.Add(EPanelType.TypePanel, "UI/TypePanel");
        m_PanelPathDict.Add(EPanelType.DailyPanel, "UI/DailyPanel");
        m_PanelPathDict.Add(EPanelType.DailyNotePanel, "UI/DailyNotePanel");
        m_PanelPathDict.Add(EPanelType.PlanPanel, "UI/PlanPanel");
        m_PanelPathDict.Add(EPanelType.AddTypePanel, "UI/AddTypePanel");
        m_PanelPathDict.Add(EPanelType.AddDailyNotePanel, "UI/AddDailyNotePanel");
        m_PanelPathDict.Add(EPanelType.AddPlanNotePanel, "UI/AddPlanNotePanel");
        m_PanelPathDict.Add(EPanelType.PlanNotePanel, "UI/PlanNotePanel");
        m_PanelPathDict.Add(EPanelType.SelectTimePanel, "UI/SelectTimePanel");
        m_PanelPathDict.Add(EPanelType.WealthPanel, "UI/WealthPanel");
        m_PanelPathDict.Add(EPanelType.WealthCenterPanel, "UI/WealthCenterPanel");
        m_PanelPathDict.Add(EPanelType.WealthTypePanel, "UI/WealthTypePanel");
        m_PanelPathDict.Add(EPanelType.ColorSelectPanel, "UI/ColorSelectPanel");
        m_PanelPathDict.Add(EPanelType.AccountPanel, "UI/AccountPanel");

        panelScale = new Vector3(Screen.width * 1.0f / OrginalScreenWidth, Screen.height * 1.0f / OrginalScreenHeight, 1);

        m_Canvas = GameObject.Find("Canvas").transform;

        PushPanel(EPanelType.MainPanel);
    }

    public void PushPanel(EPanelType inPanelType,object inPara = null)
    {
        if (GetCurPanelType() == inPanelType)
            return;

        BasePanel panel;
        if (!m_PanelDict.TryGetValue(inPanelType, out panel) || panel == null)
        {
            panel = GameObject.Instantiate(Resources.Load<GameObject>(m_PanelPathDict[inPanelType]), m_Canvas).GetComponent<BasePanel>();
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = panelScale;
            panel.PanelType = inPanelType;
            m_PanelDict.Add(inPanelType, panel);
        }
        if (panel != null)
        {
            panel.transform.localScale = panelScale;
            panel.transform.localPosition = Vector3.zero;
            panel.OnPush(inPara);
            panel.transform.SetAsLastSibling();
            m_PanelStack.Push(panel);
        }
    }

    public void PopPanel()
    {
        if (m_PanelStack.Count >= 1)
        {
            m_PanelStack.Peek().OnPop();
            m_PanelStack.Pop();

            if (m_PanelStack.Count >= 1)
            {
                m_PanelStack.Peek().OnResume();
            }
        }
    }

    public EPanelType GetCurPanelType()
    {
        if (m_PanelStack.Count >= 1)
        {
            return m_PanelStack.Peek().PanelType;
        }
        return EPanelType.None;
    }
}
