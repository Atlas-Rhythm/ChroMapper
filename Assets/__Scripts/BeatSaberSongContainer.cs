using UnityEngine;
using UnityEngine.SceneManagement;

public class BeatSaberSongContainer : MonoBehaviour {
    public static BeatSaberSongContainer Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public BeatSaberSong song;
    public BeatSaberSong.DifficultyBeatmap difficultyData;
    public AudioClip loadedSong;
    public BeatSaberMap map;

    public void SelectSongForEditing(BeatSaberSong song) {
        this.song = song;
        AnimationTransitionManager.instance.TransitionAway(() =>
        {
            SceneManager.LoadSceneAsync("02_SongEditMenu");
        });
        //SceneTransitionManager.Instance.LoadScene("02_SongEditMenu");
    }
}
