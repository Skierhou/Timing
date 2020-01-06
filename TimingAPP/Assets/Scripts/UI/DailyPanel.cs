using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class DailyPanel : BasePanel
{
    private const string DailyNoteGoPath = "DailyNote";
    private const float ItemHeight = 100;

    private Transform m_Grid;
    private Button m_AddBtn;

    private TypeData typeData;

    private void Awake()
    {
        m_Grid = transform.Find("ScrollView/Viewport/Content/Grid");
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
        foreach (Transform child in m_Grid)
        {
            if (child != m_Grid)
                GameObject.Destroy(child.gameObject);
        }

        List<DailyNote> noteList = DailyNoteManager.Instance.GetNotes(typeData.typeId);
        if (noteList != null)
        {
            for (int i = 0; i < noteList.Count; i++)
            {
                DailyNoteItem dailyItem = GameObject.Instantiate(Resources.Load<GameObject>(DailyNoteGoPath), m_Grid).GetComponent<DailyNoteItem>();
                dailyItem.Initialize(noteList[i]);
            }

            //刷新grid大小
            Rect rect = ((RectTransform)m_Grid.parent).rect;
            ((RectTransform)m_Grid.parent).SetSizeWithCurrentAnchors(Axis.Vertical, ItemHeight * (noteList.Count + 1));
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
