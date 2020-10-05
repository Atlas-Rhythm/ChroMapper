using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScaleToSpectrum : MonoBehaviour
{
    public float beatThreshold = 0.2f;
    public float minimumTimeToBeat = 0.15f;
    public float beatDuration = 0.04f;
    public float falloffMultiplier = 2.8f;
    public float scaleSize = 50;

    private float timer;
    private bool isAnimating;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = transform as RectTransform;
    }

    void Update()
    {
        Debug.Log(SpectrumManager.currentValue);
        if(SpectrumManager.currentValue > beatThreshold && SpectrumManager.lastValue < beatThreshold ||
            SpectrumManager.currentValue < beatThreshold && SpectrumManager.lastValue > beatThreshold)
        {
            if(timer > minimumTimeToBeat)
            {
                timer = 0;
                isAnimating = true;
                StartCoroutine(HandleBeat());
            }
        }
        timer += Time.deltaTime;
        if (isAnimating) return;
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(rectTransform.sizeDelta.x, 0), Time.deltaTime * falloffMultiplier);
    }

    private IEnumerator HandleBeat()
    {
        float animationTime = 0;
        while(rectTransform.sizeDelta.y < scaleSize)
        {
            rectTransform.sizeDelta = Vector2.Lerp(new Vector2(rectTransform.sizeDelta.x, 0), new Vector2(rectTransform.sizeDelta.x, scaleSize), animationTime / beatDuration);
            animationTime += Time.deltaTime;
            yield return null;
        }
        isAnimating = false;
    }
}
