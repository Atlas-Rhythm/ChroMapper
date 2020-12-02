using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class KeybindUpdateUIController : MonoBehaviour, CMInput.IWorkflowsActions, CMInput.IEventUIActions
{
    [SerializeField] private PlacementModeController placeMode;
    [SerializeField] private LightingModeController lightMode;
    [SerializeField] private PrecisionStepDisplayController stepController;

    [SerializeField] private Toggle redToggle;
    [SerializeField] private Toggle blueToggle;

    public void OnChangeWorkflows(InputAction.CallbackContext context)
    {
        //this keybind is obsolete
        //if (context.performed) workflowToggle.UpdateWorkflowGroup();
    }

    public void OnPlaceBlueNoteorEvent(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        blueToggle.onValueChanged.Invoke(true);
        placeMode.SetMode(PlacementModeController.PlacementMode.NOTE);
        lightMode.UpdateValue();
    }

    public void OnPlaceRedNoteorEvent(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        redToggle.onValueChanged.Invoke(true);
        placeMode.SetMode(PlacementModeController.PlacementMode.NOTE);
        lightMode.UpdateValue();
    }

    public void OnPlaceBomb(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.BOMB);
    }

    public void OnPlaceObstacle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.WALL);
    }

    public void OnToggleDeleteTool(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.DELETE);
    }

    public void OnUpdateSwingArcVisualizer(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE) as NotesContainer)
            .UpdateSwingArcVisualizer();
    }

    public void OnTypeOn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.NOTE);
        lightMode.SetMode(LightingModeController.LightingMode.ON);
    }

    public void OnTypeFlash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.NOTE);
        lightMode.SetMode(LightingModeController.LightingMode.FLASH);
    }

    public void OnTypeOff(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.NOTE);
        lightMode.SetMode(LightingModeController.LightingMode.OFF);
    }

    public void OnTypeFade(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        placeMode.SetMode(PlacementModeController.PlacementMode.NOTE);
        lightMode.SetMode(LightingModeController.LightingMode.FADE);
    }

    public void OnTogglePrecisionRotation(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        //UpdatePrecisionRotationValue(); todo
    }

    public void OnSwapCursorInterval(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        stepController.SwapSelectedInterval();
    }
}
