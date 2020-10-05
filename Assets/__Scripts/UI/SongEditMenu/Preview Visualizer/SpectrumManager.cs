using UnityEngine;
using UnityEngine.UI;

public class SpectrumManager : MonoBehaviour
{
    private float[] spectrum = new float[128];
    public static float lastValue;
    public static float currentValue;

    void Start()
    {
        for(int i = 0; i < 128; i++)
        {
            GameObject chunk = new GameObject();
            chunk.transform.parent = transform;
            chunk.AddComponent<Image>();
            (chunk.transform as RectTransform).sizeDelta = Vector2.zero;
            ScaleToSpectrum sts = chunk.AddComponent<ScaleToSpectrum>();
            sts.beatThreshold = (i / 128f) * 0.6f;
        }
    }

    void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);
        lastValue = currentValue;
        currentValue = spectrum[0];
    }
}
