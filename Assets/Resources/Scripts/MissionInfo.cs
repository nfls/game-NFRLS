using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionInfo {

	public int id;
	public string title;
	public string substitle;
	public string description;
	public string tip;
	public bool inProgress;
	public bool finished {
		set {
			m_finished = value;
		}
		get {
			return m_finished;
		}
	}

	bool m_finished;

	void OnFinished() {

	}
}