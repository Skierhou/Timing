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
}

public class UIManager:SingletonMono<UIManager>
{
    public EOperateType curPanelType;

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

        panelScale = new Vector3(Screen.width * 1.0f / OrginalScreenWidth, Screen.height * 1.0f / OrginalScreenHeight, 1);

        m_Canvas = GameObject.Find("Canvas").transform;

        PushPanel(EPanelType.MainPanel);
    }

    public void PushPanel(EPanelType inPanelType,object inPara = null)
    {
        BasePanel panel;
        if (!m_PanelDict.TryGetValue(inPanelType, out panel) || panel == null)
        {
            panel = GameObject.Instantiate(Resources.Load<GameObject>(m_PanelPathDict[inPanelType]), m_Canvas).GetComponent<BasePanel>();
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = panelScale;
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
}
