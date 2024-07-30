using UnityEngine;
using Mirror;
using RedicionStudio.NetworkUtils;

namespace RedicionStudio.InventorySystem {

	[RequireComponent(typeof(Collider))]
	public class ItemDrop : NetworkBehaviour, INetInteractable<PlayerInventoryModule> {

		[SyncVar] public Item item;
		[SyncVar] public int amount;
        [HideInInspector] public bool remove = false;

		private void Start() {
            if(!remove)
			    Instantiate(item.itemSO.modelPrefab).transform.SetParent(transform, false);
            else
                NetworkServer.Destroy(gameObject);
        }

		// (Server)
		public void OnServerInteract(PlayerInventoryModule player) {
			Debug.Log("Calling on server");
			if (amount < 1)
			{
				Debug.Log("Amount is less than 1, destroying object");
				NetworkServer.Destroy(gameObject);
			}

			if (player.Add(item, amount))
			{
				Debug.Log("Item added to player inventory, destroying object");
				amount = 0; // ?
				NetworkServer.Destroy(gameObject);
			}
			else
			{
				Debug.Log("Item could not be added to player inventory");
			}
		}

		// (Client)
		public void OnClientInteract(PlayerInventoryModule player) { }

		// (Client)
		public string GetInfoText() {
			if (amount > 0 && item.itemSO != null) { // ?
				return amount > 1 ? item.itemSO.uniqueID + " (" + amount + ')' : item.itemSO.uniqueID; // ?
			}
			return "???";
		}
	}
}
