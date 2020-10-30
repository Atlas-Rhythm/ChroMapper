using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ExtensionButtons : MonoBehaviour
{
    public static ExtensionButtons instance; //null unless in editor scene

    [SerializeField] private Transform container;
    [SerializeField] private ExtensionButton prefab;

    public void AddButton(string text, string tooltip, Sprite icon, UnityAction onClick)
    {
        ExtensionButton extensionButton = Instantiate(prefab, container);
        extensionButton.Text = text;
        extensionButton.Tooltip = tooltip;
        extensionButton.Icon = icon;
        extensionButton.AddOnClick(onClick);
    }

    public void RemoveButton(string text)
    {
        ExtensionButton toDelete = container.GetComponentsInChildren<ExtensionButton>().FirstOrDefault(x => x.Text == text);
        if (toDelete != null)
            Destroy(toDelete.gameObject);
    }

    void Awake()
    {
        instance = this;
    }
}