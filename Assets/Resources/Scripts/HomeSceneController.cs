using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Video;

public class HomeSceneController : MonoBehaviour {

	public GameObject player;
	public GameObject portal;
	public GameObject screen;

	public VideoClip plotVideo;
	public VideoClip[] randomVideos;

	VideoPlayer videoPlayer;

	FirstPersonController fpsController;

	public int currentPhase = 0;

	void Start() {

		videoPlayer = screen.GetComponent<VideoPlayer>();

		PlayPhase1();
	}

	void Update() {

	}

	void PlayPhase1() {
		currentPhase = 1;
		portal.SetActive(false);
	}

	void PlayPhase2() {
		currentPhase = 2;
	}

	void PlayPhase3() {
		currentPhase = 3;
	}
}
