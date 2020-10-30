using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExtensionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Tooltip tooltip;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    public string Text
    {
        get => text.text;
        set => text.text = value;
    }

    public string Tooltip
    {
        get => tooltip.tooltipOverride;
        set => tooltip.tooltipOverride = value;
    }

    public Sprite Icon
    {
        get => icon.sprite;
        set => icon.sprite = value;
    }

    public void AddOnClick(UnityAction onClick)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }
}
