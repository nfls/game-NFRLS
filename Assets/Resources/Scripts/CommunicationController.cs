using System.Collections.Generic;
using System;
using UnityEngine;

public class CommunicationController : ISingletionBehaviour {

	static CommunicationController communicationController;

	static Dictionary<string, Action<Dictionary<string, object>>> registeredOtherEvents;

	public void Init() {
		communicationController = SingletionManager.GetSingletionBehaviour<CommunicationController>();
		registeredOtherEvents = new Dictionary<string, Action<Dictionary<string, object>>>();
	}

	public static void RegisterOtherEvent(string name, Action action) {
		Action<Dictionary<string, object>> a = delegate (Dictionary<string, object> para) {
			action.Invoke();
		};
		RegisterOtherEvent(name, a);
	}

	public static void RegisterOtherEvent(string name, Action<Dictionary<string, object>> action) {
		registeredOtherEvents[name] = action;
	}

	public static void HandleCommunicationRequest(CommunicationRequest request) {
		switch (request.communicationType) {
			case CommunicationType.ACHIEVEMENT: communicationController.HandleMessageRequest(request.contents); break;
			case CommunicationType.MESSAGE: communicationController.HandleMessageRequest(request.contents); break;
			case CommunicationType.MISSION: communicationController.HandleMissionRequest(request.contents); break;
			case CommunicationType.OTHER: communicationController.HandleOtherRequest(request.contents); break;
		}
	}

	protected virtual void HandleAchievementRequest(Dictionary<string, object> contents) {

	}

	protected virtual void HandleMessageRequest(Dictionary<string, object> contents) {
		CrossHairController.SetMessageText((string)contents["text"], (Color)contents["color"], (float)contents["duration"]);
	}

	protected virtual void HandleMissionRequest(Dictionary<string, object> contents) {
		if (((string)contents["status"]).Equals("finished")) {
			MissionManager.HandleMissionFinishedRequest((int)contents["id"]);
		} else {
			MissionManager.AddNewMission((MissionInfo)contents["missionInfo"]);
		}
	}

	protected virtual void HandleOtherRequest(Dictionary<string, object> contents) {
		string name = (string)contents["name"];
		if (registeredOtherEvents.ContainsKey(name)) {
			registeredOtherEvents[name].Invoke(contents);
			if (contents.ContainsKey("deleteAfterExe")) {
				if ((bool)contents["deleteAfterExe"]) {
					registeredOtherEvents.Remove(name);
				}
			}
		}
	}

	public enum CommunicationType {
		ACHIEVEMENT,
		MESSAGE,
		MISSION,
		OTHER
	}
}
