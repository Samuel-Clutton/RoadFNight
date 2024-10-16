﻿using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using RedicionStudio.InventorySystem;
using RedicionStudio.NetworkUtils;

public class PlayerInteractionModule : NetworkBehaviour
{

    [Header("Player Modules")]
    public PlayerInventoryModule playerInventory;

    [SerializeField] private LayerMask ignoreLayers;
    
    [HideInInspector] public INetInteractable<PlayerInventoryModule> currentInteractable;

    [SerializeField] private float _maxDistance;

    [Space]
    [SerializeField] private GameObject UIMessagePrefab;
    GameObject instantiatedUIMessage;

    private static Transform _camera;

    private static Vector3 _position;
    private static Vector3 _forward;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //_camera = FindObjectOfType<Camera>().transform;
        _camera = GameObject.Find("MainCamera").transform;
        UIInteraction.playerInteraction = this;
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        _position = _camera.position;
        _forward = _camera.forward;

        Raycast(_position, _forward);

        _keyboard = Keyboard.current;

        if (currentInteractable == null || _keyboard == null)
        {
            return;
        }

        if (_keyboard.fKey.wasPressedThisFrame)
        {
            currentInteractable.OnClientInteract(playerInventory);
            CmdInteract(_position, _forward);
        }
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            UIInteraction.playerInteraction = null;
        }
    }

    private void Raycast(Vector3 position, Vector3 forward)
    {
        if (Physics.Raycast(position, forward, out RaycastHit hitInfo, _maxDistance, ignoreLayers))
        {
           print($"I found something to interact with: {hitInfo.transform.name}");
        }
    }

    private static Keyboard _keyboard;

    [Command]
    public void CmdInteract(Vector3 position, Vector3 forward)
    {
        Raycast(position, forward);
        if (currentInteractable != null)
        {
            currentInteractable.OnServerInteract(playerInventory);
        }
    }

    /// <summary>
    /// adding a collectable(s) to the player's inventory that they have purchased from a shop, also tracking it's price
    /// </summary>
    /// <param name="player">player the has bought the item</param>
    /// <param name="itemPrice">price of the item</param>
    /// <param name="item">item information</param>
    /// <param name="amount">number of items purchased</param>
    public void AddItem(PlayerInventoryModule player, int itemPrice, RedicionStudio.InventorySystem.Item item, int amount)
    {
        if (instantiatedUIMessage != null)
            Destroy(instantiatedUIMessage);

        instantiatedUIMessage = Instantiate(UIMessagePrefab);

        // if the player doesn't have enough funds to make the purchase, this displays a message on the UI informing the player and returns to the calling method
        if (player.GetComponent<NetPlayer>().funds < itemPrice)
        {
            instantiatedUIMessage.GetComponent<UIMessage>().ShowMessage("Not enough funds");

            return;
        }

        // if the player has enough funds, the item(s) will be added to the player's inventory
        instantiatedUIMessage.GetComponent<UIMessage>().ShowMessage("Item: " + item.itemSO.uniqueName + " " + amount + "x" + " purchased");
        CmdAddItem(player, itemPrice, item, amount);
    }

    [Command]
    public void CmdAddItem(PlayerInventoryModule player, int itemPrice, RedicionStudio.InventorySystem.Item item, int amount)
    {
        NetPlayer netPlayerToGiveFunds;
        netPlayerToGiveFunds = player.GetComponent<NetPlayer>();
        if (netPlayerToGiveFunds.funds < itemPrice)
        {
            //Not enough funds
        }
        else if (netPlayerToGiveFunds.funds == itemPrice || netPlayerToGiveFunds.funds > itemPrice)
        {
            netPlayerToGiveFunds.funds -= itemPrice;
            player.Add(item, amount);
        }
    }

    /// <summary>
    /// removes an item from a player's inventory after selling it, and returns appropriate funds
    /// </summary>
    /// <param name="player">player selling the item</param>
    /// <param name="sellPrice">amount received for selling item</param>
    /// <param name="item">information on item being sold</param>
    /// <param name="amount">number of items being sold</param>
    /// <param name="itemSlotIndex">position in inventory where item will be placed</param>
    public void RemoveItem(PlayerInventoryModule player, int sellPrice, Item item, int amount, int itemSlotIndex)
    {
        NetPlayer netPlayerToGiveFunds;

        if (instantiatedUIMessage != null)
            Destroy(instantiatedUIMessage);

        instantiatedUIMessage = Instantiate(UIMessagePrefab);

        instantiatedUIMessage.GetComponent<UIMessage>().ShowMessage("Item: " + item.itemSO.uniqueName + " " + amount + "x" + " sold" + " for" + "$" + sellPrice);
        player.CmdDropAndRemoveItem(itemSlotIndex, true);
        netPlayerToGiveFunds = player.GetComponent<NetPlayer>();
        CmdRemoveItem(player, netPlayerToGiveFunds, sellPrice, item, amount);
    }

    [Command]
    public void CmdRemoveItem(PlayerInventoryModule player, NetPlayer giveFunds, int sellPrice, Item item, int amount)
    {
        giveFunds.funds += sellPrice;
        //player.Remove(item, amount);
    }

    /// <summary>
    /// adds funds to player's account when collecting money
    /// </summary>
    /// <param name="player">player who has collected the money and is having their funds increased</param>
    /// <param name="amount">amount of funds added</param>
    public void AddMoney(PlayerInventoryModule player, int amount)
    {
        NetPlayer netPlayerToGiveFunds;
        netPlayerToGiveFunds = player.GetComponent<NetPlayer>();
        if (isServer)
        {
            netPlayerToGiveFunds.funds += amount;
            RpcAddMoney(netPlayerToGiveFunds, amount);
        }
        else if (hasAuthority)
        {
            if (instantiatedUIMessage != null)
                Destroy(instantiatedUIMessage);

            instantiatedUIMessage = Instantiate(UIMessagePrefab);

            instantiatedUIMessage.GetComponent<UIMessage>().ShowMessage("Amount: " + "$" + amount + " added");
            CmdAddMoney(netPlayerToGiveFunds, amount);
        }
    }

    [Command]
    public void CmdAddMoney(NetPlayer netPlayer, int amount)
    {
        netPlayer.funds += amount;
    }

    [ClientRpc]
    public void RpcAddMoney(NetPlayer netPlayer, int amount)
    {
        netPlayer.funds += amount;
    }

    /// <summary>
    /// remove funds from player
    /// </summary>
    /// <param name="player">player having funds removed</param>
    /// <param name="amount">amount of money being removed</param>
    public void RemoveMoney(PlayerInventoryModule player, int amount)
    {
        if (instantiatedUIMessage != null)
            Destroy(instantiatedUIMessage);

        instantiatedUIMessage = Instantiate(UIMessagePrefab);


        if (player.GetComponent<NetPlayer>().funds < amount)
        {
            instantiatedUIMessage.GetComponent<UIMessage>().ShowMessage("Not enough funds");

            return;
        }

        instantiatedUIMessage.GetComponent<UIMessage>().ShowMessage("Amount: " + "$" + amount + " removed");
        CmdRemoveMoney(player, amount);
    }

    [Command]
    public void CmdRemoveMoney(PlayerInventoryModule player, int amount)
    {
        if (player.GetComponent<NetPlayer>().funds < amount)
        {
            // Amount cannot be removed because the player does not have sufficient funds.
        }
        else if (player.GetComponent<NetPlayer>().funds == amount || player.GetComponent<NetPlayer>().funds > amount)
        {
            player.GetComponent<NetPlayer>().funds -= amount;
        }
    }




}
