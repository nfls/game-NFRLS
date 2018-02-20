using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	bool hasTriggered;
	Collider lastTouch;
	Collider lastLastTouch;
	int hitTimes;

	void OnCollisionEnter(Collision collision) {
		if (hasTriggered) {
			return;
		}
		Collider touch = collision.collider;
		if (lastLastTouch) {
			if (lastLastTouch.Equals(touch)) {
				hitTimes++;
				if (hitTimes >= 10) {
					if (!bool.Parse(PlayerPrefs.GetString("simpleHarmonicMotion", "false"))) {
						PlayerPrefs.SetString("simpleHarmonicMotion", "true");
						NotificationManager.AddNotification(NotificationInfo.NotificationType.SIGN, "Achievement Unlocked", "Simple Harmonic Motion!");
						hasTriggered = true;
					}
				}
			} else {
				hitTimes = 1;
			}
		}
		lastLastTouch = lastTouch;
		lastTouch = touch;
	}
}
