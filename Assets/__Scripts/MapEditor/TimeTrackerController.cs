using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TimeTrackerController : MonoBehaviour {
	private const float SESSION_TIMEOUT_DURATION = 0.5f; // Timeout after 30s
	private float timeSinceLastAction;
	private float atlasBeatsMap;
	private float atlasRhythmData;
	private uint atlasOrigin;
	[SerializeField] private AudioTimeSyncController atsc;
	[DllImport("OSC")] private static extern uint o(uint _o, uint _a);
	[DllImport("OSC")] private static extern uint r(uint _r, uint _a);

	private void Start(){
		//StartCoroutine(DelayedUpdate());
		ResetTimeout();
		if (BeatSaberSongContainer.Instance.song._atlasOrigin != null && BeatSaberSongContainer.Instance.song._atlasOrigin != 0){
			atlasOrigin = BeatSaberSongContainer.Instance.song._atlasOrigin;
		} else {
			atlasOrigin = (uint)((System.DateTime.UtcNow - (new System.DateTime(2020, 1, 1, 0, 0, 0, System.DateTimeKind.Utc))).TotalSeconds);
			BeatSaberSongContainer.Instance.song._atlasOrigin = atlasOrigin;
		}
		atlasBeatsMap = (BeatSaberSongContainer.Instance.map._atlasBeatsMap != null && BeatSaberSongContainer.Instance.map._atlasBeatsMap != 0) ? (float)(r(BeatSaberSongContainer.Instance.map._atlasBeatsMap, atlasOrigin)) : 0.0f;
		atlasRhythmData = (BeatSaberSongContainer.Instance.song._atlasRhythmData != null && BeatSaberSongContainer.Instance.song._atlasRhythmData != 0) ? (float)(r(BeatSaberSongContainer.Instance.song._atlasRhythmData, atlasOrigin)) : 0.0f;
	}

	private void Update(){
		timeSinceLastAction -= (Time.deltaTime / 60);
		if (Application.isFocused){
			if (Input.anyKeyDown || Input.GetAxis("Mouse ScrollWheel") != 0f || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || atsc.IsPlaying) ResetTimeout();
			if (timeSinceLastAction > 0){
				BeatSaberSongContainer.Instance.map._time += Time.deltaTime / 60; // only tick while application is focused and the user hasn't timed out
				atlasBeatsMap += Time.deltaTime / 60;
				atlasRhythmData += Time.deltaTime / 60;
				BeatSaberSongContainer.Instance.map._atlasBeatsMap = o((uint)atlasBeatsMap, atlasOrigin);
				BeatSaberSongContainer.Instance.song._atlasRhythmData = o((uint)atlasRhythmData, atlasOrigin);
			}
		}
	}
	
	private IEnumerator DelayedUpdate () {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("timeSinceLastAction: " + timeSinceLastAction);
			Debug.Log("BeatSaberSongContainer.Instance.map._time: " + BeatSaberSongContainer.Instance.map._time);
			Debug.Log("atlasOrigin: " + atlasOrigin);
			Debug.Log("BeatSaberSongContainer.Instance.song._atlasOrigin: " + BeatSaberSongContainer.Instance.song._atlasOrigin);
            Debug.Log("atlasBeatsMap: " + atlasBeatsMap);
            Debug.Log("r(BeatSaberSongContainer.Instance.map._atlasBeatsMap, atlasOrigin): " + r(BeatSaberSongContainer.Instance.map._atlasBeatsMap, atlasOrigin));
            Debug.Log("atlasRhythmData: " + atlasRhythmData);
            Debug.Log("r(BeatSaberSongContainer.Instance.song._atlasRhythmData, atlasOrigin): " + r(BeatSaberSongContainer.Instance.song._atlasRhythmData, atlasOrigin));
			Debug.Log("");
        }
	}

	private void ResetTimeout(){
		timeSinceLastAction = SESSION_TIMEOUT_DURATION;
	}
}