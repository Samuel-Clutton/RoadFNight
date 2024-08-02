using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RedicionStudio.UIUtils;


public class UISlot : MonoBehaviour {

	public Button button;
	public UITooltip uITooltip;
	public UIDragAndDrop uIDragAndDrop;
	public Image image;
	public Image rarityImage;
	public TextMeshProUGUI rarityText;
	public GameObject amountContent;
	public Image cooldownIndicator;
    public TextMeshProUGUI amountText;
    [HideInInspector] public Item item;
    [HideInInspector] public PlayerInteractionModule playerInteractionModule;
    [HideInInspector] public int sellPrice;
    [HideInInspector] public int itemSlotIndex;
}

