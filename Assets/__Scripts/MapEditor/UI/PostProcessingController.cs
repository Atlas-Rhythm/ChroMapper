using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;

public class PostProcessingController : MonoBehaviour {

    public Volume PostProcess;

    private void Start()
    {
        Settings.NotifyBySettingName(nameof(Settings.PostProcessingIntensity), UpdatePostProcessIntensity);
        Settings.NotifyBySettingName(nameof(Settings.ChromaticAberration), UpdateChromaticAberration);

        UpdatePostProcessIntensity(Settings.Instance.PostProcessingIntensity);
        UpdateChromaticAberration(Settings.Instance.ChromaticAberration);
    }

    private void OnDestroy()
    {
        Settings.ClearSettingNotifications("ChromaticAberration");
        Settings.ClearSettingNotifications("PostProcessingIntensity");
    }

    public void UpdatePostProcessIntensity(object o)
    {
        float v = Convert.ToSingle(o);
        PostProcess.profile.TryGet(out Bloom bloom);
        bloom.intensity.value = v;
    }

    public void UpdateChromaticAberration(object o)
    {
        bool enabled = Convert.ToBoolean(o);
        PostProcess.profile.TryGet(out ChromaticAberration ca);
        ca.active = enabled;
    }
}
