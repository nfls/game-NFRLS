using System;
using UnityEngine;

public class ItemController : MonoBehaviour {

	public int itemId = -1;
	public string itemName = "Unknown";
	public string itemType = "Item";
	public string itemInfo = "An unknown item";
	public bool interactOnce;
	public bool interactive;
	public bool interacting;
	public bool interacted;
	public InteractionType interactionType;

	public bool willSendMessageRequest;
	public string messageText;
	public Color messageColor = CrossHairController.DEFAULT_MESSAGE_COLOR;
	public float messageDuration = CrossHairController.MESSAGE_DURATION_DEFAULT;
	public bool willSendMissionRequest;
	public int missionId;
	public string missionStatus = "finished";
	public string missionTitle;
	public string missionSubtitle;
	public string missionDescription;
	public string missionTip;
	public bool willSendOtherRequest;
	public string actionName;
	public bool deleteAfterExe;

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
