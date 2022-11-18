using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static object m_Lock = new object();

    private static DataManager m_Instance;
    public static DataManager Instance 
    { 
        get
        {
            if (FindObjectsOfType<DataManager>().Length > 1)
            {
                Debug.Log("We have a problem...");
            }

            if (m_Instance != null)
                return m_Instance;
            lock (m_Lock)
            {
                GameObject dataManagerObject = new GameObject();
                m_Instance = dataManagerObject.AddComponent<DataManager>();
                DontDestroyOnLoad(dataManagerObject);
            }
            return m_Instance;
        }
    }

    string m_SaveFileName = "Data.sv";
    FileDataHandler m_DataHandler;
    GameData m_GameData;

    void Awake()
    {
        if (m_Instance != null)
        {
            Debug.LogError("Found more than 1 instance of the DataManager.");
            Destroy(this);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void InitDataManager()
    {
        if (m_Instance != null)
            return;

        GameObject dataManagerObj = new GameObject();
        m_Instance = dataManagerObj.AddComponent<DataManager>();
        m_Instance.NewGame();
        DontDestroyOnLoad(dataManagerObj);
    }

    public void NewGame()
    {
        m_DataHandler = new FileDataHandler(m_SaveFileName);
        m_GameData = m_DataHandler.LoadGameData();
        
        if (m_GameData == null)
            m_GameData = new GameData();
    }

    public void SaveGame()
    {
        m_DataHandler.Save(m_GameData);
    }

    public ref LevelData LoadLevel(int level)
    {
        return ref m_GameData.levelData[level - 2];
    }

    public ref LevelData[] LoadLevels()
    {
        return ref m_GameData.levelData;
    }

    public void SaveLevel(LevelData data, int level)
    {
        m_GameData.levelData.SetValue(data, level - 2);
    }

    private void OnDestroy()
    {
        SaveGame();
    }

}