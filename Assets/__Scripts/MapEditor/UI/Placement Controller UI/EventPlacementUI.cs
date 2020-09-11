﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[Obsolete]
public class EventPlacementUI : MonoBehaviour/*, CMInput.IEventUIActions*/
{
    [SerializeField] private EventPlacement eventPlacement;
    [SerializeField] private Toggle redColorToggle;
    [SerializeField] private Toggle blueColorToggle;
    [SerializeField] private Toggle offValueToggle;
    [SerializeField] private Toggle onValueToggle;
    [SerializeField] private Toggle fadeValueToggle;
    [SerializeField] private Toggle flashValueToggle;
    [SerializeField] private CanvasGroup precisionRotationCanvasGroup;
    [SerializeField] private DeleteToolController deleteToolController;
    private bool red = true;

    public bool IsTypingRotation { get; private set; } = false;

    public void Off(bool active)
    {
        if (!active) return;
        UpdateValue(MapEvent.LIGHT_VALUE_OFF);
        UpdateUI(false);
    }

    public void On(bool active)
    {
        if (!active) return;
        UpdateValue(red ? MapEvent.LIGHT_VALUE_RED_ON : MapEvent.LIGHT_VALUE_BLUE_ON);
        UpdateUI(false);
    }

    public void Fade(bool active)
    {
        if (!active) return;
        UpdateValue(red ? MapEvent.LIGHT_VALUE_RED_FADE : MapEvent.LIGHT_VALUE_BLUE_FADE);
        UpdateUI(false);
    }

    public void Flash(bool active)
    {
        if (!active) return;
        UpdateValue(red ? MapEvent.LIGHT_VALUE_RED_FLASH : MapEvent.LIGHT_VALUE_BLUE_FLASH);
        UpdateUI(false);
    }

    public void Red(bool active)
    {
        if (!active) return;
        red = true;
        UpdateUI(false);
        eventPlacement.SwapColors(true);
    }

    public void Blue(bool active)
    {
        if (!active) return;
        red = false;
        UpdateUI(false);
        eventPlacement.SwapColors(false);
    }

    public void UpdateUI(bool del) // delete toggle isnt in event toggle group, so lets fake it
    {
        deleteToolController.UpdateDeletion(del);
    }

    private void UpdateValue(int value)
    {
        //if (!customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return; // again idk what this is

        // We can't limit this as most of the time to get to the selection bar the user will have hovered
        // over a utility track and we still need to update the value otherwise the UI gets out of sync
        //if (!eventPlacement.queuedData.IsUtilityEvent)

        eventPlacement.UpdateValue(value);
    }

    public void UpdatePrecisionRotationValue()
    {
        bool enabled = precisionRotationCanvasGroup.alpha == 0;
        precisionRotationCanvasGroup.alpha = enabled ? 1 : 0;
        eventPlacement.PlacePrecisionRotation = enabled;
    }

    public void UpdatePrecisionRotation(string res)
    {
        if (int.TryParse(res, out int value))
        {
            eventPlacement.PrecisionRotationValue = value;
            IsTypingRotation = false;
        }
    }

    public void UpdateTyping(string res)
    {
        IsTypingRotation = true;
    }

    public void OnTypeOn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        onValueToggle.isOn = true;
        UpdateUI(false);
    }

    public void OnTypeFlash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        flashValueToggle.isOn = true;
        UpdateUI(false);
    }

    public void OnTypeOff(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        offValueToggle.isOn = true;
        UpdateUI(false);
    }

    public void OnTypeFade(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        fadeValueToggle.isOn = true;
        UpdateUI(false);
    }

    public void OnTogglePrecisionRotation(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        UpdatePrecisionRotationValue();
    }
}
