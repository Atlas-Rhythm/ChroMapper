﻿using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StrobeLightingPass : StrobeGeneratorPass
{
    private IEnumerable<int> values;
    private bool alternateColors;
    private bool dynamic;
    private int precision;
    private Func<float, float> easingFunc;

    public StrobeLightingPass(IEnumerable<int> alternatingValues, bool switchColors, bool dynamicStrobe, int strobePrecision, string strobeEasing)
    {
        values = alternatingValues;
        alternateColors = switchColors;
        dynamic = dynamicStrobe;
        precision = strobePrecision;
        easingFunc = Easing.named(strobeEasing);
    }

    public override bool IsEventValidForPass(MapEvent @event) => !@event.IsUtilityEvent && !@event.IsLegacyChromaEvent;

    public override IEnumerable<MapEvent> StrobePassForLane(IEnumerable<MapEvent> original, int type)
    {
        Debug.Log(string.Join(",", values));
        List<MapEvent> generated = new List<MapEvent>();
        generated.AddRange(StrobePassForPropID(original.Where(x => x._customData == null || !x._customData.HasKey("_propID")), type, null));
        foreach (var grouping in original.Where(x => x._customData != null && x._customData.HasKey("_propID"))
            .GroupBy(x => x._customData["_propID"].AsInt))
        {
            int propID = grouping.Key;
            generated.AddRange(StrobePassForPropID(grouping, type, propID));
        }
        return generated;
    }

    private IEnumerable<MapEvent> StrobePassForPropID(IEnumerable<MapEvent> original, int type, int? propID)
    {
        List<MapEvent> generatedObjects = new List<MapEvent>();

        float startTime = original.First()._time;
        float endTime = original.Last()._time;

        List<int> alternatingTypes = new List<int>(values);
        int typeIndex = 0;
        if (alternateColors)
        {
            for (int i = 0; i < values.Count(); i++)
            {
                alternatingTypes.Add(InvertColors(alternatingTypes[i]));
            }
        }
        float distanceInBeats = endTime - startTime;
        float originalDistance = distanceInBeats;
        MapEvent lastPassed = null;

        while (distanceInBeats >= 0)
        {
            if (typeIndex >= alternatingTypes.Count) typeIndex = 0;

            MapEvent any = original.Where(x => x._time <= endTime - distanceInBeats).LastOrDefault();
            if (any != lastPassed && dynamic && (MapEvent.IsBlueEventFromValue(any._value) != MapEvent.IsBlueEventFromValue(alternatingTypes[typeIndex])))
            {
                lastPassed = any;
                for (int i = 0; i < alternatingTypes.Count; i++)
                {
                    alternatingTypes[i] = InvertColors(alternatingTypes[i]);
                }
            }

            int value = alternatingTypes[typeIndex];
            float progress = (originalDistance - distanceInBeats) / originalDistance;
            float newTime = (easingFunc(progress) * originalDistance) + startTime;
            MapEvent data = new MapEvent(newTime, type, value);
            if (propID != null)
            {
                data._customData = new JSONObject();
                data._customData.Add("_propID", propID.Value);
            }
            generatedObjects.Add(data);
            typeIndex++;
            distanceInBeats -= 1 / (float)precision;
        }

        return generatedObjects;
    }

    private int InvertColors(int colorValue)
    {
        switch (colorValue)
        {
            case MapEvent.LIGHT_VALUE_BLUE_ON: return MapEvent.LIGHT_VALUE_RED_ON;
            case MapEvent.LIGHT_VALUE_BLUE_FLASH: return MapEvent.LIGHT_VALUE_RED_FLASH;
            case MapEvent.LIGHT_VALUE_BLUE_FADE: return MapEvent.LIGHT_VALUE_RED_FADE;
            case MapEvent.LIGHT_VALUE_RED_ON: return MapEvent.LIGHT_VALUE_BLUE_ON;
            case MapEvent.LIGHT_VALUE_RED_FLASH: return MapEvent.LIGHT_VALUE_BLUE_FLASH;
            case MapEvent.LIGHT_VALUE_RED_FADE: return MapEvent.LIGHT_VALUE_BLUE_FADE;
            default: return MapEvent.LIGHT_VALUE_OFF;
        }
    }
}
