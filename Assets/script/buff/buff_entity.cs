using Unity;
using UnityEngine;
using System.Collections.Generic;


class BuffEntity : MonoBehaviour
{
    [Header("buff id")]
    public int number;
    [Header("buff名称")]
    public string buffName;
    [Header("buff描述")]
    public string buffDescription;
    [Header("buff 层数")]
    public int layer;

    [Header("效果id")]
    public EffectEntity effectEntity;

    [Header("参数列表")]
    public List<int> data;
    
    public void excute()
    {
        Debug.Log("buff" + buffName + "：执行");
        effectEntity.Execute();
    }
}