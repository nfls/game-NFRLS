using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationInfo {

	public static readonly float TRANSITION_NONE = 0f;
	public static readonly float TRANSITION_SHORT = 0.5f;
	public static readonly float TRANSITION_MEDIUM = 1f;
	public static readonly float TRANSITION_LONG = 1.5f;

	public static readonly float DURATION_SHORT = 0.5f;
	public static readonly float DURATION_DEFAULT = 1f;
	public static readonly float DURATION_MEDIUM = 3f;
	public static readonly float DURATION_LONG = 5f;

	public NotificationType type;
	public string title;
	public string content;
	public float transition;
	public float duration;

	public NotificationInfo(NotificationType type, string title, string content, float transition, float duration) {
		this.type = type;
		this.title = title;
		this.content = content;
		this.transition = transition;
		this.duration = duration;
	}

	public enum NotificationType {
		MISSION,
		OPERATION,
		SIGN,
		TIP,
		WARNING
	}
}
