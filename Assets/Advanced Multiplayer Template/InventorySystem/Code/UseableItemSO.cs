using UnityEngine;
using Mirror;

namespace RedicionStudio.InventorySystem {

	public abstract class UseableItemSO : ItemSO {

		[Space]
		public float cooldownInSeconds;
		public string cooldownTag;

		/// <summary>
		/// (Server)
		/// </summary>
		public virtual void Use(PlayerInventoryModule playerInventory, int slotIndex) {
			if (cooldownInSeconds > 0f)
			{
				string itemSoUniqueID = playerInventory.Slots[slotIndex].item.itemSO.uniqueID;
				string tag = GetCooldownTag();
				playerInventory.SetCooldown(itemSoUniqueID, tag, cooldownInSeconds);
			}
		}

		
		public virtual bool CanBeUsed(PlayerInventoryModule playerInventory, int slotIndex)
		{
			string itemSoUniqueID = playerInventory.slots[slotIndex].item.itemSO.uniqueID;
			string tag = GetCooldownTag();

			return playerInventory.GetCooldown(itemSoUniqueID, tag) == 0f;
		}

		public virtual string GetCooldownTag()
		{
			return cooldownTag;
		}
		

		/// <summary>
		/// (Client)
		/// </summary>
		public abstract void OnUsed(PlayerInventoryModule playerInventory);

		protected override void OnValidate() {
			base.OnValidate();

			if (stackSize > 1) {
				stackSize = 1;
			}
		}
	}
}
