using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

	public int itemId = -1;
	public string itemName = "Unknown";
	public string itemType = "Item";
	public string itemInfo = "An unknown item";
	public bool interactive;
	public bool interacting;
	public bool interacted;
	public InteractionType interactionType;

	public Collider itemCollider;
	public Rigidbody itemBody;

	protected virtual void Start() {
		itemCollider = GetComponent<Collider>();
		itemBody = GetComponent<Rigidbody>();
	}

	public virtual string GetCheckingInfo() {
		string info = "[" + itemType + "] - " + itemName;
		if (interactive) {
			info += " - <b>[F]</b> " + interactionType.ToString().Replace('_', ' ');
		}
		return info;
	}

	public virtual void OnInteractionEntered() {

	}

	public virtual void OnInteractionStayed() {

	}

	public virtual void OnInteractionExited() {

	}

	public enum InteractionType {
		PICK_UP,
		VIEW,
		OPERATE
	}
}
