using System;
using System.ComponentModel;
using UnityEngine;

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
    private LightingMode currentMode;

    void Start()
    {
        lightingPicker.Initialize(typeof(LightingMode));
        lightingPicker.onClick += UpdateMode;
    }

    public void SetMode(Enum lightingMode)
    {
        lightingPicker.Select(lightingMode);
        UpdateMode(lightingMode);
    }

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
