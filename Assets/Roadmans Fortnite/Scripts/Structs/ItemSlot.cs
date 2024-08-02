using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemSlot
{

    public Item item;
    public int amount;

    public int IncreaseBy(int value) {
        int c = Mathf.Clamp(value, 0, item.itemSO.stackSize - amount); // ?
        amount += c;
        return c;
    }

    public int DecreaseBy(int value) {
        int c = Mathf.Clamp(value, 0, amount);
        amount -= c;
        return c;
    }

    public string TooltipText => item.TooltipText.Replace("{AMOUNT}", amount.ToString());
}
