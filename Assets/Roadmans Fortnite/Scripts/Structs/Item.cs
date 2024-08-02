using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Represents an individual item in the inventory system
/// </summary>
[System.Serializable]
public struct Item {

    public int hash; // unique ID?

    [System.NonSerialized]
    private ItemSO _itemSO; // cashed reference to the items ScriptableObject
    [System.NonSerialized]
    private int oldHash; // previous ID to detect changes
		
    /// <summary>
    /// Get the associated scriptable object (SO) for this item/
    /// If the cashed item SO is null or has changed, it fetchees the items SO based on the Hash ID
    /// </summary>
    public ItemSO itemSO {
        get {
            if (_itemSO == null || hash != oldHash) {
                _itemSO = ItemSO.GetItemSO(hash);
                oldHash = hash;
            }
            return _itemSO;
        }
    }

    public float currentShelfLifeInSeconds;

    /// <summary>
    /// Initialise a new instance of the Item Struct with the specified unique name with a current shelf life in seconds
    /// </summary>
    /// <param name="itemSOUniqueName">ID</param>
    /// <param name="currentShelfLifeInSeconds">Life Time</param>
    public Item(string itemSOUniqueName, float currentShelfLifeInSeconds) {
        hash = itemSOUniqueName.GetStableHashCode(); // custom extension method for mirror to formulate a standardized hash for each machine

        _itemSO = null;
        oldHash = hash;

        this.currentShelfLifeInSeconds = currentShelfLifeInSeconds;
    }

    public string TooltipText => itemSO.GetTooltipText().Replace("{CURRENT_SHELF_LIFE}",
        System.TimeSpan.FromSeconds(currentShelfLifeInSeconds).ToString("hh\\:mm\\:ss"));
}


