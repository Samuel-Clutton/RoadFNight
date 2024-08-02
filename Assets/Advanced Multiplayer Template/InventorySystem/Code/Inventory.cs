using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using RedicionStudio.NetworkUtils;


public class SyncListItemSlot : SyncList<ItemSlot> { }

public abstract class Inventory : ItemContainer {

public bool PossibleToAdd(Item item, int amount) {
	for (int i = 4; i < slots.Count; i++) {
		if (slots[i].amount == 0) {
			amount -= item.itemSO.stackSize;
		}
		else if (slots[i].item.Equals(item)) { // ?
			amount -= slots[i].item.itemSO.stackSize - slots[i].amount;
		}

		if (amount <= 0) {
			return true;
		}
	}

	return false;
}

public bool Add(Item item, int amount) {
	if (!PossibleToAdd(item, amount)) {
		return false;
	}

	int i;

	for (i = 4; i < slots.Count; i++) {
		if (slots[i].amount > 0 && slots[i].item.Equals(item)) { // ?
			ItemSlot tempSlot = slots[i];
			amount -= tempSlot.IncreaseBy(amount);
			slots[i] = tempSlot;
		}

		if (amount <= 0) {
			return true;
		}
	}

	for (i = 4; i < slots.Count; i++) {
		if (slots[i].amount == 0) {
			int tempValue = Mathf.Min(amount, item.itemSO.stackSize);
			slots[i] = new ItemSlot { item = item, amount = tempValue };
			amount -= tempValue;
		}

		if (amount <= 0) {
			return true;
		}
	}

	return false;
}

public bool Remove(Item item, int amount) {
	ItemSlot slot;
	for (int i = 0; i < slots.Count; i++) {
		slot = slots[i];
		if (slot.amount > 0 && slot.item.Equals(item)) { // ?
			amount -= slot.DecreaseBy(amount);
			slots[i] = slot;

			if (amount == 0) {
				return true;
			}
		}
	}

	return false;
}
}

