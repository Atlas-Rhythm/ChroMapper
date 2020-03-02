﻿using TMPro;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EventAppearanceSO", menuName = "Map/Appearance/Event Appearance SO")]
public class EventAppearanceSO : ScriptableObject
{
    [SerializeField] private Vector3 FlashShaderOffset;
    [SerializeField] private Vector3 FadeShaderOffset;
    [Space(5)]
    [SerializeField] private GameObject LaserSpeedPrefab;
    [Space(5)]
    [Header("Default Colors")]
    [SerializeField] private Color RedColor;
    [SerializeField] private Color BlueColor;
    [SerializeField] private Color OffColor;
    [Header("Other Event Colors")]
    [SerializeField] private Color RingEventsColor;
    [Tooltip("Example: Ring rotate/Ring zoom/Light speed change events")]
    [SerializeField] private Color OtherColor;

    public void SetEventAppearance(BeatmapEventContainer e, bool final = true) {
        Color color = Color.white;
        e.UpdateAlpha(final ? 1.0f : 0.6f);
        e.UpdateScale(final ? 0.75f : 0.6f);
        foreach (TextMeshProUGUI t in e.GetComponentsInChildren<TextMeshProUGUI>()) Destroy(t.transform.parent.gameObject);
        if (e.eventData.IsRotationEvent || e.eventData.IsLaserSpeedEvent || e.eventData.IsBPMChangeEvent)
        {
            GameObject instantiate = Instantiate(LaserSpeedPrefab, e.transform);
            Canvas canvas = instantiate.GetComponentInChildren<Canvas>();
            canvas.sortingLayerName = "Default";
            instantiate.transform.localPosition = new Vector3(0, 0.25f, 0);
            TextMeshProUGUI text = instantiate.GetComponentInChildren<TextMeshProUGUI>();
            if (e.eventData.IsRotationEvent)
            {
                int? rotation = e.eventData.GetRotationDegreeFromValue();
                text.text = rotation != null ? $"{rotation}°" : "Invalid Rotation";
            }
            else if (e.eventData.IsBPMChangeEvent)
            {
                text.text = $"{e.eventData._value} BPM";
            }
            else
            {
                text.text = e.eventData._value.ToString();
            }
            text.rectTransform.localScale = Vector3.one * (2f / 3);
        }
        if (e.eventData.IsUtilityEvent)
        {
            if (e.eventData.IsRingEvent) e.ChangeColor(RingEventsColor);
            else e.ChangeColor(OtherColor);
            e.UpdateOffset(Vector3.zero);
            return;
        }
        else
        {
            if (e.eventData._value >= ColourManager.RGB_INT_OFFSET)
            {
                color = ColourManager.ColourFromInt(e.eventData._value);
                e.UpdateAlpha(final ? 0.9f : 0.6f);
            }
            else if (e.eventData._value <= 3)
            {
                if (BeatSaberSongContainer.Instance.difficultyData.envColorRight != BeatSaberSong.DEFAULT_RIGHTCOLOR)
                    color = BeatSaberSongContainer.Instance.difficultyData.envColorRight;
                else color = BlueColor;
            }
            else if (e.eventData._value <= 7 && e.eventData._value >= 5)
            {
                if (BeatSaberSongContainer.Instance.difficultyData.envColorLeft != BeatSaberSong.DEFAULT_LEFTCOLOR)
                    color = BeatSaberSongContainer.Instance.difficultyData.envColorLeft;
                else color = RedColor;
            }
            else if (e.eventData._value == 4) color = OffColor;
        }
        e.ChangeColor(color);
        switch (e.eventData._value)
        {
            case MapEvent.LIGHT_VALUE_OFF:
                e.ChangeColor(OffColor);
                e.UpdateOffset(Vector3.zero);
                break;
            case MapEvent.LIGHT_VALUE_BLUE_ON:
                e.UpdateOffset(Vector3.zero);
                break;
            case MapEvent.LIGHT_VALUE_BLUE_FLASH:
                e.UpdateOffset(FlashShaderOffset);
                break;
            case MapEvent.LIGHT_VALUE_BLUE_FADE:
                e.UpdateOffset(FadeShaderOffset);
                break;
            case MapEvent.LIGHT_VALUE_RED_ON:
                e.UpdateOffset(Vector3.zero);
                break;
            case MapEvent.LIGHT_VALUE_RED_FLASH:
                e.UpdateOffset(FlashShaderOffset);
                break;
            case MapEvent.LIGHT_VALUE_RED_FADE:
                e.UpdateOffset(FadeShaderOffset);
                break;
        }
    }
}
