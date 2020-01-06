using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public string DataPath = Application.dataPath + "/Resources/Data.ini";

    private Dictionary<string, Dictionary<string, string>> m_DataDict = new Dictionary<string, Dictionary<string, string>>();
    public override void Initialize()
    {
        using (StreamReader sin = new StreamReader(new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
        {
            string classStr = "";
            while (!sin.EndOfStream)
            {
                string str = sin.ReadLine();
                if (str.StartsWith("[") && str.EndsWith("]"))
                {
                    classStr = str;
                    m_DataDict.Add(classStr, new Dictionary<string, string>());
                }
                else
                {
                    string[] strs = str.Split('=');
                    if (strs.Length == 2)
                    {
                        m_DataDict[classStr].Add(strs[0], strs[1]);
                    }
                }
            }
        }
    }

    public void WriteData()
    {
        if(File.Exists(DataPath))
            File.Delete(DataPath);

        using (StreamWriter sin = new StreamWriter(new FileStream(DataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
        {
            string title = "[DailyNoteManager]";
            sin.WriteLine(title);
            WriteDailyData(sin);

            title = "[PlanManager]";
            sin.WriteLine(title);
            title = "[WealthManager]";
            sin.WriteLine(title);
        }
    }

    public void WriteDailyData(StreamWriter sin)
    {
        
    }
}