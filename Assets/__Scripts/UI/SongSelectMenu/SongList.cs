using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SongList : MonoBehaviour {

    public enum SortingOption
    {
        [Description("Song Name")]
        SONG,
        [Description("Artist Name")]
        ARTIST,
        [Description("Last Modified")]
        MODIFIED
    }
    private SortingOption lastSortingOption = SortingOption.SONG;

    [SerializeField]
    TMP_InputField searchField;

    [SerializeField]
    EnumPicker sortingOptions;

    [SerializeField]
    Transform itemContainer;

    [SerializeField]
    SongListItem songListItemPrefrab;

    private List<SongListItem> items = new List<SongListItem>();
    private Stack<SongListItem> unusedItems = new Stack<SongListItem>();

    [SerializeField]
    public List<BeatSaberSong> songs = new List<BeatSaberSong>();
    
    [SerializeField]
    LocalizeStringEvent songLocationToggleText;

    public bool WIPLevels = true;
    public bool FilteredBySearch = false;

    private void Start()
    {
        //WIPLevels = lastVisited_WasWIP;
        RefreshSongList(false);
        sortingOptions.Initialize(typeof(SortingOption));
        sortingOptions.onClick += SortBy;
    }

    public void ToggleSongLocation()
    {
        WIPLevels = !WIPLevels;
        //lastVisited_WasWIP = WIPLevels;
        RefreshSongList(true);
    }

    public void RefreshSongList(bool search) {
        songLocationToggleText.StringReference.TableEntryReference = WIPLevels ? "custom" : "wip";

        FilteredBySearch = search;
        string[] directories;
        directories = Directory.GetDirectories(WIPLevels ? Settings.Instance.CustomWIPSongsFolder : Settings.Instance.CustomSongsFolder);
        songs.Clear();
        foreach (var dir in directories)
        {
            BeatSaberSong song = BeatSaberSong.GetSongFromFolder(dir);
            if (song == null)
            {
                Debug.LogWarning($"No song at location {dir} exists! Is it in a subfolder?");
                /*
                 * Subfolder loading support has been removed for the following:
                 * A) SongCore does not natively support loading from subfolders, only through editing a config file
                 * B) OneClick no longer saves to a subfolder
                 */
                /*if (dir.ToUpper() == "CACHE") continue; //Ignore the cache folder
                //Get songs from subdirectories
                string[] subDirectories = Directory.GetDirectories(dir);
                foreach (var subDir in subDirectories)
                {
                    song = BeatSaberSong.GetSongFromFolder(subDir);
                    if (song != null) songs.Add(song);
                }*/
            }
            else
            {
                songs.Add(song);
            }
        }
        //Sort by song name, and filter by search text.
        if (FilteredBySearch)
            songs = songs.Where(x => searchField.text != "" ? x.songName.AllIndexOf(searchField.text).Any() : true).ToList();
        SortBy(lastSortingOption);
    }

    public void SortBy(Enum sortingOption)
    {
        lastSortingOption = (SortingOption) sortingOption;
        switch (lastSortingOption)
        {
            case SortingOption.SONG:
                songs = songs.OrderBy(x => x.songName).ToList();
                break;
            case SortingOption.ARTIST:
                songs = songs.OrderBy(x => x.songAuthorName).ToList();
                break;
            case SortingOption.MODIFIED:
                songs = songs.OrderByDescending(x => Directory.GetLastWriteTime(x.directory)).ToList();
                break;
        }
        UpdateList();
    }

    public void UpdateList()
    {
        RecycleItems();
        foreach (BeatSaberSong song in songs)
        {
            AddItem(song);
        }
    }

    private void RecycleItems()
    {
        foreach(SongListItem item in items)
        {
            item.gameObject.SetActive(false);
            unusedItems.Push(item);
        }
        items.Clear();
    }

    private void AddItem(BeatSaberSong song)
    {
        SongListItem item = null;
        if(unusedItems.Count > 0)
        {
            item = unusedItems.Pop();
            item.transform.SetAsLastSibling();
            item.gameObject.SetActive(true);
        }
        else
        {
            item = Instantiate(songListItemPrefrab, itemContainer);
        }
        if (item == null)
            throw new System.Exception("Could not create SongListItem");
        items.Add(item);
        item.AssignSong(song);
    }
}
