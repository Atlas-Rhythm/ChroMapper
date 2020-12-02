using UnityEngine;
using UnityEngine.InputSystem;

public class LightshowController : MonoBehaviour, CMInput.ILightshowActions
{
    [SerializeField] private GameObject[] ThingsToToggle;
    [SerializeField] private CameraController cameraController;

    private bool showObjects = true;

    void Start()
    {
        Settings.NotifyBySettingName("Lightshow", (val) => UpdateLightshow((bool)val));
        if(Settings.NonPersistentSettings.TryGetValue("Lightshow", out object value))
            UpdateLightshow((bool)value);
    }

    void OnDestroy()
    {
        Settings.ClearSettingNotifications("Lightshow");
    }

    public void UpdateLightshow(bool enabled)
    {
        showObjects = enabled;
        foreach (GameObject obj in ThingsToToggle) obj.SetActive(enabled);
    }

    public void OnToggleLightshowMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (cameraController.transform.parent != null)
            {
                cameraController.OnAttachtoNoteGrid(context);
            }
            UpdateLightshow(!showObjects);
        }
    }
}
