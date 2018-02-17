using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {

	GameObject minimapMask;
	bool hidden;

	void Start() {
		minimapMask = transform.Find("Minimap Mask").gameObject;
	}

	void Update() {
		if (Input.GetButtonDown("Switch Minimap")) {
			minimapMask.SetActive(hidden);
			hidden = !hidden;
		}
	}
}
