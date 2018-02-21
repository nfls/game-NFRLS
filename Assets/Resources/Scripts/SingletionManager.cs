using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletionManager : MonoBehaviour {

	static Camera m_mainCamera;
	static Camera m_itemCamera;
	static GameObject m_singletionCanvas;

	public static Camera mainCamera {
		set {
			m_mainCamera = value;
		}
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
	public static Canvas[] allCanvases;

	public static List<ISingletionBehaviour> singletionBehaviours = new List<ISingletionBehaviour>();

	void Start() {
		itemCamera.gameObject.SetActive(false);

		singletionBehaviours.AddRange(GetComponentsInChildren<ISingletionBehaviour>());
		singletionBehaviours.Add(new CommunicationController());

		foreach (ISingletionBehaviour sb in singletionBehaviours) {
			sb.Init();
		}

		DontDestroyOnLoad(gameObject);

		SceneManager.activeSceneChanged += SwitchScene;
	}

	public static T GetSingletionBehaviour<T>() where T : ISingletionBehaviour {
		foreach (ISingletionBehaviour mb in singletionBehaviours) {
			if (mb.GetType().Equals(typeof(T))) {
				return (T)mb;
			}
		}
		return default(T);
	}

	public static void SwitchScene(Scene oldScene, Scene newScene) {
		mainCamera = Camera.main;
		GetSingletionBehaviour<CommunicationController>().Init();
	}

	public static void HideAllUI() {
		allCanvases = FindObjectsOfType<Canvas>();
		foreach (Canvas canvas in allCanvases) {
			canvas.gameObject.SetActive(false);
		}
		Cursor.visible = false;
	}

	public static void ShowAllUI() {
		foreach (Canvas canvas in allCanvases) {
			canvas.gameObject.SetActive(true);
		}
		Cursor.visible = true;
	}

	void OnApplicationFocus(bool focus) {
		Cursor.lockState = CursorLockMode.Locked;
	}
}
