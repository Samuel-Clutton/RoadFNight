using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Improves performance to check equality within the new dictionary
/// </summary>
[Serializable]
public struct ItemCooldownKey : IEquatable<ItemCooldownKey>
{
    public string uniqueID;
    public string cooldownTag;

    public ItemCooldownKey(string lUniqueID, string lCooldownTag) // Constructor for struct
    {
        this.uniqueID = lUniqueID;
        this.cooldownTag = lCooldownTag;
    }

    /// <summary>
    /// Overrides the standard equals method for system.object
    /// checks if the object passed is of the type ItemCooldownKey
    /// if it is it casts obj to cooldown key
    /// compares unique tags and cooldown tag
    ///
    /// Allows ItemCooldownKey to be compared with any object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj) 
    {
        if (obj is ItemCooldownKey other)
        {
            return uniqueID == other.uniqueID && cooldownTag == other.cooldownTag;
        }
        return false;
    }
    
    /// <summary>
    /// Compares current  and others Unique ID and cooldownTag
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ItemCooldownKey other)
    {
        return uniqueID == other.uniqueID && cooldownTag == other.cooldownTag;
    }
    
    /// <summary>
    /// Overrides the method to get hash codes from System.Object
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return (uniqueID, cooldownTag).GetHashCode();
    }
}
