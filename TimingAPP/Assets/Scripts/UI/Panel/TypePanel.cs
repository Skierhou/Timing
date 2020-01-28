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
    public Color color;
}

public class TypePanel : BasePanel
{
    private const float TypeItemHeight = 40;

    public EOperateType curPanelType;

    private GameObject typeItemTemplate;

    public GridLayoutGroup grid;
    public Button addBtn;

    private List<GameObject> m_TypeGoList = new List<GameObject>();

    private void Awake()
    {
        grid = transform.Find("ScrollView/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
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

        foreach (Transform child in grid.transform)
        {
            if (child != grid.transform)
                GameObject.Destroy(child.gameObject);
        }
        m_TypeGoList.Clear();
        switch (inPanelType)
        {
            case EOperateType.Daily:
                dataList = DailyNoteManager.Instance.GetDailyTypes();
                for (int i = 0; i < dataList.Count; i++)
                {
                    go = GameObject.Instantiate(typeItemTemplate, grid.transform);
                    go.GetComponent<TypeItem>().Initialize(dataList[i]); 
                    m_TypeGoList.Add(go);
                }
                break;
            case EOperateType.Wealth:
                dataList = WealthManager.Instance.GetWealthTypes();
                for (int i = 0; i < dataList.Count; i++)
                {
                    go = GameObject.Instantiate(typeItemTemplate, grid.transform);
                    go.GetComponent<TypeItem>().Initialize(dataList[i]);
                    m_TypeGoList.Add(go);
                }
                break;
            case EOperateType.Plan:
                break;
        }

        //刷新grid大小
        float height = grid.cellSize.y * dataList.Count + grid.padding.top + grid.padding.bottom + grid.spacing.y;
        Rect rect = ((RectTransform)grid.transform.parent).rect;
        ((RectTransform)grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
        rect.position = Vector2.zero;
    }

    public void AddBtnClick()
    {
        //开启添加Type的UI
        UIManager.Instance.PushPanel(EPanelType.AddTypePanel, curPanelType);
    }
}
