using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WealthManager : Singleton<WealthManager>
{
    //Key:typeId,Value:typeId下的所有Note
    private Dictionary<int, List<WealthNote>> m_NoteDict = new Dictionary<int, List<WealthNote>>();
    //每个typeId顺序
    private List<int> m_TypeSequence = new List<int>();
    //Key:typeId,Value:每个type下的NoteId
    private Dictionary<int, List<int>> m_NoteSequence = new Dictionary<int, List<int>>();
    //Key:typeId,Value:noteId
    private Dictionary<int, int> m_NoteIdDict = new Dictionary<int, int>();
    //Key:typeId,Value:typeName
    private Dictionary<int, TypeData> m_TypeDataDict = new Dictionary<int, TypeData>();
    //记录当前typeId
    private int typeId;

    public override void Initialize()
    {
        //读取之前所有的存储数据
        m_NoteDict.Clear();
        m_TypeSequence.Clear();
        m_NoteSequence.Clear();
        m_NoteIdDict.Clear();
        m_TypeDataDict.Clear();
        typeId = 0;
        ReadData();
    }
    public void StoreData()
    {
        int i = 0;
        PlayerPrefs.SetInt("WealthTypeCount", m_TypeDataDict.Count);
        foreach (int key in m_TypeDataDict.Keys)
        {
            PlayerPrefs.SetString("WealthTypeName_" + i++, m_TypeDataDict[key].name + ","
                + m_TypeDataDict[key].color.r + "-" + m_TypeDataDict[key].color.g + "-" + m_TypeDataDict[key].color.b + "-" + m_TypeDataDict[key].color.a);
        }

        i = 0;
        foreach (int key in m_NoteDict.Keys)
        {
            if (m_NoteDict[key] != null)
            {
                PlayerPrefs.SetInt("WealthTypeId_" + i, m_NoteDict[key].Count);
                for (int j = 0; j < m_NoteDict[key].Count; j++)
                {
                    WealthNote note = m_NoteDict[key][j];
                    string str = string.Format("{0},{1},{2}", note.DataStrForStroe, note.Content,note.Money);
                    PlayerPrefs.SetString("WealthNote_" + i + "_" + j, str);
                }
            }
            i++;
        }
    }

    private void ReadData()
    {
        //添加类型
        if (PlayerPrefs.HasKey("WealthTypeCount"))
        {
            int index = PlayerPrefs.GetInt("WealthTypeCount");
            for (int i = 0; i < index; i++)
            {
                string str = PlayerPrefs.GetString("WealthTypeName_" + i);
                string[] strs = str.Split(',');
                string[] colorStrs = strs[1].Split('-');
                TypeData typeData = new TypeData
                {
                    typeId = i,
                    name = strs[0],
                    color = new Color(float.Parse(colorStrs[0]), float.Parse(colorStrs[1]), float.Parse(colorStrs[2]), float.Parse(colorStrs[3]))
                };
                AddType(typeData.name,typeData.color);
            }
        }

        //添加记录
        foreach (int key in m_TypeDataDict.Keys)
        {
            int index = PlayerPrefs.GetInt("WealthTypeId_" + key);
            for (int i = 0; i < index; i++)
            {
                string[] strs = PlayerPrefs.GetString("WealthNote_" + key + "_" + i).Split(',');
                if (strs.Length == 3)
                {
                    AddNote(Tools.GetTime(strs[0]), strs[1], float.Parse(strs[2]), key, m_TypeDataDict[key].name, m_TypeDataDict[key].color);
                }
            }
        }
    }

    public List<WealthNote> GetWealthNotesByType(int inType = -1)
    {
        List<WealthNote> notes = new List<WealthNote>();
        if (m_NoteDict.ContainsKey(inType))
        {
            notes.AddRange(m_NoteDict[inType]);
        }
        else
        {
            foreach (List<WealthNote> tempNotes in m_NoteDict.Values)
            {
                notes.AddRange(tempNotes);
            }
        }
        notes = notes.OrderByDescending(note => note.Date.Ticks).ToList<WealthNote>();
        return notes;
    }

    public bool RemoveNote(int inTypeId, int inId)
    {
        if (m_NoteDict.ContainsKey(inTypeId))
        {
            foreach (WealthNote tempNote in m_NoteDict[inTypeId])
            {
                if (tempNote.Id == inId)
                    return m_NoteDict[inTypeId].Remove(tempNote);
            }
        }
        return false;
    }
    public void AddNote(WealthNote inNote)
    {
        AddNote(inNote.Date, inNote.Content, inNote.Money, inNote.PayTypeId, inNote.PayTypeName, inNote.Color);
    }


    public void AddNote(DateTime inDateTime, string inContent, float inMoney, int inPayTypeId, string inPayTypeName, Color inColor)
    {
        int id = 0;
        if (!m_NoteIdDict.TryGetValue(inPayTypeId, out id))
        {
            m_NoteIdDict.Add(inPayTypeId, id);
        }
        ++m_NoteIdDict[inPayTypeId];

        if (!m_TypeSequence.Contains(inPayTypeId))
        {
            m_TypeSequence.Add(inPayTypeId);
        }
        WealthNote note = new WealthNote(id, inDateTime, inContent, inMoney, inPayTypeId, inPayTypeName, inColor);

        List<WealthNote> list;
        if (!m_NoteDict.TryGetValue(inPayTypeId, out list) || list == null)
        {
            list = new List<WealthNote>();
            m_NoteDict.Add(inPayTypeId, list);
        }
        list.Add(note);

        List<int> sequenceList;
        if (!m_NoteSequence.TryGetValue(inPayTypeId, out sequenceList) || sequenceList == null)
        {
            sequenceList = new List<int>();
            m_NoteSequence.Add(inPayTypeId, sequenceList);
        }
        sequenceList.Add(id);
    }
    public TypeData GetTypeByName(string typeName)
    {
        foreach (TypeData data in m_TypeDataDict.Values)
        {
            if (data.name == typeName)
                return data;
        }
        Debug.LogError("没找到对应类型：" + typeName);
        return new TypeData();
    }

    public bool AddType(string inName,Color inColor)
    {
        if (string.IsNullOrEmpty(inName)) return false;

        foreach (TypeData data in m_TypeDataDict.Values)
        {
            if (string.Equals(data.name, inName))
                return false;
        }
        m_TypeSequence.Add(typeId);
        m_TypeDataDict.Add(typeId, new TypeData{typeId = typeId,name = inName,color = inColor});
        typeId++;
        return true;
    }
    public bool RemoveType(int inId)
    {
        return m_TypeDataDict.Remove(inId);
    }

    public List<TypeData> GetWealthTypes()
    {
        List<TypeData> dataList = new List<TypeData>();
        for (int i = m_TypeSequence.Count - 1; i >= 0; i--)
        {
            TypeData typedata;
            if (m_TypeDataDict.TryGetValue(m_TypeSequence[i], out typedata))
            {
                dataList.Add(new TypeData { name = typedata.name, typeId = typedata.typeId, color = typedata.color });
            }
        }
        return dataList;
    }

    public int GetTypeCount()
    {
        return m_TypeSequence.Count;
    }
}
