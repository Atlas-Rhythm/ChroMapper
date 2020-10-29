﻿using SimpleJSON;
using System;

[Serializable]
public class BeatmapObstacle : BeatmapObject
{

    //These are uhh, assumptions...
    public const int VALUE_FULL_BARRIER = 0;
    public const int VALUE_HIGH_BARRIER = 1;

    /*
     * Obstacle Logic
     */

    public BeatmapObstacle(JSONNode node) {
        _time = RetrieveRequiredNode(node, "_time").AsFloat;
        _lineIndex = RetrieveRequiredNode(node, "_lineIndex").AsInt;
        _type = RetrieveRequiredNode(node, "_type").AsInt;
        _duration = RetrieveRequiredNode(node, "_duration").AsFloat;
        _width = RetrieveRequiredNode(node, "_width").AsInt;
        _customData = node["_customData"];
    }

    public BeatmapObstacle(float time, int lineIndex, int type, float duration, int width, JSONNode customData = null) {
        _time = time;
        _lineIndex = lineIndex;
        _type = type;
        _duration = duration;
        _width = width;
        _customData = customData;
    }

    public override JSONNode ConvertToJSON() {
        JSONNode node = new JSONObject();
        node["_time"] = Math.Round(_time, decimalPrecision);
        node["_lineIndex"] = _lineIndex;
        node["_type"] = _type;
        node["_duration"] = Math.Round(_duration, decimalPrecision); //Get rid of float precision errors
        node["_width"] = _width;
        if (_customData != null) node["_customData"] = _customData;
        /*if (Settings.Instance.AdvancedShit) //This will be left commented unless its 100%, absolutely, positively required.
        {   
            //By request of Spooky Ghost to determine BeatWalls VS CM walls
            if (!node["_customData"].HasKey("_editor"))
            {
                node["_customData"]["_editor"] = BeatSaberSongContainer.Instance.song.editor;
            }
        }*/
        return node;
    }

    protected override bool IsConflictingWithObjectAtSameTime(BeatmapObject other, bool deletion)
    {
        if (other is BeatmapObstacle obstacle)
        {
            if (IsNoodleExtensionsWall || obstacle.IsNoodleExtensionsWall)
            {
                return ConvertToJSON().ToString() == other.ConvertToJSON().ToString();
            }
            return _lineIndex == obstacle._lineIndex && _type == obstacle._type;
        }
        return false;
    }

    public bool IsNoodleExtensionsWall => _customData != null &&
        (_customData.HasKey("_position") || _customData.HasKey("_scale")
            || _customData.HasKey("_localRotation") || _customData.HasKey("_rotation"));
    public override Type beatmapType { get; set; } = Type.OBSTACLE;
    public int _lineIndex;
    public int _type;
    public float _duration;
    public int _width;
}