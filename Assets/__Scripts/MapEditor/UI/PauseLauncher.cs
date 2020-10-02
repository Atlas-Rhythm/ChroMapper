using UnityEngine;
using UnityEngine.InputSystem;

public class PauseLauncher : MonoBehaviour, CMInput.IPauseMenuActions
{
    private const int MAPPING_TAB_ID = 1;
    
    public void OnPauseEditor(InputAction.CallbackContext context)
    {
        if(context.performed && !OptionsController.IsActive) OptionsController.ShowOptions(MAPPING_TAB_ID);
    }
}
