using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {

	public GameObject wall;
	public AudioClip enterAudioEffect;
	public AudioClip exitAudioEffect;

	Collider wallCollider;

	bool hasTriggered;
	int playerTravelTimes;

	void Start() {
		wallCollider = wall.GetComponent<Collider>();
	}

	void OnTriggerEnter(Collider other) {
		wallCollider.enabled = false;
		AudioSource.PlayClipAtPoint(enterAudioEffect, transform.position);
	}

	void OnTriggerExit(Collider other) {
		other.transform.SetPositionAndRotation(new Vector3(-5.4f, 0.15f, -10f), other.transform.rotation);
		wallCollider.enabled = true;
		AudioSource.PlayClipAtPoint(exitAudioEffect, transform.position);
		if (other.tag.Equals("Player")) {
			playerTravelTimes += 1;
			if (!hasTriggered && playerTravelTimes > 10) {
				NotificationManager.AddNotification(NotificationInfo.NotificationType.SIGN, "Achievement Unlocked", "Busy Traveller");
			}
		}
	}
}
