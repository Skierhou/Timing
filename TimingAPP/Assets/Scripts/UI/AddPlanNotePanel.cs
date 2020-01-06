using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class AddPlanNotePanel : BasePanel
{
    private const float ItemHeight = 115;

    private InputField m_TitleInput;
    private Button m_EndTimeBtn;
    private Text m_EndTimeTxt;
    private Button m_SureBtn;
    private Button m_CancelBtn;
    private Button m_AddTaskBtn;
    private Transform m_TaskGrid;

    private PlanNote planNote;

    private DateTime endDateTime;

    private List<TaskItem> taskItemList = new List<TaskItem>();
    private int taskId;

    private void Awake()
    {
        m_TitleInput = transform.Find("Bg/TitleInput").GetComponent<InputField>();
        m_EndTimeBtn = transform.Find("Bg/SelectTimeBtn").GetComponent<Button>();
        m_EndTimeTxt = m_EndTimeBtn.transform.Find("Text").GetComponent<Text>();
        m_SureBtn = transform.Find("Bg/SureBtn").GetComponent<Button>();
        m_CancelBtn = transform.Find("Bg/CancelBtn").GetComponent<Button>();
        m_AddTaskBtn = transform.Find("Bg/AddTaskBtn").GetComponent<Button>();
        m_TaskGrid = transform.Find("Bg/ScrollView/Viewport/Content/Grid");

        m_EndTimeBtn.onClick.AddListener(EndTimeBtnClick);
        m_SureBtn.onClick.AddListener(SureBtnClick);
        m_CancelBtn.onClick.AddListener(CancelBtnClick);
        m_AddTaskBtn.onClick.AddListener(AddTaskBtnClick);
    }

    public override void OnPush(object inPara)
    {
        if (inPara != null)
            planNote = (PlanNote)inPara;

        gameObject.SetActive(true);

        m_TitleInput.text = "";
        m_EndTimeTxt.text = "";
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (Transform child in m_TaskGrid)
        {
            if (child != m_TaskGrid)
                GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < taskItemList.Count; i++)
        {
            GameObject.Destroy(taskItemList[i].gameObject,0.1f);
        }
        taskItemList.Clear();

        if (planNote != null)
        {
            m_TitleInput.text = planNote.Title;
            m_EndTimeTxt.text = Tools.GetTimeString(planNote.Timer);
            for (int i = 0; i < planNote.Tasks.Count; i++)
            {
                TaskItem taskItem = GameObject.Instantiate(Resources.Load<GameObject>("TaskItem"), m_TaskGrid).GetComponent<TaskItem>();
                taskItem.Inialize(new TaskData { id = i + 1, content = planNote.Tasks[i] }, this);
                taskItemList.Add(taskItem);
            }
        }

        //刷新grid大小
        Rect rect = ((RectTransform)m_TaskGrid.parent).rect;
        ((RectTransform)m_TaskGrid.parent).SetSizeWithCurrentAnchors(Axis.Vertical, ItemHeight * (taskItemList.Count + 1));
        rect.position = Vector2.zero;

        taskId = taskItemList.Count + 1;
    }
    public override void OnResume()
    {
        UpdateUI();
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    public void RemoveTask(TaskItem inTask)
    {
        taskItemList.Remove(inTask);
        for (int i = 0; i < taskItemList.Count; i++)
        {
            taskItemList[i].ChangeId(i + 1);
        }
        GameObject.Destroy(inTask.gameObject);

        taskId = taskItemList.Count + 1;
    }

    private void EndTimeBtnClick()
    {
        //选择日期
        UIManager.Instance.PushPanel(EPanelType.SelectTimePanel, this);
    }
    private void SureBtnClick()
    {
        //字符串转换成Date时间 2019-10-02 18:37
        string[] strs = m_EndTimeTxt.text.Split(' ');
        DateTime endDate = endDateTime == null ? System.DateTime.Now : endDateTime;
        //if (strs.Length == 2)
        //{
        //    string[] tempStrs1 = strs[0].Split('-');
        //    string[] tempStrs2 = strs[1].Split(':');
        //    endDate = new DateTime(int.Parse(tempStrs1[0]), int.Parse(tempStrs1[1]), int.Parse(tempStrs1[2]), int.Parse(tempStrs2[0]), int.Parse(tempStrs2[1]), 0);
        //}
        string content = "";
        for (int i = 0; i < taskItemList.Count; i++)
        {
            content += taskItemList[i].taskData.content;
            if (i != taskItemList.Count - 1)
                content += ",";
        }
        if (planNote == null)
        {
            PlanManager.Instance.AddNote(m_TitleInput.text, content, System.DateTime.Now, ETimeType.EveryDay, endDate, 0, 0, Color.white);
        }
        else
        {
            planNote.Change(m_TitleInput.text, content, System.DateTime.Now, ETimeType.EveryDay, endDate, 0, 0, Color.white, planNote.IsFinish);
        }
        UIManager.Instance.PopPanel();
    }
    private void CancelBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
    private void AddTaskBtnClick()
    {
        TaskItem taskItem = GameObject.Instantiate(Resources.Load<GameObject>("TaskItem"),m_TaskGrid).GetComponent<TaskItem>();
        taskItem.Inialize(new TaskData { id = taskId++, content = "" }, this);
        taskItemList.Add(taskItem);

        //刷新grid大小
        Rect rect = ((RectTransform)m_TaskGrid.parent).rect;
        ((RectTransform)m_TaskGrid.parent).SetSizeWithCurrentAnchors(Axis.Vertical, ItemHeight * (taskItemList.Count + 1));
        rect.position = Vector2.zero;
    }

    public void SetEndDate(DateTime inDateTime)
    {
        Debug.Log(inDateTime.Year + " " + inDateTime.Month + " " + inDateTime.Day);
        endDateTime = inDateTime;
        if(planNote != null)
            planNote.Timer = inDateTime;
        m_EndTimeTxt.text = Tools.GetTimeString(inDateTime);
    }
}
