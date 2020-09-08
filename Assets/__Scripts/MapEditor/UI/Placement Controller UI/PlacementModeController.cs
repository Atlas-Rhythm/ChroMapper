using System;
using System.ComponentModel;
using UnityEngine;

public class PlacementModeController : MonoBehaviour
{
    public enum PlacementMode
    {
        [Description("Note")]
        NOTE,
        [Description("Bomb")]
        BOMB,
        [Description("Wall")]
        WALL,
        [Description("Delete")]
        DELETE
    }

    [SerializeField] private NotePlacement notePlacement;
    [SerializeField] private BombPlacement bombPlacement;
    [SerializeField] private ObstaclePlacement obstaclePlacement;
    [SerializeField] private DeleteToolController deleteToolController;

    [SerializeField] private EnumPicker modePicker;
    
    void Start()
    {
        modePicker.Initialize(typeof(PlacementMode));
        modePicker.onClick += UpdateMode;
        UpdateMode(PlacementMode.NOTE);
    }

    public void SetMode(Enum placementMode)
    {
        modePicker.Select(placementMode);
        UpdateMode(placementMode);
    }

    private void UpdateMode(Enum placementMode)
    {
        PlacementMode mode = (PlacementMode)placementMode;
        notePlacement.IsActive = mode == PlacementMode.NOTE;
        bombPlacement.IsActive = mode == PlacementMode.BOMB;
        obstaclePlacement.IsActive = mode == PlacementMode.WALL;
        deleteToolController.UpdateDeletion(mode == PlacementMode.DELETE);
    }
}
