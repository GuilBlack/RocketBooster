using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;

public class FileDataHandler
{
    private string m_SaveDirPath;
    private string m_SaveFileName;
    private byte[] m_Key;

    public FileDataHandler(string saveFileName)
    {
        m_SaveFileName = saveFileName;
        m_SaveDirPath = Application.persistentDataPath;
        m_Key = Encoding.UTF8.GetBytes("[M7'=rYf/ZQy\"}sOzJN4 :7:)]A]2OhN");
    }

    public FileDataHandler(string saveDirPath, string saveFileName)
    {
        m_SaveFileName = saveFileName;
        m_SaveDirPath = saveDirPath;
        m_Key = Encoding.UTF8.GetBytes("[M7'=rYf/ZQy\"}sOzJN4 :7:)]A]2OhN");
    }

    public GameData LoadGameData()
    {
        string pathToSaveFile = Path.Combine(m_SaveDirPath, m_SaveFileName);
        GameData gameData = null;
        if (!File.Exists(pathToSaveFile))
            return gameData;

        try
        {
            using (Aes aes = Aes.Create())
            using (FileStream fs = new(pathToSaveFile, FileMode.Open, FileAccess.Read))
            {
                aes.KeySize = 256;

                byte[] iv = new byte[aes.IV.Length];
                fs.Read(iv, 0, iv.Length);

                using (CryptoStream cs = new(fs, aes.CreateDecryptor(m_Key, iv), CryptoStreamMode.Read))
                using (StreamReader sr = new(cs))
                {
                    string jsonData = sr.ReadToEnd();
                    gameData = JsonConvert.DeserializeObject<GameData>(jsonData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Couldn't reat data from file " + pathToSaveFile + " because of the following error: " + e.Message);
        }
        return gameData;
    }

    public void Save(GameData data)
    {
        string pathToSaveFile = Path.Combine(m_SaveDirPath, m_SaveFileName);
        try
        {
            Directory.CreateDirectory(m_SaveDirPath);

            string jsonData = JsonConvert.SerializeObject(data);

            Debug.Log(jsonData);
            
            using (Aes aes = Aes.Create())
            using (FileStream fs = new(pathToSaveFile, FileMode.Create, FileAccess.Write))
            {
                aes.KeySize = 256;

                fs.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new(fs, aes.CreateEncryptor(m_Key, aes.IV), CryptoStreamMode.Write))
                using (StreamWriter sw = new(cs))
                {
                    sw.WriteAsync(jsonData);
                }
            }
        } 
        catch (Exception e)
        {
            Debug.LogError("Couldn't save data to the file " + pathToSaveFile + "because of the following error: " + e.Message);
        }
    }
}
