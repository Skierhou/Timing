using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public struct TypeData
{
    public int typeId;
    public string name;
}

public class TypePanel : BasePanel
{
    private const float TypeItemHeight = 40;

    public EOperateType curPanelType;

    private GameObject typeItemTemplate;

    public Transform grid;
    public Button addBtn;

    private List<GameObject> m_TypeGoList = new List<GameObject>();

    private void Awake()
    {
        grid = transform.Find("ScrollView/Viewport/Content/Grid");
        addBtn = transform.Find("AddBtn").GetComponent<Button>();
        typeItemTemplate = Resources.Load<GameObject>("TypeItem");

        addBtn.onClick.AddListener(AddBtnClick);
    }

    public override void OnPop()
    {
        base.OnPop();
    }
    public override void OnPush(object inPara)
    {
        if (inPara != null)
        {
            curPanelType = (EOperateType)inPara;
            UpdateUI(curPanelType);
        }
    }

    public override void OnResume()
    {
        UpdateUI(curPanelType);
    }

    public void UpdateUI(EOperateType inPanelType)
    {
        curPanelType = inPanelType;
        GameObject go = null;
        List<TypeData> dataList = new List<TypeData>();

        foreach (Transform child in grid)
        {
            if (child != grid)
                GameObject.Destroy(child.gameObject);
        }
        m_TypeGoList.Clear();
        switch (inPanelType)
        {
            case EOperateType.Daily:
                dataList = DailyNoteManager.Instance.GetDailyTypes();
                for (int i = 0; i < dataList.Count; i++)
                {
                    go = GameObject.Instantiate(typeItemTemplate, grid);
                    go.GetComponent<TypeItem>().Initialize(dataList[i]); 
                    m_TypeGoList.Add(go);
                }
                break;
            case EOperateType.Wealth:
                dataList = WealthManager.Instance.GetWealthTypes();
                for (int i = 0; i < dataList.Count; i++)
                {
                    go = GameObject.Instantiate(typeItemTemplate, grid);
                    go.GetComponent<TypeItem>().Initialize(dataList[i]);
                    m_TypeGoList.Add(go);
                }
                break;
            case EOperateType.Plan:
                break;
        }

        //刷新grid大小
        Rect rect = ((RectTransform)grid.parent).rect;
        ((RectTransform)grid.parent).SetSizeWithCurrentAnchors(Axis.Vertical, TypeItemHeight * (dataList.Count + 1));
        rect.position = Vector2.zero;
    }

    public void AddBtnClick()
    {
        //开启添加Type的UI
        UIManager.Instance.PushPanel(EPanelType.AddTypePanel, curPanelType);
    }
}
