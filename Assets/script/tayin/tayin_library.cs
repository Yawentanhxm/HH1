using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TayinLibrary
{

    [System.Serializable]
    public class TayinDataList
    {
        public TayinData[] tayinList;
    }

    [System.Serializable]
    public class TayinData
    {
        public int id;
        public string name;
        public int effect_id;
        public string description;
        public List<int> data;
    }

    public List<TayinEntity> tayinList = new List<TayinEntity>();
    public EffectFactory effectFactory = new EffectFactory();
    public void LoadLibrary()
    {
        // 读取拓印data目录下的json文件
        string filePath = Path.Combine(Application.streamingAssetsPath, "data/tayin_data.json");
        
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            TayinDataList temp = JsonUtility.FromJson<TayinDataList>(jsonContent);
            for (int i = 0; i < temp.tayinList.Length; i++)
            {
                TayinData data = temp.tayinList[i];
                EffectEntity effect = effectFactory.GetEffect(data.effect_id);
                TayinEntity tayinEntity = new TayinEntity(data.id, data.name, effect, data.data, data.description);
                tayinList.Add(tayinEntity);
                Debug.Log("拓印技能：" + tayinEntity.Name + "加载成功" + "对应技能：" + data.effect_id + "加载成功");
            }
            // Debug.Log($"Name: {data.name}, Level: {data.effect_id}, Health: {data.description}");
        }
        else
        {
            Debug.LogError("JSON文件不存在: " + filePath);
        }
    }

    public TayinEntity GetTayinDataById(int id)
    {
        return tayinList[id];
    }
    public TayinEntity GetRandomTayinData()
    {
        int index = Random.Range(0, tayinList.Count);
        return tayinList[index];
    }
    public void SaveLibrary()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "data/tayin_data.json");
        string jsonContent = JsonUtility.ToJson(tayinList);
        File.WriteAllText(filePath, jsonContent);
    }
}