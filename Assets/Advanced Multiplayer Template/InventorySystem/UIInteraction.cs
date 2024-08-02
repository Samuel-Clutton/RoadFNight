using System;
using UnityEngine;
using TMPro;

public class UIInteraction : MonoBehaviour {

	[SerializeField] private GameObject _content;
	[SerializeField] private TextMeshProUGUI _interactionText;

	public static PlayerInteractionModule playerInteraction;

	private PlayerInventoryModule _pIm;

	private void Start()
	{
		_pIm = FindObjectOfType<PlayerInventoryModule>();
	}

	private void Update() {
		if (playerInteraction == null || playerInteraction.currentInteractable == null || _pIm.inMenu) {
			_content.SetActive(false);
			return;
		}

		_content.SetActive(true);

		_interactionText.text = playerInteraction.currentInteractable.GetInfoText();
	}
}
