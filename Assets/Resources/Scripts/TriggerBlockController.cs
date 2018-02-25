using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBlockController : MonoBehaviour {

	public bool triggerOnce;
	public bool triggerCollider = true;
	public bool triggerCharacter = true;
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

	protected virtual void OnTriggerEnter(Collider other) {
		if (triggerCollider) {
			return;
		}
		Process();
	}

	protected virtual void OnControllerColliderHit(ControllerColliderHit hit) {
		if (triggerCharacter) {
			return;
		}
		Process();
	}

	protected virtual void Process() {
		if (willSendMessageRequest) {
			CommunicationRequest request = new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.MESSAGE,
				contents = new Dictionary<string, object> {
					{"text", messageText},
					{"color", messageColor},
					{"duration", messageDuration}
				}
			};

			CommunicationController.HandleCommunicationRequest(request);
		}
		if (willSendMissionRequest) {
			CommunicationRequest request = new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.MISSION,
				contents = new Dictionary<string, object> {
					{"id", missionId},
					{"status", missionStatus}
				}
			};
			if (!missionStatus.Equals("finished")) {
				request.contents["missionInfo"] = new MissionInfo {
					id = missionId,
					title = missionTitle,
					substitle = missionSubtitle,
					description = missionDescription,
					tip = missionTip
				};
			}

			CommunicationController.HandleCommunicationRequest(request);
		}
		if (willSendOtherRequest) {

			CommunicationController.HandleCommunicationRequest(new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.OTHER,
				contents = new Dictionary<string, object> {
					{"name", actionName},
					{"deleteAfterExe", deleteAfterExe}
				}
			});
		}
		if (triggerOnce) {
			triggerCollider = false;
			triggerCharacter = false;
			Destroy(this);
		}
	}
}