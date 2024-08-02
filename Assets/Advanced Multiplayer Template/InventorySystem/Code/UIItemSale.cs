using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemSale : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool saleMode;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(saleMode)
        {
            SellItem();
        }
    }

    public void SellItem()
    {
        UISlot uiSlot;
        uiSlot = GetComponent<UISlot>();

        uiSlot.playerInteractionModule.RemoveItem(uiSlot.playerInteractionModule.GetComponent<PlayerInventoryModule>(), uiSlot.sellPrice, uiSlot.item, 1, uiSlot.itemSlotIndex);
    }
}
