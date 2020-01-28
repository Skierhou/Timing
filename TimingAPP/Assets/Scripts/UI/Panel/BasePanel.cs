using UnityEngine;
using UnityEditor;

public class BasePanel : MonoBehaviour
{
    public EPanelType PanelType;
    public virtual void OnPush(object inPara)
    {

    }
    public virtual void OnResume()
    {

    }
    public virtual void OnPop()
    {

    }
}