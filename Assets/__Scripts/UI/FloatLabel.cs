using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class FloatLabel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    private Animation animation;
    [SerializeField] private AnimationClip floatUp;
    [SerializeField] private AnimationClip floatDown;

    private bool isSelected;
    private bool labelIsUp;

    void Start()
    {
        animation = GetComponent<Animation>();
        inputField.onValueChanged.AddListener(OnTextChanged);
        inputField.onSelect.AddListener(OnSelect);
        inputField.onDeselect.AddListener(OnDeselect);
    }

    void Update()
    {
        if (!labelIsUp && !isSelected && !string.IsNullOrEmpty(inputField.text))
        {
            animation.clip = floatUp;
            animation.Play();
            animation[floatUp.name].normalizedTime = 1;
            labelIsUp = true;
        }
    }
    
    private void OnTextChanged(string text)
    {
        Debug.Log($"TEXT CHANGE {text}");
    }

    private void OnSelect(string text)
    {
        isSelected = true;
        if (!labelIsUp)
        {
            animation.clip = floatUp;
            animation.Play();
            labelIsUp = true;
        }
    }

    private void OnDeselect(string text)
    {
        isSelected = false;
        if(labelIsUp && string.IsNullOrEmpty(text))
        {
            animation.clip = floatDown;
            animation.Play();
            labelIsUp = false;
        }
    }
}
