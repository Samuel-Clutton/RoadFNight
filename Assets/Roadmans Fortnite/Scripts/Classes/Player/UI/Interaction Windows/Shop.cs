using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Shop : NetworkBehaviour
{
	private PlayerInventoryModule _pIm;

	private void Start()
	{
		_pIm = GetComponent<PlayerInventoryModule>();
	}

	/// <summary>
	/// Activation toggle for the UI of the ShopUI, can be done in event based instead of this but that can happen in phase 3
	/// </summary>
	public void ShopUIToggle()
	{
	    // PSEUDO
 		// allow for expanding to event based later PHASE 3
 		
	    if(!PlayerInventoryModule.Keyboard.tabKey.wasPressedThisFrame) return;
	    
	    _pIm.inMenu = !_pIm.inMenu;

	    switch (_pIm.inMenu)
	    {
	        case true:
 				EnterShopMenu();
 		        break;
	        case false:
 		        ExitShopMenu();
 		        break;
	        
	    }
	}

	/// <summary>
	/// Toggle on the Shop Menu, checking if BSsystem has in menu to toggle another instance
	/// </summary>
	private void EnterShopMenu()
	{
	    UIPlayerInventory.SetActive(true);
	    UIPlayerInventory.InventoryUI.SetActive(true);
	    TPController.TPCameraController.LockCursor(false);
	}

	/// <summary>
	/// Toggle off the shop menu
	/// </summary>
	private void ExitShopMenu()
	{
	    UIPlayerInventory.SetActive(false);
	    UIPlayerInventory.InventoryUI.SetActive(false);
	    TPController.TPCameraController.LockCursor(true);
	}
	     
      
}
