using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLSceneController : MonoBehaviour {

	public GameObject fireworksGroup;
	public GameObject celebrationText;

	public float textRiseSpeed = 1f;
	public float cameraMoveBackSpeed = 2f;
	public float cameraRotateUpSpeed = 2f;
	public float duration = 5f;

	Camera mainCamera;

	void Start() {
		mainCamera = Camera.main;

		fireworksGroup.SetActive(false);
		celebrationText.SetActive(false);
	}

	public void PlayA() {
		fireworksGroup.SetActive(true);
	}

	public void PlayB() {
		celebrationText.SetActive(true);
		StartCoroutine(ExeTask());
	}

	IEnumerator ExeTask() {
		while (duration > 0) {
			Vector3 position = celebrationText.transform.position;
			celebrationText.transform.Translate(0, textRiseSpeed * Time.deltaTime, 0);
			position = mainCamera.transform.position;
			mainCamera.transform.Translate(0, 0, -cameraMoveBackSpeed * Time.deltaTime);
			mainCamera.transform.Rotate(-cameraRotateUpSpeed * Time.deltaTime, 0, 0);
			duration -= Time.deltaTime;
			yield return 0;
		}
	}
}
