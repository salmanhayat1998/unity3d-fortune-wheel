using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class item
{
    public string label;
    public Sprite icon;
    public int amount;
}

public enum spinRewardType
{
    coins,
    diamonds,
    hints
}
