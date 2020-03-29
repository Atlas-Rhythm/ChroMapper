using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TimeTrackerController : MonoBehaviour {
	private const float SESSION_TIMEOUT_DURATION = 2.0f; // Timeout after 2 minutes
	private float timeSinceLastAction;
	private float atlasBeatsMap;
	private float atlasRhythmData;
	[SerializeField] private AudioTimeSyncController atsc;
	[DllImport("OSC")] private static extern uint o(uint _o);
	[DllImport("OSC")] private static extern uint r(uint _r);

	private void Start(){
		ResetTimeout();
		atlasBeatsMap = (float)(r(BeatSaberSongContainer.Instance.map._atlasBeatsMap));
		atlasRhythmData = (float)(r(BeatSaberSongContainer.Instance.song._atlasRhythmData));
	}

	private void Update(){
		timeSinceLastAction -= (Time.deltaTime / 60);
		if (Application.isFocused){
			if (Input.anyKeyDown || Input.GetAxis("Mouse ScrollWheel") != 0f || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || atsc.IsPlaying) ResetTimeout();
			if (timeSinceLastAction > 0){
				BeatSaberSongContainer.Instance.map._time += Time.deltaTime / 60; // only tick while application is focused and the user hasn't timed out
				atlasBeatsMap += Time.deltaTime / 60;
				atlasRhythmData += Time.deltaTime / 60;
				BeatSaberSongContainer.Instance.map._atlasBeatsMap = o((uint)atlasBeatsMap);
				BeatSaberSongContainer.Instance.song._atlasRhythmData = o((uint)atlasRhythmData);
			}
		}
	}

	private void ResetTimeout(){
		timeSinceLastAction = SESSION_TIMEOUT_DURATION;
	}
}