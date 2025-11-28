using UnityEngine;
using System.Collections.Generic;
using System.IO; 

public class CardLibary
{

    [System.Serializable]
    public class CardList
    {
        public List<CardData> cardList;
    }

    public List<CardData> cardList = new List<CardData>();
    public void LoadLibrary()
    {
        // 读取拓印data目录下的json文件
        string filePath = Path.Combine(Application.streamingAssetsPath, "data/player_library_data.json");
        
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            CardList temp = JsonUtility.FromJson<CardList>(jsonContent);
            cardList = temp.cardList;
            Debug.Log($"cardList 读取成功 {cardList[0]}");
        }
        else
        {
            Debug.LogError("JSON文件不存在: " + filePath);
        }
    }
    public CardData GetRandomCardData()
    {
        int index = Random.Range(0, cardList.Count);
        return cardList[index];
    }
    public void SaveLibrary()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "data/player_library_data.json");
        string jsonContent = JsonUtility.ToJson(cardList);
        Debug.Log($"cardList {jsonContent}");
        File.WriteAllText(filePath, jsonContent);
        Debug.Log($"cardList 保存成功");
    }
}