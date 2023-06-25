using System.IO;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    private static APIKeyManager inst;
    public static APIKeyManager Inst 
    {
        get 
        { 
            return inst; 
        } 
    }

    private string configFilePath = "config.json";
    private string apiKey;
    private string organizeKey;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            LoadConfig();
        }
    }

    private void LoadConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, configFilePath);

        if (File.Exists(configPath))
        {
            string configData = File.ReadAllText(configPath);
            ConfigJsonData jsonData = JsonUtility.FromJson<ConfigJsonData>(configData);

            apiKey = jsonData.apiKey;
            organizeKey = jsonData.organizeKey;
        }
        else
        {
            Debug.LogError("config.json 파일을 찾을 수 없습니다!");
        }
    }

    public string GetApiKey()
    {
        return apiKey;
    }

    public string GetOrganizeKey()
    {
        return organizeKey;
    }

    private class ConfigJsonData
    {
        public string apiKey;
        public string organizeKey;
    }
}