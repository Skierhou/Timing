using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyNoteManager : Singleton<DailyNoteManager>
{
    //Key:typeId,Value:typeId下的所有Note
    private Dictionary<int, List<DailyNote>> m_NoteDict = new Dictionary<int, List<DailyNote>>();
    //每个typeId顺序
    private List<int> m_TypeSequence = new List<int>();
    //Key:typeId,Value:每个type下的NoteId
    private Dictionary<int, List<int>> m_NoteSequence = new Dictionary<int, List<int>>();
    //Key:typeId,Value:(用来记忆NoteId)
    private Dictionary<int, int> m_NoteIdDict = new Dictionary<int, int>();
    //Key:typeId,Value:TypeName
    private Dictionary<int, string> m_TypeNameDict = new Dictionary<int, string>();

    //用来记忆当前typeId
    private int typeId;

    public void StoreData()
    {
        int i = 0;
        PlayerPrefs.SetInt("DailyTypeCount",m_TypeNameDict.Count);
        foreach (int key in m_TypeNameDict.Keys)
        {
            PlayerPrefs.SetString("TypeName_" + i++, m_TypeNameDict[key]);
        }

        foreach (int key in m_NoteDict.Keys)
        {
            if (m_NoteDict[key] != null)
            {
                //"DateTime,Title,Content,TypeName,TypeId"
                PlayerPrefs.SetInt("TypeId_" + m_TypeNameDict[key], m_NoteDict[key].Count);
                for (i = 0; i < m_NoteDict[key].Count; i++)
                {
                    string str = string.Format("{0},{1},{2},{3},{4}", m_NoteDict[key][i].DateStr, m_NoteDict[key][i].Title
                        , m_NoteDict[key][i].Content, m_NoteDict[key][i].TypeName, m_NoteDict[key][i].TypeId);
                    PlayerPrefs.SetString("DailyNote_" + m_TypeNameDict[key] + "_" + i, str);
                }
            }
        }
    }

    public void ReadData()
    {
        List<DailyNote> noteList = new List<DailyNote>();
        if (PlayerPrefs.HasKey("DailyTypeCount"))
        {
            int index = PlayerPrefs.GetInt("DailyTypeCount");
            for (int i = 0; i < index; i++)
            {
                m_TypeNameDict.Add(i, PlayerPrefs.GetString("TypeName_" + i));
            }
        }
        foreach (int key in m_TypeNameDict.Keys)
        {
            int index = PlayerPrefs.GetInt("TypeId_" + m_TypeNameDict[key]);
            for (int i = 0; i < index; i++)
            {
                string[] strs = PlayerPrefs.GetString("DailyNote_" + m_TypeNameDict[key] + "_" + i).Split(',');
                if (strs.Length == 5)
                {
                    AddNote(strs[2], strs[1], Tools.GetTime(strs[0]), strs[3], int.Parse(strs[4]), Color.white);
                }
            }
        }
    }

    public List<DailyNote> GetNotes(int typeId)
    {
        List<DailyNote> resList = null;
        List<DailyNote> noteList;
        if (m_NoteDict.TryGetValue(typeId, out noteList) && noteList != null)
        {
            List<int> sequenceList;
            if (m_NoteSequence.TryGetValue(typeId, out sequenceList) && sequenceList != null)
            {
                resList = new List<DailyNote>();
                for (int i = 0; i < sequenceList.Count; i++)
                {
                    resList.Add(noteList[sequenceList[i]]);
                }
            }
        }
        return resList;
    }

    public override void Initialize()
    {
        //读取之前所有的存储数据
        ReadData();
        typeId = m_TypeNameDict.Count;
    }

    public void AddNote(DailyNote inNote)
    {
        AddNote(inNote.Content, inNote.Title,inNote.Date, inNote.TypeName, inNote.TypeId, inNote.Color);
    }
    public void AddNote(string inContent, string inTitle, DateTime inDateTime, string inTypeName, int inTypeId, Color inColor)
    {
        int id = 0;
        if (!m_NoteIdDict.TryGetValue(inTypeId, out id))
        {
            m_NoteIdDict.Add(inTypeId, id);
        }
        ++m_NoteIdDict[inTypeId];

        if (!m_TypeSequence.Contains(inTypeId))
        {
            m_TypeSequence.Add(inTypeId);
        }
        DailyNote note = new DailyNote(id,inTitle ,inContent, inDateTime, inTypeName, inTypeId, inColor);

        List<DailyNote> list;
        if (!m_NoteDict.TryGetValue(inTypeId, out list) || list == null)
        {
            list = new List<DailyNote>();
            m_NoteDict.Add(inTypeId, list);
        }
        list.Add(note);

        List<int> sequenceList;
        if (!m_NoteSequence.TryGetValue(inTypeId, out sequenceList) || sequenceList == null)
        {
            sequenceList = new List<int>();
            m_NoteSequence.Add(inTypeId, sequenceList);
        }
        sequenceList.Add(id);
    }

    public void ChangeNote(int inId, string inTitle, string inContent, DateTime inDateTime, string inTypeName, int inTypeId, Color inColor)
    {
        List<DailyNote> list;
        if (m_NoteDict.TryGetValue(inTypeId, out list) && list != null)
        {
            DailyNote tNote = list.Find((note) => note.Id == inId);
            tNote.Change(inContent, inTitle,inDateTime, inTypeName, inTypeId, inColor);
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
        m_TypeSequence.Add(typeId);
        m_TypeNameDict.Add(typeId, inName);
        typeId++;
        return true;
    }

    public List<TypeData> GetDailyTypes()
    {
        List<TypeData> dataList = new List<TypeData>();
        for (int i = m_TypeSequence.Count - 1; i >= 0; i--)
        {
            string nameStr = "";
            if (m_TypeNameDict.TryGetValue(m_TypeSequence[i], out nameStr))
            {
                dataList.Add(new TypeData { name = nameStr, typeId = m_TypeSequence[i] });
            }
        }
        return dataList;
    }
}