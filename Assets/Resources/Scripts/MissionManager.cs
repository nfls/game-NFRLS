using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour, ISingletionBehaviour {

	static Queue<MissionInfo> finishedMissions;
	static Queue<MissionInfo> followingMissions;
	static MissionInfo currentMission;

	static MissionManager missionManager;
	static GameObject missionBoard;
	static GameObject infoPanel;
	static GameObject tipPanel;
	static Text idText;
	static Text titleText;
	static Text subtitleText;
	static Text descriptionText;
	static Text tipText;

	static bool hasMissionInProgress;

	void Update() {
		if (Input.GetButtonDown("Switch Mission Info")) {
			infoPanel.SetActive(!infoPanel.activeSelf);
		}
		if (Input.GetButtonDown("Switch Mission Tip")) {
			tipPanel.SetActive(!tipPanel.activeSelf);
		}
	}

	public void Init() {
		missionManager = SingletionManager.GetSingletionBehaviour<MissionManager>();
		missionBoard = SingletionManager.singletionCanvas.transform.Find("Mission Board").gameObject;
		infoPanel = missionBoard.transform.Find("Info Panel").gameObject;
		tipPanel = missionBoard.transform.Find("Tip Panel").gameObject;
		idText = infoPanel.transform.Find("Id Text").GetComponent<Text>();
		titleText = infoPanel.transform.Find("Title Text").GetComponent<Text>();
		subtitleText = infoPanel.transform.Find("Subtitle Text").GetComponent<Text>();
		descriptionText = infoPanel.transform.Find("Description Text").GetComponent<Text>();
		tipText = tipPanel.transform.Find("Tip Text").GetComponent<Text>();

		tipPanel.SetActive(false);

		finishedMissions = new Queue<MissionInfo>();
		followingMissions = new Queue<MissionInfo>();

		MissionInfo missionInfo = new MissionInfo {
			id = 1,
			title = "Watch TV",
			substitle = "To kill time",
			description = "Just another boring summer afternoon, you decide to find something to do.",
			tip = "Go to TV and turn it on."
		};
		AddNewMission(missionInfo);
	}

	public static void AddNewMission(MissionInfo missionInfo) {
		followingMissions.Enqueue(missionInfo);
		if (!hasMissionInProgress && followingMissions.Count > 0) {
			StartNewMission();
		}
	}

	public static void ReceiveMissionFinishedMessage(int id) {
		if (id == currentMission.id) {
			FinishCurrentMission();
		}
	}

	static void FinishCurrentMission() {
		NotificationManager.AddNotification(NotificationInfo.NotificationType.MISSION, "Mission Finished", currentMission.title + " - " + currentMission.substitle, NotificationInfo.DURATION_MEDIUM);
		hasMissionInProgress = false;
		currentMission.finished = true;
		currentMission.inProgress = false;
		finishedMissions.Enqueue(currentMission);
		currentMission = null;
		if (followingMissions.Count > 0) {
			StartNewMission();
		}
	}

	static void StartNewMission() {
		hasMissionInProgress = true;
		currentMission = followingMissions.Dequeue();
		DisplayNewMission();
	}

	static void DisplayNewMission() {
		NotificationManager.AddNotification(NotificationInfo.NotificationType.MISSION, "New Mission", currentMission.title + " - " + currentMission.substitle, NotificationInfo.DURATION_LONG);
		RefreshMissionBoard();
	}

	static void RefreshMissionBoard() {
		idText.text = "Mission [" + currentMission.id + "]";
		titleText.text = currentMission.title;
		subtitleText.text = "- " + currentMission.substitle;
		descriptionText.text = currentMission.description;
		tipText.text = currentMission.tip;
	}

	static void ClearMissionBoard() {
		idText.text = "";
		titleText.text = "";
		subtitleText.text = "";
		descriptionText.text = "";
		tipText.text = "";
	}

	public static void Show() {
		missionBoard.SetActive(true);
	}

	public static void Hide() {
		missionBoard.SetActive(false);
	}
}
