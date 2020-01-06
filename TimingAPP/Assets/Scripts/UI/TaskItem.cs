using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TaskData
{
    public int id;
    public string content;
}

public class TaskItem : MonoBehaviour
{
    private Button m_CancelBtn;
    private Text m_IdTxt;
    private InputField m_ContentInput;

    public TaskData taskData;
    private AddPlanNotePanel panel;

    private float timer;

    private void Awake()
    {
        m_CancelBtn = transform.Find("CancelBtn").GetComponent<Button>();
        m_IdTxt = transform.Find("IdTxt").GetComponent<Text>();
        m_ContentInput = transform.Find("ContentInput").GetComponent<InputField>();

        m_CancelBtn.onClick.AddListener(CancelBtnClick);
        m_ContentInput.onValueChanged.AddListener(ContentInputChanged);
    }

    public void Inialize(TaskData inData,AddPlanNotePanel inPanel)
    {
        taskData = inData;
        panel = inPanel;

        Invoke("UpdateUI",0.01f);
    }

    public void ChangeId(int id)
    {
        taskData.id = id;
        m_IdTxt.text = Tools.GetOrderNum(id);
    }

    private void UpdateUI()
    {
        m_IdTxt.text = Tools.GetOrderNum(taskData.id);
        m_ContentInput.text = taskData.content;
    }

    private void ContentInputChanged(string inValue)
    {
        taskData.content = inValue;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    private void CancelBtnClick()
    {
        if (timer <= 0)
            timer = 1;
        else
            panel.RemoveTask(this);
    }
}
