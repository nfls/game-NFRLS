using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	bool hasTriggerd;
	Collider lastTouch;
	Collider lastLastTouch;
	int hitTimes;

	void OnCollisionEnter(Collision collision) {
		Collider touch = collision.collider;
		if (lastLastTouch) {
			if (lastLastTouch.Equals(touch)) {
				hitTimes++;
				if (hitTimes >= 10) {
					if (!hasTriggerd) {
						hasTriggerd = true;
						NotificationManager.AddNotification(NotificationInfo.NotificationType.SIGN, "Achievement Unlocked", "Simple Harmonic Motion!");
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
