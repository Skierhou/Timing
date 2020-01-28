using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class DailyPanel : BasePanel
{
    private const string DailyNoteGoPath = "DailyNote";
    private const float ItemHeight = 100;

    private GridLayoutGroup m_Grid;
    private Button m_AddBtn;

    private TypeData typeData;

    private void Awake()
    {
        m_Grid = transform.Find("ScrollView/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
        m_AddBtn = transform.Find("AddBtn").GetComponent<Button>();

        m_AddBtn.onClick.AddListener(AddBtnClick);
    }

    public override void OnPush(object inPara)
    {
        if (inPara != null)
            typeData = (TypeData)inPara;
        UpdateUI();
    }
    private void UpdateUI()
    {
        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        List<DailyNote> noteList = DailyNoteManager.Instance.GetNotes(typeData.typeId);
        if (noteList != null)
        {
            for (int i = 0; i < noteList.Count; i++)
            {
                DailyNoteItem dailyItem = GameObject.Instantiate(Resources.Load<GameObject>(DailyNoteGoPath), m_Grid.transform).GetComponent<DailyNoteItem>();
                dailyItem.Initialize(noteList[i]);
            }

            //刷新grid大小
            float height = m_Grid.cellSize.y * noteList.Count + m_Grid.padding.top + m_Grid.padding.bottom + m_Grid.spacing.y;
            Rect rect = ((RectTransform)m_Grid.transform.parent).rect;
            ((RectTransform)m_Grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
            rect.position = Vector2.zero;
        }
    }
    public override void OnResume()
    {
        UpdateUI();
    }
    public override void OnPop()
    {
    }

    private void AddBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AddDailyNotePanel, typeData);
    }
}
