using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour, ISingletionBehaviour {

	static GameObject notificationBar;
	static NotificationManager notificationManager;
	static Queue<NotificationInfo> notifications;
	static Sprite[] notificationIcons;

	public void Init() {
		notificationManager = SingletionManager.GetSingletionBehaviour<NotificationManager>();
		notifications = new Queue<NotificationInfo>();
		if (notificationBar) {
			Destroy(notificationBar);
		}
		notificationBar = Resources.Load<GameObject>("Prefabs/Notification Bar");
		notificationBar = Instantiate(notificationBar, SingletionManager.singletionCanvas.transform);
		notificationBar.SetActive(false);
		notificationIcons = new Sprite[5];
		notificationIcons[0] = Resources.Load<Sprite>("Textures/icon_mission");
		notificationIcons[1] = Resources.Load<Sprite>("Textures/icon_operation");
		notificationIcons[2] = Resources.Load<Sprite>("Textures/icon_sign");
		notificationIcons[3] = Resources.Load<Sprite>("Textures/icon_tip");
		notificationIcons[4] = Resources.Load<Sprite>("Textures/icon_warning");
		System.GC.Collect();
	}

	static int AddNotification(NotificationInfo info) {
		notifications.Enqueue(info);
		if (notifications.Count == 1) {
			DisplayNewNotification(notifications.Peek());
		}
		return notifications.Count;
	}

	public static int AddNotification(string content) {
		return AddNotification(NotificationInfo.NotificationType.TIP, content);
	}

	public static int AddNotification(NotificationInfo.NotificationType type, string content) {
		return AddNotification(type, content, NotificationInfo.DURATION_DEFAULT);
	}

	public static int AddNotification(NotificationInfo.NotificationType type, string title, string content) {
		float duration = 0f;
		switch (type) {
			case NotificationInfo.NotificationType.MISSION: duration = NotificationInfo.DURATION_LONG; break;
			case NotificationInfo.NotificationType.OPERATION: duration = NotificationInfo.DURATION_SHORT; break;
			case NotificationInfo.NotificationType.SIGN: duration = NotificationInfo.DURATION_MEDIUM; break;
			case NotificationInfo.NotificationType.TIP: duration = NotificationInfo.DURATION_DEFAULT; break;
			case NotificationInfo.NotificationType.WARNING: duration = NotificationInfo.DURATION_LONG; break;
		}
		return AddNotification(type, title, content, NotificationInfo.TRANSITION_MEDIUM, NotificationInfo.DURATION_DEFAULT);
	}

	public static int AddNotification(NotificationInfo.NotificationType type, string content, float duration) {
		string title = type.ToString();
		title = title.Substring(0, 1) + title.Substring(1).ToLower();
		return AddNotification(type, title, content, NotificationInfo.TRANSITION_MEDIUM, duration);
	}

	public static int AddNotification(NotificationInfo.NotificationType type, string title, string content, float duration) {
		return AddNotification(type, title, content, NotificationInfo.TRANSITION_MEDIUM, duration);
	}

	public static int AddNotification(NotificationInfo.NotificationType type, string title, string content, float transition, float duration) {
		return AddNotification(new NotificationInfo(type, title, content, transition, duration));
	}

	static void DisplayNewNotification(NotificationInfo info) {
		RefreshNotificationBar(info);
		notificationManager.StartCoroutine(ExeDisplayTask(info.transition, info.duration));
	}

	static void RefreshNotificationBar(NotificationInfo info) {
		notificationBar.transform.Find("Icon Panel/Image").GetComponent<Image>().sprite = notificationIcons[(int)info.type];
		notificationBar.transform.Find("Body Panel/Title Text").GetComponent<Text>().text = info.title;
		notificationBar.transform.Find("Body Panel/Content Text").GetComponent<Text>().text = info.content;
	}

	static IEnumerator ExeDisplayTask(float transition, float duration) {
		notificationBar.SetActive(true);
		CanvasGroup canvasGroup = notificationBar.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		if (transition > -float.Epsilon && transition < float.Epsilon) {
			canvasGroup.alpha = 1;
		} else {
			while (canvasGroup.alpha < 1f) {
				yield return 0;
				canvasGroup.alpha += Time.deltaTime / transition;
			}
		}
		yield return new WaitForSeconds(duration);
		notifications.Dequeue();
		notificationBar.SetActive(false);
		if (notifications.Count > 0) {
			DisplayNewNotification(notifications.Peek());
		}
	}
}
