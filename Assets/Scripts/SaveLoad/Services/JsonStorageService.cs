using UnityEngine;
using System.IO;

public class JsonStorageService : IStorageService
{
    public void Save<T>(string key, T data)
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public T Load<T>(string key)
    {
        string path = Path.Combine(Application.persistentDataPath, key + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        return default;
    }
}