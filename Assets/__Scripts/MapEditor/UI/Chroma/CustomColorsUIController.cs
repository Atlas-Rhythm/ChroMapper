﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomColorsUIController : MonoBehaviour
{
    [SerializeField] private ColorPicker picker;
    [Space]
    [SerializeField] private Image redNote;
    [SerializeField] private Image blueNote;
    [SerializeField] private Image redLight;
    [SerializeField] private Image blueLight;
    [SerializeField] private Image obstacle;
    [Space]
    [SerializeField] private NoteAppearanceSO noteAppearance;
    [SerializeField] private ObstaclesContainer obstacles;
    [SerializeField] private ObstacleAppearanceSO obstacleAppearance;
    [SerializeField] private EventsContainer events;
    [SerializeField] private EventAppearanceSO eventAppearance;

    private PlatformDescriptor platform;
    private Color oldPlatformColorR;
    private Color oldPlatformColorB;

    // Start is called before the first frame update
    void Start()
    {
        LoadInitialMap.PlatformLoadedEvent += LoadedPlatform;
        LoadInitialMap.LevelLoadedEvent += LevelLoaded;
        redNote.color = BeatSaberSongContainer.Instance.difficultyData.colorLeft;
        blueNote.color = BeatSaberSongContainer.Instance.difficultyData.colorRight;
        redLight.color = BeatSaberSongContainer.Instance.difficultyData.envColorLeft;
        blueLight.color = BeatSaberSongContainer.Instance.difficultyData.envColorRight;
        obstacle.color = BeatSaberSongContainer.Instance.difficultyData.obstacleColor;
    }

    private void LevelLoaded()
    {
        noteAppearance.UpdateColor(redNote.color, blueNote.color);
        foreach (BeatmapObjectContainer con in BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers)
        {
            BeatmapNote note = (con.objectData as BeatmapNote);
            if (note._type != BeatmapNote.NOTE_TYPE_BOMB)
            {
                Color color = note._type == BeatmapNote.NOTE_TYPE_A ? redNote.color : blueNote.color;
                if (note._customData?.HasKey("_color") ?? false)
                {
                    color = note._customData["_color"];
                }
                (con as BeatmapNoteContainer).SetColor(color);
            }
        }
        foreach (BeatmapObjectContainer con in events.LoadedContainers)
            eventAppearance.SetEventAppearance(con as BeatmapEventContainer, true, platform);
        foreach (BeatmapObjectContainer con in obstacles.LoadedContainers)
            obstacleAppearance.SetObstacleAppearance(con as BeatmapObstacleContainer, platform);
    }

    private void LoadedPlatform(PlatformDescriptor obj)
    {
        platform = obj;
        oldPlatformColorR = platform.RedColor;
        oldPlatformColorB = platform.BlueColor;
        SetColorIfNotEqual(ref redNote, platform.RedNoteColor);
        SetColorIfNotEqual(ref blueNote, platform.BlueNoteColor);
        SetColorIfNotEqual(ref redLight, platform.RedColor);
        SetColorIfNotEqual(ref blueLight, platform.BlueColor);
        SetColorIfNotEqual(ref obstacle, platform.ObstacleColor);
    }

    private void SetColorIfNotEqual(ref Image a, Color b)
    {
        if (a.color != b) a.color = b;
    }

    private void OnDestroy()
    {
        LoadInitialMap.PlatformLoadedEvent -= LoadedPlatform;
        LoadInitialMap.LevelLoadedEvent -= LevelLoaded;
    }

    public void UpdateRedNote()
    {
        redNote.color = picker.CurrentColor;
        BeatSaberSongContainer.Instance.difficultyData.colorLeft = picker.CurrentColor;
        noteAppearance.UpdateColor(picker.CurrentColor, blueNote.color);
        foreach (BeatmapObjectContainer con in BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers)
        {
            BeatmapNote note = (con.objectData as BeatmapNote);
            if (note._type == BeatmapNote.NOTE_TYPE_A)
            {
                Color color = redNote.color;
                if (note._customData?.HasKey("_color") ?? false)
                {
                    color = note._customData["_color"];
                }
                (con as BeatmapNoteContainer).SetColor(color);
            }
        }
    }

    public void UpdateBlueNote()
    {
        blueNote.color = picker.CurrentColor;
        BeatSaberSongContainer.Instance.difficultyData.colorRight = picker.CurrentColor;
        noteAppearance.UpdateColor(redNote.color, picker.CurrentColor);
        foreach (BeatmapObjectContainer con in BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers)
        {
            BeatmapNote note = (con.objectData as BeatmapNote);
            if (note._type == BeatmapNote.NOTE_TYPE_B)
            {
                Color color = blueNote.color;
                if (note._customData?.HasKey("_color") ?? false)
                {
                    color = note._customData["_color"];
                }
                (con as BeatmapNoteContainer).SetColor(color);
            }
        }
    }

    public void UpdateRedLight()
    {
        redLight.color = picker.CurrentColor;
        BeatSaberSongContainer.Instance.difficultyData.envColorLeft = picker.CurrentColor;
        platform.RedColor = picker.CurrentColor;
        foreach (BeatmapObjectContainer con in events.LoadedContainers)
            eventAppearance.SetEventAppearance(con as BeatmapEventContainer);
    }

    public void UpdateBlueLight()
    {
        blueLight.color = picker.CurrentColor;
        BeatSaberSongContainer.Instance.difficultyData.envColorRight = picker.CurrentColor;
        platform.BlueColor = picker.CurrentColor;
        foreach (BeatmapObjectContainer con in events.LoadedContainers)
            eventAppearance.SetEventAppearance(con as BeatmapEventContainer);
    }

    public void UpdateObstacles()
    {
        obstacle.color = picker.CurrentColor;
        BeatSaberSongContainer.Instance.difficultyData.obstacleColor = picker.CurrentColor;
        obstacleAppearance.defaultObstacleColor = picker.CurrentColor;
        foreach (BeatmapObjectContainer con in obstacles.LoadedContainers)
            obstacleAppearance.SetObstacleAppearance(con as BeatmapObstacleContainer);
    }

    public void ResetNotes()
    {
        BeatSaberSongContainer.Instance.difficultyData.colorLeft = BeatSaberSong.DEFAULT_LEFTNOTE;
        BeatSaberSongContainer.Instance.difficultyData.colorRight = BeatSaberSong.DEFAULT_RIGHTNOTE;
        blueNote.color = platform.BlueNoteColor;
        redNote.color = platform.RedNoteColor;
        noteAppearance.UpdateColor(redNote.color, blueNote.color);
        foreach (BeatmapObjectContainer con in BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers)
        {
            BeatmapNote note = (con.objectData as BeatmapNote);
            if (note._type != BeatmapNote.NOTE_TYPE_BOMB)
            {
                (con as BeatmapNoteContainer).SetColor(note._type == BeatmapNote.NOTE_TYPE_A ? redNote.color : blueNote.color);
            }
        }
    }

    public void ResetLights()
    {

        redLight.color = BeatSaberSong.DEFAULT_LEFTCOLOR;
        BeatSaberSongContainer.Instance.difficultyData.envColorLeft = BeatSaberSong.DEFAULT_LEFTCOLOR;
        platform.RedColor = oldPlatformColorR;
        blueLight.color = BeatSaberSong.DEFAULT_RIGHTCOLOR;
        BeatSaberSongContainer.Instance.difficultyData.envColorRight = BeatSaberSong.DEFAULT_RIGHTCOLOR;
        platform.BlueColor = oldPlatformColorB;
        foreach (BeatmapObjectContainer con in events.LoadedContainers)
            eventAppearance.SetEventAppearance(con as BeatmapEventContainer, true, platform);
    }

    public void ResetObstacles()
    {
        obstacle.color = BeatSaberSong.DEFAULT_LEFTCOLOR;
        BeatSaberSongContainer.Instance.difficultyData.obstacleColor = BeatSaberSong.DEFAULT_LEFTCOLOR;
        obstacleAppearance.defaultObstacleColor = BeatSaberSong.DEFAULT_LEFTCOLOR;
        foreach (BeatmapObjectContainer con in obstacles.LoadedContainers)
            obstacleAppearance.SetObstacleAppearance(con as BeatmapObstacleContainer, platform);
    }
}
