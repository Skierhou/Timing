using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.RectTransform;

public class PlanPanel : BasePanel
{
    private const float ItemHeight = 90;

    private Button m_AddBtn;
    private GridLayoutGroup m_Grid;

    private void Awake()
    {
        m_AddBtn = transform.Find("AddBtn").GetComponent<Button>();
        m_Grid = transform.Find("ScrollView/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();

        m_AddBtn.onClick.AddListener(AddBtnClick);
    }

    public override void OnPush(object inPara)
    {
        UpdateUI();
        gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        List<PlanNote> noteList = PlanManager.Instance.PlanList.OrderByDescending((note) => note.Date).ToList();
        if (noteList != null)
        {
            for (int i = 0; i < noteList.Count; i++)
            {
                PlanNoteItem noteItem = GameObject.Instantiate(Resources.Load<GameObject>("PlanNoteItem"), m_Grid.transform).GetComponent<PlanNoteItem>();
                noteItem.Initialize(noteList[i]);
            }

            //刷新grid大小
            float height = m_Grid.cellSize.y * noteList.Count + m_Grid.padding.top + m_Grid.padding.bottom + m_Grid.spacing.y;
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
        UpdateUI();
    }

    private void AddBtnClick()
    {
        UIManager.Instance.PushPanel(EPanelType.AddPlanNotePanel, null);
    }
}
