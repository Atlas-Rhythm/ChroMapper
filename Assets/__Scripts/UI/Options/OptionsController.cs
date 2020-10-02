﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

public class OptionsController : MonoBehaviour, CMInput.IPauseMenuActions
{
    [SerializeField] private CanvasGroup optionsCanvasGroup;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private Canvas optionsCanvas;
    //[SerializeField] private Button iCareForModders; I CARE TOO!!! But like, this just wont work atm.

    public static Action OptionsLoadedEvent;

    public List<CanvasGroup> OptionBodyCanvasGroups;

    private GameObject postProcessingGO;

    public static bool IsActive { get; internal set; }

    public static void ShowOptions(int loadGroup = 0)
    {
        if (IsActive) return;
        SceneManager.LoadScene(4, LoadSceneMode.Additive);
        CMInputCallbackInstaller.DisableActionMaps(typeof(CMInput).GetNestedTypes().Where(x => x.IsInterface));
        CMInputCallbackInstaller.ClearDisabledActionMaps(new Type[] { typeof(CMInput.IPauseMenuActions) });
        OptionsLoadedEvent?.Invoke();
        IsActive = true;
    }

    public void Close()
    {
        StartCoroutine(CloseOptions());
    }

    public void GoToURL(string url)
    {
        Application.OpenURL(url);
    }

    private IEnumerator CloseOptions()
    {
        yield return StartCoroutine(Close(2, optionsCanvasGroup));
        CMInputCallbackInstaller.ClearDisabledActionMaps(typeof(CMInput).GetNestedTypes().Where(x => x.IsInterface));
        IsActive = false;
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("04_Options"));
    }

    IEnumerator FadeIn(float rate, CanvasGroup group)
    {
        group.blocksRaycasts = true;
        group.interactable = true;
        float t = 0;
        while (t < 1)
        {
            group.alpha = fadeInCurve.Evaluate(t);
            t += Time.deltaTime * rate;
            yield return new WaitForEndOfFrame();
        }
        group.alpha = 1;
        yield return new WaitForEndOfFrame();
        foreach(CanvasGroup notGroup in OptionBodyCanvasGroups.Where(x => x != group))
        {
            notGroup.blocksRaycasts = false;
            notGroup.interactable = false;
            notGroup.alpha = 0;
        }
    }

    IEnumerator Close(float rate, CanvasGroup group)
    {
        float t = 1;
        while (t > 0)
        {
            group.alpha = fadeOutCurve.Evaluate(t);
            t -= Time.deltaTime * rate;
            yield return new WaitForEndOfFrame();
        }
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    public void ToggleBongo()
    {
        FindObjectsOfType<BongoCat>().FirstOrDefault()?.ToggleBongo();
    }

    public void OnPauseEditor(InputAction.CallbackContext context)
    {
        if (context.performed) Close();
    }
}
