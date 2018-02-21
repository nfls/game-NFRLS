using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class CrossHairController : MonoBehaviour, ISingletionBehaviour {

	public static readonly Color DEFAULT_MESSAGE_COLOR = new Color(0.247f, 0.247f, 0.247f);
	public static readonly Color INTERACTIVE_MESSAGE_COLOR = new Color(0.451f, 0.878f, 0.290f);
	public static readonly Color NON_INTERACTIVE_MESSGAE_COLOR = Color.red;

	public static readonly float MESSAGE_DURATION_SHORT = 1f;
	public static readonly float MESSAGE_DURATION_MEDIUM = 3f;
	public static readonly float MESSAGE_DURATION_DEFAULT = 5f;
	public static readonly float MESSAGE_DURATION_LONG = 8f;

	public static GameObject viewBoard;
	public static Text infoText;
	public static Text messageText;
	public static CrossHairState currentState;

	public static CrossHairController crossHairController;
	public float detectDistance = 15f;
	public float interactDistance = 10f;
	public float pickedUpItemDistance = 1f;
	public float dropForce = 1f;
	public float viewedItemDistance = 1f;
	public float itemCameraRotateSpeed = 10f;
	public float scrollWheelSensity = 5f;
	public float minViewFov = 15f;
	public float maxViewFov = 90f;

	static Texture2D[] icons;

	static bool interacting;
	static ItemController interactingItemController;
	static Collider lastOnCollider;
	static ItemController lastOnItemController;
	static IEnumerator interactingItemUpdateTask;
	static IEnumerator messageTextDisplayTask;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
	}

	public void Init() {
		string[] stateNames = Enum.GetNames(typeof(CrossHairState));
		icons = new Texture2D[stateNames.Length];
		for (int i = 0; i < stateNames.Length; i++) {
			icons[i] = Resources.Load<Texture2D>("Textures/crosshair_" + stateNames[i].ToLower());
		}

		crossHairController = SingletionManager.GetSingletionBehaviour<CrossHairController>();
		infoText = SingletionManager.singletionCanvas.transform.Find("Info Text").GetComponent<Text>();
		messageText = SingletionManager.singletionCanvas.transform.Find("Message Text").GetComponent<Text>();
		viewBoard = SingletionManager.singletionCanvas.transform.Find("View Board").gameObject;
		viewBoard.SetActive(false);
		SetState(CrossHairState.OBSERVE);
		SetInfoText("");
	}

	void Update() {
		Ray ray = new Ray(SingletionManager.mainCamera.transform.position, SingletionManager.mainCamera.transform.forward);
		RaycastHit[] hitInfos = Physics.RaycastAll(ray, detectDistance);
		if (currentState == CrossHairState.OBSERVE || currentState == CrossHairState.CHECK) {
			if (interacting) {
				return;
			}
			if (hitInfos != null && hitInfos.Length > 0) {
				Collider collider = null;
				ItemController itemController = null;
				for (int i = 0; i < hitInfos.Length; i++) {
					collider = hitInfos[i].collider;
					itemController = collider.GetComponent<ItemController>();
					if (itemController) {
						break;
					}
				}
				if (itemController) {
					if (collider.Equals(lastOnCollider)) {
						OverItem(itemController);
					} else {
						lastOnCollider = collider;
						lastOnItemController = collider.GetComponent<ItemController>();
						EnterItem(itemController);
					}
				} else {
					ExitItem(lastOnItemController);
					lastOnCollider = null;
					lastOnItemController = null;
				}
			} else {
				if (lastOnItemController) {
					ExitItem(lastOnItemController);
					lastOnCollider = null;
					lastOnItemController = null;
				}
			}
		}
	}

	public static void SetState(CrossHairState state) {
		currentState = state;
		Texture2D icon = icons[(int)state];
		Cursor.SetCursor(icon, new Vector2(icon.width / 2, icon.height / 2), CursorMode.ForceSoftware);
	}

	public static void SetInfoText(string text) {
		SetInfoText(text, DEFAULT_MESSAGE_COLOR);
	}

	public static void SetInfoText(string text, bool isValid) {
		if (isValid) {
			SetInfoText(text, INTERACTIVE_MESSAGE_COLOR);
		} else {
			SetInfoText(text, NON_INTERACTIVE_MESSGAE_COLOR);
		}
	}

	public static void SetInfoText(string text, Color color) {
		infoText.text = "<color=" + "#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
	}

	public static void SetMessageText(string text, Color color, float duration) {
		if (messageTextDisplayTask != null) {
			crossHairController.StopCoroutine(messageTextDisplayTask);
		}
		messageText.text = "<color=" + "#" + ColorUtility.ToHtmlStringRGBA(color) + ">[Message] - " + text + "</color>";
		messageTextDisplayTask = ExeMessageTextDisplayTask(duration);
		crossHairController.StartCoroutine(messageTextDisplayTask);
	}

	public static void StartInteraction(ItemController itemController) {
		SetState(CrossHairState.INTERACT);
		interacting = true;
		interactingItemController = itemController;
		itemController.interacted = true;
		itemController.interacting = true;
		switch (itemController.interactionType) {
			case ItemController.InteractionType.PICK_UP: PickUp(itemController); break;
			case ItemController.InteractionType.VIEW: StartViewing(); break;
			case ItemController.InteractionType.OPERATE: Operate(); break;
		}
		interactingItemUpdateTask = ExeInteractingItemUpdatingTask();
		crossHairController.StartCoroutine(interactingItemUpdateTask);
		itemController.OnInteractionEntered();
	}

	public static void StopInteraction() {
		SetState(CrossHairState.OBSERVE);
		if (interactingItemController.interactOnce) {
			interactingItemController.interactive = false;
		}
		interacting = false;
		interactingItemController.interacting = false;
		ItemController itemController = interactingItemController;
		interactingItemController = null;
		switch (itemController.interactionType) {
			case ItemController.InteractionType.PICK_UP: DropDown(itemController); break;
			case ItemController.InteractionType.VIEW: StopViewing(itemController); break;
			case ItemController.InteractionType.OPERATE: break;
		}
		if (interactingItemUpdateTask != null) {
			crossHairController.StopCoroutine(interactingItemUpdateTask);
			interactingItemUpdateTask = null;
		}
		SetInfoText("");
		itemController.OnInteractionExited();
	}

	public static void PickUp(ItemController itemController) {
		if (itemController.itemBody) {
			itemController.itemBody.angularVelocity = Vector3.zero;
			itemController.itemBody.velocity = Vector3.zero;
			itemController.itemBody.useGravity = false;
		}
		itemController.itemCollider.enabled = false;
		SetInfoText(itemController.GetCheckingInfo(), itemController.interactive);
		Operate();
	}

	public static void DropDown(ItemController itemController) {
		itemController.itemCollider.enabled = true;
		if (itemController.itemBody) {
			itemController.itemBody.useGravity = true;
			itemController.itemBody.AddForce(SingletionManager.mainCamera.transform.forward * crossHairController.dropForce, ForceMode.Impulse);
		}
	}

	public static void StartViewing() {
		FirstPersonController.controllerEnabled = false;
		//Cursor.lockState = CursorLockMode.Confined;

		SingletionManager.itemCamera.gameObject.SetActive(true);
		/*
		SingletionManager.itemCamera.transform.SetPositionAndRotation(SingletionManager.mainCamera.transform.position, SingletionManager.mainCamera.transform.rotation);
		SingletionManager.itemCamera.transform.position = interactingItemController.transform.position - SingletionManager.itemCamera.transform.forward * crossHairController.viewedItemDistance;
		*/

		SingletionManager.itemCamera.transform.position = interactingItemController.transform.position - new Vector3(0, 0, crossHairController.viewedItemDistance);
		SingletionManager.itemCamera.transform.LookAt(interactingItemController.transform);

		interactingItemController.gameObject.layer = 10;
		foreach (Transform transform in interactingItemController.GetComponentsInChildren<Transform>()) {
			transform.gameObject.layer = 10;
		}

		viewBoard.SetActive(true);
		viewBoard.transform.Find("Panel/Head Text").GetComponent<Text>().text = "[" + interactingItemController.itemType + "] - " + interactingItemController.itemName;
		viewBoard.transform.Find("Panel/Info Text").GetComponent<Text>().text = interactingItemController.itemInfo;
	}

	public static void Operate() {
		if (interactingItemController.willSendMessageRequest) {
			CommunicationRequest request = new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.MESSAGE,
				contents = new Dictionary<string, object> {
					{"text", interactingItemController.messageText},
					{"color", interactingItemController.messageColor},
					{"duration", interactingItemController.messageDuration}
				}
			};

			CommunicationController.HandleCommunicationRequest(request);
		}
		if (interactingItemController.willSendMissionRequest) {
			CommunicationRequest request = new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.MISSION,
				contents = new Dictionary<string, object> {
					{"id", interactingItemController.missionId},
					{"status", interactingItemController.missionStatus}
				}
			};
			if (!interactingItemController.missionStatus.Equals("finished")) {
				request.contents["missionInfo"] = new MissionInfo {
					id = interactingItemController.missionId,
					title = interactingItemController.missionTitle,
					substitle = interactingItemController.missionSubtitle,
					description = interactingItemController.missionDescription,
					tip = interactingItemController.missionTip
				};
			}

			CommunicationController.HandleCommunicationRequest(request);
		}
		if (interactingItemController.willSendOtherRequest) {
			CommunicationRequest request = new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.OTHER,
				contents = new Dictionary<string, object>()
			};
			request.contents["name"] = interactingItemController.actionName;
			request.contents["deleteAfterExe"] = interactingItemController.deleteAfterExe;

			CommunicationController.HandleCommunicationRequest(request);
		}
	}

	public static void StopViewing(ItemController itemController) {
		viewBoard.SetActive(false);
		itemController.gameObject.layer = 0;
		foreach (Transform transform in itemController.GetComponentsInChildren<Transform>()) {
			transform.gameObject.layer = 0;
		}
		//Cursor.lockState = CursorLockMode.Locked;
		FirstPersonController.controllerEnabled = true;
	}

	static IEnumerator ExeMessageTextDisplayTask(float duration) {
		messageText.gameObject.SetActive(true);
		yield return new WaitForSeconds(duration);
		messageText.gameObject.SetActive(false);
	}

	static IEnumerator ExeInteractingItemUpdatingTask() {
		while (true) {
			yield return 0;
			switch (interactingItemController.interactionType) {
				case ItemController.InteractionType.PICK_UP: ExePickingUpUpdateTask(); break;
				case ItemController.InteractionType.VIEW: ExeViewingUpdateTask(); break;
				case ItemController.InteractionType.OPERATE: ExeOperatingUpdateTask(); break;
			}
			if (Input.GetButtonDown("Interact")) {
				StopInteraction();
			}
		}
	}

	static void ExePickingUpUpdateTask() {
		interactingItemController.transform.position = SingletionManager.mainCamera.transform.position + SingletionManager.mainCamera.transform.forward * crossHairController.pickedUpItemDistance;
	}

	static void ExeViewingUpdateTask() {
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");
		float fovPlus = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel") * crossHairController.scrollWheelSensity;

		SingletionManager.itemCamera.fieldOfView = Mathf.Clamp(SingletionManager.itemCamera.fieldOfView + fovPlus, crossHairController.minViewFov, crossHairController.maxViewFov);
		SingletionManager.itemCamera.transform.RotateAround(interactingItemController.transform.position, -SingletionManager.itemCamera.transform.right, vertical * crossHairController.itemCameraRotateSpeed);
		SingletionManager.itemCamera.transform.RotateAround(interactingItemController.transform.position, -SingletionManager.itemCamera.transform.up, horizontal * crossHairController.itemCameraRotateSpeed);

		if (Input.GetButtonDown("Reload")) {
			SingletionManager.itemCamera.transform.position = interactingItemController.transform.position - new Vector3(0, 0, crossHairController.viewedItemDistance);
			SingletionManager.itemCamera.transform.LookAt(interactingItemController.transform);
		}
	}

	static void ExeOperatingUpdateTask() {
		StopInteraction();
	}

	static void EnterItem(ItemController itemController) {
		SetState(CrossHairState.CHECK);
		SetInfoText(itemController.GetCheckingInfo(), itemController.interactive);
	}

	static void OverItem(ItemController itemController) {
		if (itemController.interactive) {
			if (Input.GetButtonDown("Interact")) {
				if ((itemController.transform.position - SingletionManager.mainCamera.transform.position).sqrMagnitude < crossHairController.interactDistance * crossHairController.interactDistance) {
					StartInteraction(itemController);
				}
			}
		}
	}

	static void ExitItem(ItemController itemController) {
		SetState(CrossHairState.OBSERVE);
		SetInfoText("");
	}

	static void OnSceneFinished() {
		interacting = false;
		interactingItemController = null;
		lastOnCollider = null;
		lastOnItemController = null;
	}

	public enum CrossHairState {
		OBSERVE,
		AIM,
		CHECK,
		INTERACT
	}
}
