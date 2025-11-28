using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class EffectFactory
{
    private static readonly Dictionary<int, Func<EffectEntity>> effectCreators = 
        new Dictionary<int, Func<EffectEntity>>
    {
        { 1, () => new Effect_1() },
        { 2, () => new Effect_2() },
        { 3, () => new Effect_3() },
        { 4, () => new Effect_4() },
        { 5, () => new Effect_5() },
        { 6, () => new Effect_6() },
        // 可以轻松添加新的效果
    };

    public EffectEntity GetEffect(int id)
    {
        if (effectCreators.TryGetValue(id, out var creator))
        {
            return creator();
        }
        
        Debug.LogWarning($"未找到ID为 {id} 的效果");
        return null;
    }

    // 批量获取多个效果
    public List<EffectEntity> GetEffects(params int[] ids)
    {
        return ids.Select(GetEffect).Where(effect => effect != null).ToList();
    }

    // 注册新的效果创建器（运行时动态扩展）
    public void RegisterEffect(int id, Func<EffectEntity> creator)
    {
        effectCreators[id] = creator;
    }

    // 获取所有可用的效果ID
    public IEnumerable<int> GetAvailableEffectIds()
    {
        return effectCreators.Keys;
    }
}