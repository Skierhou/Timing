using System;
using System.Collections;
using System.Collections.Generic;
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
    private Dictionary<int, string> m_TypeNameDict = new Dictionary<int, string>();
    //记录当前typeId
    private int typeId;

    public override void Initialize()
    {
        //读取之前所有的存储数据
        ReadData();
    }

    private void ReadData()
    {
        List<WealthNote> notes = new List<WealthNote>();
        for (int i = 0; i < notes.Count; i++)
        {
            AddNote(notes[i]);
        }
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

    public void ChangeNote(int inId, DateTime inDateTime, string inContent, float inMoney, int inPayTypeId, string inPayTypeName, Color inColor)
    {
        List<WealthNote> list;
        if (m_NoteDict.TryGetValue(inPayTypeId, out list) && list != null)
        {
            WealthNote tNote = list.Find((note) => note.Id == inId);
            tNote.Change(inDateTime, inContent, inMoney, inPayTypeId, inPayTypeName, inColor);
        }
    }

    public bool AddType(string inName)
    {
        if (string.IsNullOrEmpty(inName)) return false;

        foreach (string name in m_TypeNameDict.Values)
        {
            if (string.Equals(name, inName))
                return false;
        }
        m_TypeNameDict.Add(typeId++, inName);
        return true;
    }
    public List<TypeData> GetWealthTypes()
    {
        List<TypeData> dataList = new List<TypeData>();
        for (int i = m_TypeSequence.Count - 1; i > 0; i--)
        {
            string name = "";
            if (m_TypeNameDict.TryGetValue(m_TypeSequence[i], out name))
            {
                dataList.Add(new TypeData { name = name, typeId = m_TypeSequence[i] });
            }
        }
        return dataList;
    }
}
