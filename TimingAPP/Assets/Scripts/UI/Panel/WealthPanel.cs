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
    private Button m_ImgBtn;
    private Button m_TimeBtn;
    private Text m_TimeTxt;
    private Dropdown m_TypeSelect;

    private WealthData wealthData;
    private DateTime curDate;

    private Action<List<WealthNote>> selectTimeCallBack;

    private void Awake()
    {
        m_Grid = transform.Find("ScrollView/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
        m_CenterBtn = transform.Find("BtnBg/CenterBtn").GetComponent<Toggle>();
        m_AccountBtn = transform.Find("BtnBg/AccountBtn").GetComponent<Toggle>();
        m_SelectBtn = transform.Find("BtnBg/SelectBtn").GetComponent<Toggle>();
        m_GotoAddBtn = transform.Find("GotoAddBtn").GetComponent<Button>();
        m_ImgBtn = transform.Find("ImgBtn").GetComponent<Button>();
        m_TimeBtn = transform.Find("TimeBtn").GetComponent<Button>();
        m_TimeTxt = transform.Find("TimeBtn/TimeTxt").GetComponent<Text>();
        m_TypeSelect = transform.Find("TypeSelect").GetComponent<Dropdown>();

        m_CenterBtn.onValueChanged.AddListener(OnCenterChanged);
        m_AccountBtn.onValueChanged.AddListener(OnAccountChanged);
        m_SelectBtn.onValueChanged.AddListener(OnSelectChanged);
        m_GotoAddBtn.onClick.AddListener(OnGotoAddBtnClick);
        m_ImgBtn.onClick.AddListener(OnImgBtnClick);
        m_TimeBtn.onClick.AddListener(OnTimeBtnClick);
        m_TypeSelect.onValueChanged.AddListener(OnTypeSelectChanged);

        selectTimeCallBack = UpdateNoteList;
    }

    public override void OnPush(object inPara)
    {
        if (inPara != null)
        {
            wealthData = (WealthData)inPara;
        }
        gameObject.SetActive(true);

        UpdateType();
        UpdateUI(System.DateTime.Now);
    }
    private void UpdateType()
    {
        m_TypeSelect.options.Clear();
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        optionDatas.Add(new Dropdown.OptionData("全部类型"));
        List<TypeData> typeDatas = WealthManager.Instance.GetWealthTypes();
        for (int i = 0; i < typeDatas.Count; i++)
        {
            optionDatas.Add(new Dropdown.OptionData(typeDatas[i].name));
        }
        m_TypeSelect.AddOptions(optionDatas);
        m_TypeSelect.value = 0;
        m_TypeSelect.captionText.text = m_TypeSelect.options[0].text;
    }

    #region 暂时舍弃
    private void UpdateUI()
    {
        List<WealthNote> notes = new List<WealthNote>();

        if (wealthData.bSelectByType)
        {
            notes.AddRange(WealthManager.Instance.GetWealthNotesByType(wealthData.id));
        }
        else
        {
            notes.AddRange(WealthManager.Instance.GetWealthNotesByType());
            notes.RemoveAll((note) => note.Date.Ticks > wealthData.startTicks && note.Date.Ticks <= wealthData.endTicks);
        }
        UpdateNoteList(notes);
    }
    public void UpdateNoteList(List<WealthNote> inNotes)
    {
        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        if (inNotes.Count == 0)
            m_GotoAddBtn.gameObject.SetActive(true);
        else
            m_GotoAddBtn.gameObject.SetActive(false);

        for (int i = 0; i < inNotes.Count; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("WealthNoteItem"), m_Grid.transform);
            go.GetComponent<WealthNoteItem>().Initialize(inNotes[i]);
        }

        float height = m_Grid.cellSize.y * inNotes.Count + m_Grid.padding.top + m_Grid.padding.bottom + m_Grid.spacing.y;
        Rect rect = ((RectTransform)m_Grid.transform.parent).rect;
        ((RectTransform)m_Grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
        rect.position = Vector2.zero;
    }
    #endregion

    private void UpdateUI(DateTime inDate)
    {
        curDate = inDate;
        m_TimeTxt.text = inDate.Year + "-" + Tools.SuppleTime(inDate.Month);

        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        List<WealthNote> notes = new List<WealthNote>();
        if (m_TypeSelect.value == 0)
        {
            notes.AddRange(WealthManager.Instance.GetWealthNotesByType());
        }
        else
        {
            TypeData typeData = WealthManager.Instance.GetTypeByName(m_TypeSelect.captionText.text);
            notes.AddRange(WealthManager.Instance.GetWealthNotesByType(typeData.typeId));
        }

        int day = DateTime.DaysInMonth(inDate.Year, inDate.Month);
        DateTime startDate = new DateTime(inDate.Year, inDate.Month, 1);
        DateTime endDate = new DateTime(inDate.Year, inDate.Month, day);

        notes.RemoveAll((note) => { return note.Date.Ticks < startDate.Ticks || note.Date.Ticks > endDate.Ticks; });
        m_GotoAddBtn.gameObject.SetActive(notes.Count == 0);

        if (notes.Count > 0)
        {
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("WealthNoteItem"), m_Grid.transform);
                go.GetComponent<WealthNoteItem>().Initialize(notes[i]);
            }

            float height = (m_Grid.cellSize.y + m_Grid.spacing.y) * notes.Count + m_Grid.padding.top + m_Grid.padding.bottom;
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
            UpdateUI();
        }
    }
    private void OnSelectChanged(bool inEnable)
    {
        if (inEnable)
        {
            UIManager.Instance.PushPanel(EPanelType.WealthLimitPanel, selectTimeCallBack);
        }
    }
    private void OnGotoAddBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AccountPanel);
    }
    private void OnImgBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.WealhtLinePanel);
    }

    private void OnTimeBtnClick()
    {
        int startYear = 0;
        int endYear = 0;

        List<WealthNote> noteList = null;
        noteList = WealthManager.Instance.GetWealthNotesByType();
        if (noteList.Count > 0)
        {
            startYear = noteList[noteList.Count - 1].Date.Year - DateTime.Now.Year;
        }
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel,new SelectTimeData { callback=UpdateUI,startYear= startYear, endYear= endYear, bCantShowDaySelect=true});
    }
    private void OnTypeSelectChanged(int inValue)
    {
        UpdateUI(curDate);
    }
}
