using UnityEngine;
using UnityEngine.UI;

public class ColorTypeController : MonoBehaviour
{
    [SerializeField] private NotePlacement notePlacement;
    [SerializeField] private Outline leftSelected;
    [SerializeField] private Outline rightSelected;

    void OnEnable()
    {
        UpdateUI();
    }

    public void RedNote(bool active)
    {
        if (active) UpdateValue(BeatmapNote.NOTE_TYPE_A);
    }

    public void BlueNote(bool active)
    {
        if (active) UpdateValue(BeatmapNote.NOTE_TYPE_B);
    }

    public void UpdateValue(int type)
    {
        notePlacement.UpdateType(type);
        UpdateUI();
    }

    public void UpdateUI()
    {
        leftSelected.enabled = notePlacement.queuedData._type == BeatmapNote.NOTE_TYPE_A;
        rightSelected.enabled = notePlacement.queuedData._type == BeatmapNote.NOTE_TYPE_B;
    }
}
