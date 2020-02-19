using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainPanel : BasePanel
{
    public Color m_HighLightColor;
    public Color m_LowLightColor;

    public EOperateType curPanelType;
    private InputField m_SearchIF;
    private Button m_DailyNoteBtn;
    private Button m_WealthNoteBtn;
    private Button m_PlanNoteBtn;
    private Button m_SettingBtn;

    private Image m_DailyImg;
    private Image m_WealthImg;
    private Image m_PlanImg;

    List<string> curStringList = new List<string>();

    private void Awake()
    {
        m_SearchIF = transform.Find("Head/Search").GetComponent<InputField>();
        m_DailyNoteBtn = transform.Find("Head/DailyNoteBtn").GetComponent<Button>();
        m_WealthNoteBtn = transform.Find("Head/WealthNoteBtn").GetComponent<Button>();
        m_PlanNoteBtn = transform.Find("Head/PlanNoteBtn").GetComponent<Button>();
        m_SettingBtn = transform.Find("Head/SettingBtn").GetComponent<Button>();

        m_DailyImg = m_DailyNoteBtn.GetComponent<Image>();
        m_WealthImg = m_WealthNoteBtn.GetComponent<Image>();
        m_PlanImg = m_PlanNoteBtn.GetComponent<Image>();

        m_SearchIF.onValueChanged.AddListener(SearchValueChange);
        m_DailyNoteBtn.onClick.AddListener(DailyNoteBtnClick);
        m_WealthNoteBtn.onClick.AddListener(WealthNoteBtnClick);
        m_PlanNoteBtn.onClick.AddListener(PlanNoteBtnClick);
        m_SettingBtn.onClick.AddListener(OnSettingBtnClick);
        DailyNoteBtnClick();
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);
        Invoke("DailyNoteBtnClick",0.01f);
    }

    private void SearchValueChange(string inValue)
    {
        if (string.IsNullOrEmpty(inValue)) return;

        List<string> searchList = curStringList.FindAll((s)=>s.Contains(inValue));
        //创建搜索目标的Item
    }

    private void DailyNoteBtnClick()
    {
        ChangeColor(EOperateType.Daily);
        UIManager.Instance.PushPanel(EPanelType.TypePanel, EOperateType.Daily);

        StoreData();
    }
    private void WealthNoteBtnClick()
    {
        ChangeColor(EOperateType.Wealth);
        UIManager.Instance.PushPanel(EPanelType.WealthPanel,new WealthData {
            bSelectByType = true,
            id = -1
        });

        StoreData();
    }
    private void PlanNoteBtnClick()
    {
        ChangeColor(EOperateType.Plan);
        UIManager.Instance.PushPanel(EPanelType.PlanPanel);

        StoreData();
    }

    private void ChangeColor(EOperateType inPanelType)
    {
        switch (inPanelType)
        {
            case EOperateType.Daily:
                m_DailyImg.color = m_HighLightColor;
                m_WealthImg.color = m_LowLightColor;
                m_PlanImg.color = m_LowLightColor;
                break;
            case EOperateType.Wealth:
                m_DailyImg.color = m_LowLightColor;
                m_WealthImg.color = m_HighLightColor;
                m_PlanImg.color = m_LowLightColor;
                break;
            case EOperateType.Plan:
                m_DailyImg.color = m_LowLightColor;
                m_WealthImg.color = m_LowLightColor;
                m_PlanImg.color = m_HighLightColor;
                break;
        }
    }

    private void StoreData()
    {
        DailyNoteManager.Instance.StoreData();
        PlanManager.Instance.StoreData();
        WealthManager.Instance.StoreData();
    }

    private void OnSettingBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.SettingPanel);
    }
}
