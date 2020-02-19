using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEngine.RectTransform;

public class AccountPanel : BasePanel
{
    private Dropdown m_TypeSelect;
    private MyScrollRect m_ScrollRect;
    private GridLayoutGroup m_Grid;
    private Button m_AddBtn;
    private Button m_SignBtn;
    private Text m_SignTxt;
    private InputField m_MoneyInput;
    private InputField m_DesInput;

    private Action deleteCallBack;

    private void Awake()
    {
        m_TypeSelect = GetComponentInChildren<Dropdown>();
        m_ScrollRect = GetComponentInChildren<MyScrollRect>();
        m_Grid = transform.Find("Select/Viewport/Content/Grid").GetComponent<GridLayoutGroup>();
        m_AddBtn = transform.Find("AddBtn").GetComponent<Button>();
        m_MoneyInput = transform.Find("MoneyInput").GetComponent<InputField>();
        m_DesInput = transform.Find("DesInput").GetComponent<InputField>();
        m_SignBtn = transform.Find("MoneyInput/SignBtn").GetComponent<Button>();
        m_SignTxt = transform.Find("MoneyInput/SignBtn/Text").GetComponent<Text>();

        m_AddBtn.onClick.AddListener(OnAddBtnClick);
        m_SignBtn.onClick.AddListener(OnSignBtnClick);
        m_MoneyInput.onValueChanged.AddListener(OnMoneyInputChanged);

        deleteCallBack = OnDeleteCallBack;
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        if (WealthManager.Instance.GetTypeCount() == 0)
        {
            WealthManager.Instance.AddType("默认",Color.white);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (Transform child in m_Grid.transform)
        {
            if (child != m_Grid.transform)
                GameObject.Destroy(child.gameObject);
        }

        m_TypeSelect.options.Clear();
        List<TypeData> typeDataList = WealthManager.Instance.GetWealthTypes();
        for (int i = 0; i < typeDataList.Count; i++)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData { text = typeDataList[i].name };
            m_TypeSelect.options.Add(optionData);
        }
        if (typeDataList.Count > 0)
            m_TypeSelect.captionText.text = typeDataList[0].name;

        List<WealthNote> noteList = WealthManager.Instance.GetWealthNotesByType();

        int length = 3;
        for (int i = 0; i < length; i++)
        {
            GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_Grid.transform);
        }
        for (int i = 0; i < noteList.Count; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("AccountItem"), m_Grid.transform);
            go.GetComponent<WealthNoteItem>().Initialize(noteList[i],deleteCallBack);
        }
        length = noteList.Count == 1 ? 3 : 2;
        for (int i = 0; i < 2; i++)
        {
            GameObject.Instantiate(Resources.Load<GameObject>("DateSelectItem"), m_Grid.transform);
        }

        //刷新grid大小
        float height = (m_Grid.cellSize.y + m_Grid.spacing.y) * (noteList.Count + 5) + m_Grid.padding.top + m_Grid.padding.bottom;
        Rect rect = ((RectTransform)m_Grid.transform.parent).rect;
        ((RectTransform)m_Grid.transform.parent).SetSizeWithCurrentAnchors(Axis.Vertical, height);
        rect.position = Vector2.zero;
    }

    public override void OnResume()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    private void OnAddBtnClick()
    {
        if (m_TypeSelect.options.Count <= 0)
        {
            Tools.MakeToast("请先添加账单类型!");
            return;
        }
        if (!string.IsNullOrEmpty(m_MoneyInput.text) && Tools.IsNumeric(m_MoneyInput.text))
        {
            string typeName = m_TypeSelect.captionText.text;
            TypeData typeData = WealthManager.Instance.GetTypeByName(typeName);

            int sign = m_SignTxt.text == "+" ? 1 : -1;

            WealthManager.Instance.AddNote(DateTime.Now, m_DesInput.text, float.Parse(m_MoneyInput.text) * sign, typeData.typeId, typeData.name, typeData.color);
            UpdateUI();

            m_MoneyInput.text = "";
            m_DesInput.text = "";
        }
        else
        {
            Tools.MakeToast("金钱不能为空!");
        }
    }
    private void OnMoneyInputChanged(string inValue)
    {
        //安全校验
        if (!Tools.IsNumeric(inValue))
        {
            if (inValue.Length > 1)
                m_MoneyInput.text = inValue.Substring(0, inValue.Length - 1);
            else
                m_MoneyInput.text = "";
        }
    }
    private void OnDeleteCallBack()
    {
        UpdateUI();
    }

    private void OnSignBtnClick()
    {
        if (m_SignTxt.text == "+")
            m_SignTxt.text = "-";
        else
            m_SignTxt.text = "+";
    }
}
