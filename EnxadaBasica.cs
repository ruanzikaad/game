using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;

public class EnxadaBasica : ShopItem
{
    public EnxadaBasica(string name, int cost, int minimumLevel, int xpReward, GameObject prefab, Vector3 spawnOffset)
        : base(name, cost, minimumLevel, xpReward, prefab, spawnOffset)
    {
    }

    public override bool Purchase(int userId)
    {
        return base.Purchase(userId);
    }
}