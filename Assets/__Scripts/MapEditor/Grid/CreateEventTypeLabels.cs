﻿using UnityEngine;
using TMPro;

public class CreateEventTypeLabels : MonoBehaviour {

    public TMP_FontAsset AvailableAsset;
    public TMP_FontAsset UtilityAsset;
    public TMP_FontAsset RedAsset;
    public GameObject LayerInstantiate;
    public Transform[] EventGrid;

    private LightsManager[] LightingManagers;

	// Use this for initialization
	void Start () {
        LoadInitialMap.PlatformLoadedEvent += PlatformLoaded;
	}

    public void UpdateLabels(bool isRingPropagation, int lanes = 16)
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
            instantiate.transform.localPosition = new Vector3(isRingPropagation ? i : modified, 0, 0);
            try
            {
                TextMeshProUGUI textMesh = instantiate.GetComponentInChildren<TextMeshProUGUI>();
                if (isRingPropagation)
                {
                    textMesh.font = UtilityAsset;
                    if (i == 0)
                    {
                        textMesh.text = "All rings";
                        textMesh.font = RedAsset;
                    }
                    else
                    {
                        textMesh.text = "RING " + i.ToString();
                        if (i % 2 == 0)
                            textMesh.font = UtilityAsset;
                        else
                            textMesh.font = AvailableAsset;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case MapEvent.EVENT_TYPE_RINGS_ROTATE:
                            textMesh.font = UtilityAsset;
                            textMesh.text = "Ring Rotation";
                            break;
                        case MapEvent.EVENT_TYPE_RINGS_ZOOM:
                            textMesh.font = UtilityAsset;
                            textMesh.text = "Ring Zoom";
                            break;
                        case MapEvent.EVENT_TYPE_LEFT_LASERS_SPEED:
                            textMesh.text = "Left Laser Speed";
                            textMesh.font = UtilityAsset;
                            break;
                        case MapEvent.EVENT_TYPE_RIGHT_LASERS_SPEED:
                            textMesh.text = "Right Laser Speed";
                            textMesh.font = UtilityAsset;
                            break;
                        case MapEvent.EVENT_TYPE_EARLY_ROTATION:
                            textMesh.text = "Rotation (Early)";
                            textMesh.font = UtilityAsset;
                            break;
                        case MapEvent.EVENT_TYPE_LATE_ROTATION:
                            textMesh.text = "Rotation (Late)";
                            textMesh.font = UtilityAsset;
                            break;
                        default:
                            if (LightingManagers.Length > i)
                            {
                                LightsManager customLight = LightingManagers[i];
                                textMesh.text = customLight?.name;
                                textMesh.font = AvailableAsset;
                            }
                            else
                            {
                                Destroy(textMesh);
                            }
                            break;
                    }
                }
            }
            catch { }
        }
    }

    void PlatformLoaded(PlatformDescriptor descriptor)
    {
        LightingManagers = descriptor.LightingManagers;

        UpdateLabels(false);
    }

    void OnDestroy()
    {
        LoadInitialMap.PlatformLoadedEvent -= PlatformLoaded;
    }

}
