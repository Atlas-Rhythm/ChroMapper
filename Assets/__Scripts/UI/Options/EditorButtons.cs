using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorButtons : MonoBehaviour
{
    private AutoSaveController saveController;
    
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "03_Mapper")
        {
            saveController = Resources.FindObjectsOfTypeAll<AutoSaveController>().First();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Exit()
    {
        PersistentUI.Instance.ShowDialogBox("Mapper", "save", SaveAndExitResult, PersistentUI.DialogBoxPresetType.YesNoCancel);
    }

    public void Save()
    {
        saveController.Save();
    }

    private void SaveAndExitResult(int result)
    {
        if (result == 0) //Left button (ID 0) clicked; the user wants to Save before exiting.
        {
            Save();
            SceneTransitionManager.Instance.LoadScene(2);
        }
        else if (result == 1) //Middle button (ID 1) clicked; the user does not want to save before exiting.
            SceneTransitionManager.Instance.LoadScene(2);
        //Right button (ID 2) would be clicked; the user does not want to exit the editor after all, so we aint doing shit.
    }
}
