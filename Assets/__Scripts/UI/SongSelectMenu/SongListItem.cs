using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SongListItem : MonoBehaviour {

    private static Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();

    [SerializeField]
    Image cover;

    [SerializeField]
    TextMeshProUGUI mainText;

    [SerializeField]
    TextMeshProUGUI subText;

    [SerializeField]
    Button button;

    private BeatSaberSong song;

    public void AssignSong(BeatSaberSong song) {
        this.song = song;
        StopAllCoroutines();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ButtonClicked);
        mainText.text = $"{song.songName.StripTMPTags()} <size=10>{song.songSubName.StripTMPTags()}";
        subText.text = $"{song.songAuthorName.StripTMPTags()}";
        StartCoroutine(LoadImage());
    }

    IEnumerator LoadImage()
    {
        string fullPath = Path.Combine(song.directory, song.coverImageFilename);
        if(cachedSprites.TryGetValue(fullPath, out Sprite existingSprite))
        {
            cover.sprite = existingSprite;
            yield return null;
        }
        else
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture($"file:///{Uri.EscapeDataString($"{fullPath}")}");
            yield return www.SendWebRequest();
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100f);
            cover.sprite = sprite;
            cachedSprites.Add(fullPath, sprite);
        }
    }
    void ButtonClicked() {
        Debug.Log("Edit button for song " + song.songName);
        if (BeatSaberSongContainer.Instance != null && song != null) BeatSaberSongContainer.Instance.SelectSongForEditing(song);
    }

}
