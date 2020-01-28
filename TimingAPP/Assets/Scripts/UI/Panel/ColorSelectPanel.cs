using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ColorData
{
    public Action<Color> callback;
    public Color color;
};

public class ColorSelectPanel : BasePanel
{
    private Button m_Btn;
    private Slider m_RedSlider;
    private Slider m_GreenSlider;
    private Slider m_BlueSlider;
    private Slider m_AlphaSlider;
    private Text m_RedValueTxt;
    private Text m_GreenValueTxt;
    private Text m_BlueValueTxt;
    private Text m_AlphaValueTxt;
    private Image m_RedImg;
    private Image m_GreenImg;
    private Image m_BlueImg;
    private Image m_AlphaImg;
    private Text m_PreviewTxt;
    private Image m_PreviewImg;

    private ColorData colorData;

    private void Awake()
    {
        m_Btn = GetComponent<Button>();
        m_RedSlider = transform.Find("Bg/RedSlider").GetComponent<Slider>();
        m_GreenSlider = transform.Find("Bg/GreenSlider").GetComponent<Slider>();
        m_BlueSlider = transform.Find("Bg/BlueSlider").GetComponent<Slider>();
        m_AlphaSlider = transform.Find("Bg/AlphaSlider").GetComponent<Slider>();
        m_RedValueTxt = m_RedSlider.transform.Find("ValueTxt").GetComponent<Text>();
        m_GreenValueTxt = m_GreenSlider.transform.Find("ValueTxt").GetComponent<Text>();
        m_BlueValueTxt = m_BlueSlider.transform.Find("ValueTxt").GetComponent<Text>();
        m_AlphaValueTxt = m_AlphaSlider.transform.Find("ValueTxt").GetComponent<Text>();
        m_RedImg = m_RedSlider.transform.Find("Background").GetComponent<Image>();
        m_GreenImg = m_GreenSlider.transform.Find("Background").GetComponent<Image>();
        m_BlueImg = m_BlueSlider.transform.Find("Background").GetComponent<Image>();
        m_AlphaImg = m_AlphaSlider.transform.Find("Background").GetComponent<Image>();
        m_PreviewTxt = transform.Find("Bg/PreviewTxt").GetComponent<Text>();
        m_PreviewImg = transform.Find("Bg/PreviewImg").GetComponent<Image>();

        m_RedSlider.onValueChanged.AddListener(OnRedSliderChanged);
        m_GreenSlider.onValueChanged.AddListener(OnGreenSliderChanged);
        m_BlueSlider.onValueChanged.AddListener(OnBlueSliderChanged);
        m_AlphaSlider.onValueChanged.AddListener(OnAlphaSliderChanged);
        m_Btn.onClick.AddListener(OnBtnClick);
    }

    public override void OnPush(object inPara)
    {
        gameObject.SetActive(true);

        if (inPara != null)
            colorData = (ColorData)inPara;

        if (colorData.color != null)
        {
            m_PreviewImg.color = colorData.color;
            m_PreviewTxt.color = colorData.color;
            m_RedSlider.value = colorData.color.r;
            m_BlueSlider.value = colorData.color.b;
            m_GreenSlider.value = colorData.color.g;
            m_AlphaSlider.value = colorData.color.a;
        }
        else
        {
            m_RedSlider.value = 0;
            m_GreenSlider.value = 0;
            m_BlueSlider.value = 0;
            m_AlphaSlider.value = 1;
        }
    }
    public override void OnResume()
    {
        gameObject.SetActive(true);
    }
    public override void OnPop()
    {
        gameObject.SetActive(false);
    }

    private Color GetColor()
    {
        return new Color(m_RedSlider.value, m_GreenSlider.value, m_BlueSlider.value, m_AlphaSlider.value);
    }

    private void OnBtnClick()
    {
        colorData.callback?.Invoke(GetColor());
        UIManager.Instance.PopPanel();
    }

    private void UpdateUI()
    {
        Color color = GetColor();
        m_PreviewTxt.color = color;
        m_PreviewImg.color = color;
        colorData.callback?.Invoke(color);
    }

    private void OnRedSliderChanged(float inValue)
    {
        UpdateUI();
        int red = (int)(m_RedSlider.value * 255);
        m_RedValueTxt.text = red.ToString();
        m_RedImg.color = new Color(m_RedSlider.value, 0, 0, 1);
    }
    private void OnGreenSliderChanged(float inValue)
    {
        UpdateUI();
        m_GreenValueTxt.text = ((int)(inValue * 255)).ToString();
        m_GreenImg.color = new Color(0, inValue, 0, 1);
    }
    private void OnBlueSliderChanged(float inValue)
    {
        UpdateUI();
        m_BlueValueTxt.text = ((int)(inValue * 255)).ToString();
        m_BlueImg.color = new Color(0, 0, inValue, 1);
    }
    private void OnAlphaSliderChanged(float inValue)
    {
        UpdateUI();
        m_AlphaValueTxt.text = ((int)(inValue * 255)).ToString();
        m_AlphaImg.color = new Color(0, 0, 0, inValue);
    }
}
