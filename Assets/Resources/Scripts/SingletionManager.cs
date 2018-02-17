using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletionManager : MonoBehaviour {

	static Camera m_mainCamera;
	static Camera m_itemCamera;
	static GameObject m_singletionCanvas;

	public static Camera mainCamera {
		get {
			if (!m_mainCamera) {
				m_mainCamera = Camera.main;
			}
			return m_mainCamera;
		}
	}
	public static Camera itemCamera {
		get {
			if (!m_itemCamera) {
				m_itemCamera = GameObject.Find("Item Camera").GetComponent<Camera>();
			}
			return m_itemCamera;
		}
	}
	public static GameObject singletionCanvas {
		get {
			if (!m_singletionCanvas) {
				m_singletionCanvas = GameObject.Find("Singletion Canvas");
			}
			return m_singletionCanvas;
		}
	}

	public static List<MonoBehaviour> singletionBehaviours = new List<MonoBehaviour>();

	void Start() {
		itemCamera.gameObject.SetActive(false);

		singletionBehaviours.AddRange(GetComponentsInChildren<MonoBehaviour>());
		foreach (MonoBehaviour mb in singletionBehaviours) {
			if (mb is ISingletionBehaviour) {
				((ISingletionBehaviour)mb).Init();
			}
		}

		DontDestroyOnLoad(gameObject);
	}

	public static T GetSingletionBehaviour<T>() where T : MonoBehaviour {
		foreach (MonoBehaviour mb in singletionBehaviours) {
			if (mb.GetType().Equals(typeof(T))) {
				return (T)mb;
			}
		}
		return null;
	}
}
