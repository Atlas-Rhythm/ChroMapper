using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour {

    public Volume PostProcess;

    private void Start()
    {
        Settings.NotifyBySettingName("ChromaticAberration", (value) => UpdateChromaticAberration((bool)value));
        Settings.NotifyBySettingName("PostProcessingIntensity", (value) => UpdatePostProcessIntensity((float)value));
        UpdateChromaticAberration(Settings.Instance.ChromaticAberration);
        UpdatePostProcessIntensity(Settings.Instance.PostProcessingIntensity);
    }

    private void OnDestroy()
    {
        Settings.ClearSettingNotifications("ChromaticAberration");
        Settings.ClearSettingNotifications("PostProcessingIntensity");
    }

    public void UpdatePostProcessIntensity(float v)
    {
        PostProcess.profile.TryGet(out Bloom bloom);
        bloom.intensity.value = v;
    }

    public void UpdateChromaticAberration(bool enabled)
    {
        PostProcess.profile.TryGet(out ChromaticAberration ca);
        ca.active = enabled;
    }
}
