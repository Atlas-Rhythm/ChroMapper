﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyRow
{
    public Transform Obj { get; private set; }
    public Image Background { get; private set; }
    public Image ButtonBackground { get; private set; }
    public string Name { get; private set; }
    public Toggle Toggle { get; private set; }
    public Button Button { get; private set; }
    public TMP_InputField NameInput { get; private set; }
    public Button Copy { get; private set; }
    public Image CopyImage { get; private set; }
    public Button Save { get; private set; }
    public Button Revert { get; private set; }
    public Button Paste { get; private set; }

    public DifficultyRow(Transform obj)
    {
        Obj = obj;
        Name = obj.name;
        Background = obj.GetComponent<Image>();
        ButtonBackground = obj.Find("Button").GetComponent<Image>();
        Toggle = obj.Find("Button/Toggle").GetComponent<Toggle>();
        Button = obj.Find("Button").GetComponent<Button>();
        NameInput = obj.Find("Button/Name").GetComponent<TMP_InputField>();
        Copy = obj.Find("Copy").GetComponent<Button>();
        CopyImage = obj.Find("Copy").GetComponent<Image>();
        Save = obj.Find("Warning").GetComponent<Button>();
        Revert = obj.Find("Revert").GetComponent<Button>();
        Paste = obj.Find("Paste").GetComponent<Button>();
    }

    /// <summary>
    /// Helper to enable or disable a row for editing
    /// </summary>
    /// <param name="val">True if it should be enabled</param>
    public void SetInteractable(bool val)
    {
        NameInput.interactable = Button.interactable = Toggle.isOn = val;
    }

    /// <summary>
    /// Helper to show UI buttons from the current settings
    /// </summary>
    /// <param name="difficultySettings">The current difficulty state</param>
    public void ShowDirtyObjects(DifficultySettings difficultySettings)
    {
        ShowDirtyObjects(difficultySettings.IsDirty(), !difficultySettings.IsDirty());
    }

    /// <summary>
    /// Helper to show UI buttons for performing actions on this difficulty
    /// we can't show all the buttons at once, but that logic isn't here
    /// </summary>
    /// <param name="show">Should we show the copy/save button</param>
    /// <param name="copy">Should we show the copy button</param>
    public void ShowDirtyObjects(bool show, bool copy)
    {
        Copy.gameObject.SetActive(copy);
        Save.gameObject.SetActive(show);
        Revert.gameObject.SetActive(show);
    }
}