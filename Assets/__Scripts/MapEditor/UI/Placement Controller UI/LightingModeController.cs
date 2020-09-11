﻿using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class LightingModeController : MonoBehaviour
{
    public enum LightingMode
    {
        [Description("On")]
        ON,
        [Description("Off")]
        OFF,
        [Description("Flash")]
        FLASH,
        [Description("Fade")]
        FADE
    }
    [SerializeField] private EnumPicker lightingPicker;
    [SerializeField] private EventPlacement eventPlacement;
    [SerializeField] private NotePlacement notePlacement;
    [SerializeField] private Image modeLock;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    private bool modeLocked;
    private LightingMode currentMode;

    void Start()
    {
        lightingPicker.Initialize(typeof(LightingMode));
        SetLocked(false);
        lightingPicker.onClick += UpdateMode;
    }

    public void SetMode(Enum lightingMode)
    {
        if (modeLocked)
            return;
        lightingPicker.Select(lightingMode);
        UpdateMode(lightingMode);
    }

    public void SetLocked(bool locked)
    {
        modeLocked = locked;
        lightingPicker.locked = modeLocked;
        modeLock.sprite = modeLocked ? lockedSprite : unlockedSprite;
    }

    public void ToggleLock() => SetLocked(!modeLocked);

    public void UpdateValue()
    {
        bool red = notePlacement.queuedData._type == BeatmapNote.NOTE_TYPE_A;
        switch (currentMode)
        {
            case LightingMode.OFF:
                eventPlacement.UpdateValue(MapEvent.LIGHT_VALUE_OFF);
                break;
            case LightingMode.ON:
                eventPlacement.UpdateValue(red ? MapEvent.LIGHT_VALUE_RED_ON : MapEvent.LIGHT_VALUE_BLUE_ON);
                break;
            case LightingMode.FLASH:
                eventPlacement.UpdateValue(red ? MapEvent.LIGHT_VALUE_RED_FLASH : MapEvent.LIGHT_VALUE_BLUE_FLASH);
                break;
            case LightingMode.FADE:
                eventPlacement.UpdateValue(red ? MapEvent.LIGHT_VALUE_RED_FADE : MapEvent.LIGHT_VALUE_BLUE_FADE);
                break;
        }
    }

    private void UpdateMode(Enum lightingMode)
    {
        currentMode = (LightingMode)lightingMode;
        UpdateValue();
    }
}
