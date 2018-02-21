using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Video;
using System;

public class HomeSceneController : MonoBehaviour {

	public GameObject player;
	public GameObject screen;
	public GameObject bomb;
	public GameObject toyBomb;
	public GameObject door;
	public GameObject ball;

	public Camera tvCamera;

	public GameObject[] phase3Objects;
	public VideoClip[] randomVideos;
	public AudioClip instructionAudio;
	public AudioClip endAudio;
	public AudioClip tvAudio;
	public AudioClip playerAudioAboutNotice;
	public AudioClip bombCountDownAudio;
	public AudioClip bombExplodeAudio;
	public AudioClip toyBombAudio;
	public AudioClip playerOuchAudio;
	public AudioClip switchDoorAudio;
	public AudioClip[] phase1HurryAudios;
	public AudioClip[] phase3HurryAudios;

	public float hurryIntervalMin = 10f;
	public float hurryIntervalMax = 20f;

	public int currentPhase;

	int randomVideoMark;
	VideoPlayer videoPlayer;
	IEnumerator hurryTask;

	void Start() {
		videoPlayer = screen.GetComponent<VideoPlayer>();
		MissionManager.AddNewMission(new MissionInfo {
			id = 1,
			title = "Watch TV",
			substitle = "To kill time",
			description = "The last morning before your high school life, you decided to find some thing to do.",
			tip = "Go to TV and turn it on.",
			finishAction = PlayPhase2
		});
		MissionManager.AddNewMission(new MissionInfo {
			id = 2,
			title = "Leave the Room",
			substitle = "To Go to School",
			description = "It's the time now, find a way to leave this no-door room.",
			tip = "Try to interact with the stuffs in the room and see what you can get.",
			finishAction = PlayPhase4
		});

		CommunicationController.RegisterOtherEvent("Activate Bomb", ActivateBomb);
		CommunicationController.RegisterOtherEvent("Activate Toy Bomb", ActivateToyBomb);
		CommunicationController.RegisterOtherEvent("Switch Door", SwitchDoor);
		CommunicationController.RegisterOtherEvent("Player Ouch", PlayerOuch);

		foreach (GameObject go in phase3Objects) {
			go.SetActive(false);
		}

		if (bool.Parse(PlayerPrefs.GetString("hasPlayed", "false"))) {
			PlayPhase1();
		} else {
			FirstPersonController.controllerEnabled = false;
			AudioSource.PlayClipAtPoint(instructionAudio, SingletionManager.mainCamera.transform.position);
			PlayerPrefs.SetString("hasPlayed", "true");
			Invoke("PlayPhase1", 113f);
		}
	}

	void PlayPhase1() {
		currentPhase = 1;
		FirstPersonController.controllerEnabled = true;
		hurryTask = ExeHurryTask(phase1HurryAudios);
		StartCoroutine(hurryTask);
	}

	void PlayPhase2() {
		currentPhase = 2;
		StopCoroutine(hurryTask);
		FirstPersonController.controllerEnabled = false;
		SingletionManager.mainCamera.gameObject.SetActive(false);
		ball.transform.localPosition = new Vector3(-0.52f, 0.126f, 0.3866f);
		ball.SetActive(false);
		tvCamera.gameObject.SetActive(true);
		SingletionManager.HideAllUI();
		videoPlayer.Play();
		//AudioSource.PlayClipAtPoint(tvAudio, videoPlayer.transform.position);
		videoPlayer.loopPointReached += delegate {
			if (currentPhase != 2) {
				return;
			}
			tvCamera.gameObject.SetActive(false);
			player.transform.localPosition = new Vector3(0.1665f, player.transform.localPosition.y, 0.8391f);
			player.transform.localRotation = Quaternion.Euler(0, 0, 0);
			SingletionManager.mainCamera.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
			SingletionManager.mainCamera.gameObject.SetActive(true);
			StartCoroutine(PlayNoticeEnterAnimation(PlayPhase3));
		};
	}

	void PlayPhase3() {
		currentPhase = 3;
		SingletionManager.ShowAllUI();
		FirstPersonController.controllerEnabled = true;
		ball.SetActive(true);
		videoPlayer.clip = randomVideos[randomVideoMark];
		videoPlayer.Play();
		videoPlayer.loopPointReached += delegate {
			randomVideoMark += 1;
			if (randomVideoMark == randomVideos.Length) {
				randomVideoMark = 0;
			}
			videoPlayer.clip = randomVideos[randomVideoMark];
			videoPlayer.Play();
		};
		foreach (GameObject go in phase3Objects) {
			go.SetActive(true);
		}
		hurryTask = ExeHurryTask(phase3HurryAudios);
	}

	void PlayPhase4() {
		currentPhase = 4;
		StopCoroutine(hurryTask);
		FirstPersonController.controllerEnabled = false;
		StartCoroutine(ExeDemoFinishedTask());
	}

	IEnumerator ExeHurryTask(AudioClip[] audios) {
		while (true) {
			float interval = UnityEngine.Random.Range(hurryIntervalMin, hurryIntervalMax);
			yield return new WaitForSeconds(interval);
			AudioSource.PlayClipAtPoint(audios[UnityEngine.Random.Range(0, audios.Length)], SingletionManager.mainCamera.transform.position);
		}
	}

	IEnumerator ExeDemoFinishedTask() {
		yield return new WaitForSeconds(2f);
		CommunicationController.HandleCommunicationRequest(new CommunicationRequest {
			communicationType = CommunicationController.CommunicationType.MESSAGE,
			contents = new Dictionary<string, object> {
				{"text", "<b>Demo Finished !!!</b>"},
				{"color", Color.green},
				{"duration", CrossHairController.MESSAGE_DURATION_DEFAULT}
			}
		});
		FirstPersonController.controllerEnabled = false;
		AudioSource.PlayClipAtPoint(endAudio, SingletionManager.mainCamera.transform.position);
	}

	IEnumerator PlayNoticeEnterAnimation(Action action) {
		GameObject notice = Resources.Load<GameObject>("Prefabs/Notice");
		notice = Instantiate(notice, SingletionManager.mainCamera.transform);
		yield return 0;
		ItemController controller = notice.GetComponent<ItemController>();
		controller.itemCollider.enabled = false;
		notice.transform.localPosition = new Vector3(0, -0.15f, 0.2f);
		float phaseADuration = 4f;
		float phaseBDuration = 4f;
		float ySpeed = 0.14f / phaseADuration;
		float zSpeed = -0.04f / phaseBDuration;
		float rSpeed = -15f / phaseBDuration;
		while (phaseADuration > 0) {
			yield return 0;
			notice.transform.localPosition += new Vector3(0, ySpeed * Time.deltaTime, 0);
			phaseADuration -= Time.deltaTime;
		}
		while (phaseBDuration > 0) {
			yield return 0;
			notice.transform.localPosition += new Vector3(0, 0, zSpeed * Time.deltaTime);
			notice.transform.Rotate(rSpeed * Time.deltaTime, 0, 0);
			phaseBDuration -= Time.deltaTime;
		}
		yield return new WaitForSeconds(3f);
		AudioSource.PlayClipAtPoint(playerAudioAboutNotice, SingletionManager.mainCamera.transform.position);
		yield return new WaitForSeconds(5f);
		notice.transform.parent = GameObject.Find("bank").transform;
		notice.transform.localPosition = new Vector3(0.17f, 0.131f, 1f);
		notice.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
		controller.itemCollider.enabled = true;
		action.Invoke();
	}

	void PlayerOuch() {
		AudioSource.PlayClipAtPoint(playerOuchAudio, SingletionManager.mainCamera.transform.position);
	}

	void ActivateBomb() {
		StartCoroutine(ExeExplodeTask());
	}

	void ActivateToyBomb() {
		AudioSource.PlayClipAtPoint(toyBombAudio, toyBomb.transform.position);
		toyBomb.GetComponent<ItemController>().itemName = "Toy Bomb";
	}

	IEnumerator ExeExplodeTask() {
		AudioSource.PlayClipAtPoint(bombCountDownAudio, bomb.transform.position);
		yield return new WaitForSeconds(5);
		AudioSource.PlayClipAtPoint(bombExplodeAudio, bomb.transform.position);
		//player.transform.position = new Vector3(1000, 1000, 1000);
		CommunicationController.HandleCommunicationRequest(new CommunicationRequest {
			communicationType = CommunicationController.CommunicationType.MESSAGE,
			contents = new Dictionary<string, object> {
				{"text", "Game Over. Relaunch the program to continue (No time to develop the menu)"},
				{"color", Color.red},
				{"duration", CrossHairController.MESSAGE_DURATION_DEFAULT}
			}
		});
		AudioSource.PlayClipAtPoint(endAudio, SingletionManager.mainCamera.transform.position);
	}

	void SwitchDoor() {
		if (currentPhase != 3) {
			CommunicationController.HandleCommunicationRequest(new CommunicationRequest {
				communicationType = CommunicationController.CommunicationType.MESSAGE,
				contents = new Dictionary<string, object> {
					{"text", "Should work, but not the right time."},
					{"color", Color.black},
					{"duration", CrossHairController.MESSAGE_DURATION_DEFAULT}
				}
			});
			return;
		}
		AudioSource.PlayClipAtPoint(switchDoorAudio, door.transform.position);
		door.SetActive(!door.activeSelf);
	}
}
