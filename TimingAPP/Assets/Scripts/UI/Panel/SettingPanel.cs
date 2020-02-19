using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private Button m_DeleteBtn;
    private Button m_ReturnBtn;

    private float timer = 0;
    private bool bDelete = false;

    private void Awake()
    {
        m_DeleteBtn = transform.Find("DeleteAllBtn").GetComponent<Button>();
        m_ReturnBtn = transform.Find("ReturnBtn").GetComponent<Button>();

        m_DeleteBtn.onClick.AddListener(OnDeleteBtnClick);
        m_ReturnBtn.onClick.AddListener(OnReturnBtnClick);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (bDelete)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                bDelete = false;
            }
        }
    }

    private void OnDeleteBtnClick()
    {
        if (bDelete)
        {
            PlayerPrefs.DeleteAll();
            WealthManager.Instance.Initialize();
            PlanManager.Instance.Initialize();
            DailyNoteManager.Instance.Initialize();
            Tools.MakeToast("清空所有记录!");
        }
        else
        {
            timer = 1;
            bDelete = true;
            Tools.MakeToast("双击删除!");
        }
    }
    private void OnReturnBtnClick()
    {
        UIManager.Instance.PopPanel();
    }
}
