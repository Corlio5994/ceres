using System;
using UnityEngine;

[System.Serializable]
public class Item: ICloneable {
    [HideInInspector] public int id;
    public string name;
    [HideInInspector] public string description = "";
    [HideInInspector] public float weight;
    [HideInInspector] public int rarity = 1;
    [HideInInspector] public string category = "Miscelaneous";

    public int count = 1;

    public object Clone() {
        return MemberwiseClone();
    }
}