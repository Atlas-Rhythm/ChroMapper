using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TimeTrackerController : MonoBehaviour {
	private const float SESSION_TIMEOUT_DURATION = 5.0f; // Timeout after 5 minutes
	private float timeSinceLastAction;
	private float bongoTrackerTotal; // Hacky fix for Bongo Cat intermittently causing serious crashes o.O
	[SerializeField] private AudioTimeSyncController atsc;
	[DllImport("OSC")] private static extern uint o(uint _o);
	[DllImport("OSC")] private static extern uint r(uint _r);

	private void Start(){
		ResetTimeout();
		bongoTrackerTotal = (float)(r(BeatSaberSongContainer.Instance.map._bongoHash));
	}

	private void Update(){
		timeSinceLastAction -= (Time.deltaTime / 60);
		if (Application.isFocused){
			if (Input.anyKeyDown || atsc.IsPlaying) ResetTimeout();
			if (timeSinceLastAction > 0){
				BeatSaberSongContainer.Instance.map._time += Time.deltaTime / 60; // only tick while application is focused and the user hasn't timed out
				bongoTrackerTotal += Time.deltaTime / 60; // yes, and the cat too
				BeatSaberSongContainer.Instance.map._bongoHash = o((uint)bongoTrackerTotal);
			}
		}
	}

	private void ResetTimeout(){
		timeSinceLastAction = SESSION_TIMEOUT_DURATION;
	}
}