using UnityEngine;
using TMPro;

public class CreateEventTypeLabels : MonoBehaviour {
    
    public Color AvailableAssetColor;
    public Color UtilityAssetColor;
    public Color RedAssetColor;
    public GameObject LayerInstantiate;
    public Transform[] EventGrid;
    //[SerializeField] private DarkThemeSO darkTheme;

    private LightsManager[] LightingManagers;

	// Use this for initialization
	void Start () {
        LoadInitialMap.PlatformLoadedEvent += PlatformLoaded;
	}

    public void UpdateLabels(bool isPropagation, int eventType, int lanes = 16)
    {
        foreach (Transform children in LayerInstantiate.transform.parent.transform)
        {
            if (children.gameObject.activeSelf)
                Destroy(children.gameObject);
        }

        for (int i = 0; i < lanes; i++)
        {
            int modified = BeatmapEventContainer.EventTypeToModifiedType(i);
            GameObject instantiate = Instantiate(LayerInstantiate, LayerInstantiate.transform.parent);
            instantiate.SetActive(true);
            instantiate.transform.localPosition = new Vector3(isPropagation ? i : modified, 0, 0);
            try
            {
                TextMeshProUGUI textMesh = instantiate.GetComponentInChildren<TextMeshProUGUI>();
                if (isPropagation)
                {
                    textMesh.color = UtilityAssetColor;
                    if (i == 0)
                    {
                        textMesh.text = "All Lights";
                        textMesh.color = RedAssetColor;
                    }
                    else
                    {
                        textMesh.text = $"{LightingManagers[eventType].name} ID {i}";
                        if (i % 2 == 0)
                            textMesh.color = UtilityAssetColor;
                        else
                            textMesh.color = AvailableAssetColor;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case MapEvent.EVENT_TYPE_RINGS_ROTATE:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Ring Rotation";
                            break;
                        case MapEvent.EVENT_TYPE_RINGS_ZOOM:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Ring Zoom";
                            break;
                        case MapEvent.EVENT_TYPE_LEFT_LASERS_SPEED:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Left Laser Speed";
                            break;
                        case MapEvent.EVENT_TYPE_RIGHT_LASERS_SPEED:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Right Laser Speed";
                            break;
                        case MapEvent.EVENT_TYPE_EARLY_ROTATION:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Rotation (Include)";
                            break;
                        case MapEvent.EVENT_TYPE_LATE_ROTATION:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Rotation (Exclude)";
                            break;
                        case MapEvent.EVENT_TYPE_BOOST_LIGHTS:
                            textMesh.color = UtilityAssetColor;
                            textMesh.text = "Boost Lights";
                            break;
                        default:
                            if (LightingManagers.Length > i)
                            {
                                LightsManager customLight = LightingManagers[i];
                                textMesh.text = customLight?.name;
                                textMesh.color = AvailableAssetColor;
                            }
                            else
                            {
                                Destroy(textMesh);
                            }
                            break;
                    }
                    /*
                    if (Settings.Instance.DarkTheme)
                    {
                        textMesh.font = darkTheme.TekoReplacement;
                    }*/
                }
            }
            catch { }
        }
    }

    void PlatformLoaded(PlatformDescriptor descriptor)
    {
        LightingManagers = descriptor.LightingManagers;

        UpdateLabels(false, MapEvent.EVENT_TYPE_RING_LIGHTS);
    }

    void OnDestroy()
    {
        LoadInitialMap.PlatformLoadedEvent -= PlatformLoaded;
    }

}
