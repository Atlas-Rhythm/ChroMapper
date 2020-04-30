﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// Big boi master class for everything Selection.
/// </summary>
public class SelectionController : MonoBehaviour, CMInput.ISelectingActions, CMInput.IModifyingSelectionActions
{

    public static HashSet<BeatmapObjectContainer> SelectedObjects = new HashSet<BeatmapObjectContainer>();
    public static HashSet<BeatmapObject> CopiedObjects = new HashSet<BeatmapObject>();

    public static Action<BeatmapObjectContainer> ObjectWasSelectedEvent;

    [SerializeField] private AudioTimeSyncController atsc;
    [SerializeField] private Material selectionMaterial;
    [SerializeField] private Transform moveableGridTransform;
    internal BeatmapObjectContainerCollection[] collections;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color copiedColor;
    [SerializeField] private TracksManager tracksManager;
    [SerializeField] private EventPlacement eventPlacement;

    private static SelectionController instance;

    // Use this for initialization
    void Start()
    {
        collections = moveableGridTransform.GetComponents<BeatmapObjectContainerCollection>();
        instance = this;
        SelectedObjects.Clear();
    }

    #region Utils

    /// <summary>
    /// Does the user have any selected objects?
    /// </summary>
    public static bool HasSelectedObjects()
    {
        return SelectedObjects.Count > 0;
    }

    /// <summary>
    /// Does the user have any copied objects?
    /// </summary>
    public static bool HasCopiedObjects()
    {
        return CopiedObjects.Count > 0;
    }

    /// <summary>
    /// Returns true if the given container is selected, and false if it's not.
    /// </summary>
    /// <param name="container">Container to check.</param>
    public static bool IsObjectSelected(BeatmapObjectContainer container) => SelectedObjects.Contains(container);

    #endregion

    #region Selection

    /// <summary>
    /// Select an individual container.
    /// </summary>
    /// <param name="container">The container to select.</param>
    /// <param name="AddsToSelection">Whether or not previously selected objects will deselect before selecting this object.</param>
    /// <param name="AddActionEvent">If an action event to undo the selection should be made</param>
    public static void Select(BeatmapObjectContainer container, bool AddsToSelection = false, bool AutomaticallyRefreshes = true, bool AddActionEvent = true)
    {
        if (IsObjectSelected(container)) return; //Cant select an already selected object now, can ya?
        if (!AddsToSelection) DeselectAll(); //This SHOULD deselect every object unless you otherwise specify, but it aint working.
        SelectedObjects.Add(container);
        if (AutomaticallyRefreshes) RefreshSelectionMaterial();
        if (AddActionEvent) ObjectWasSelectedEvent.Invoke(container);
    }

    /// <summary>
    /// Deselects a container if it is currently selected
    /// </summary>
    /// <param name="container">The container to deselect, if it has been selected.</param>
    public static void Deselect(BeatmapObjectContainer container)
    {
        SelectedObjects.RemoveWhere(x => x == null);
        SelectedObjects.Remove(container);
        container.OutlineVisible = false;
    }

    /// <summary>
    /// Deselect all selected objects.
    /// </summary>
    public static void DeselectAll()
    {
        SelectedObjects.RemoveWhere(x => x == null);
        foreach (BeatmapObjectContainer con in SelectedObjects)
        {
            con.OutlineVisible = false;
        }
        SelectedObjects.Clear();
    }

    /// <summary>
    /// Deselect all selected objects that match a given predicate
    /// </summary>
    public static void DeselectAll(Func<BeatmapObjectContainer, bool> predicate)
    {
        SelectedObjects.RemoveWhere(x => x == null);
        foreach (BeatmapObjectContainer con in SelectedObjects)
        {
            if (predicate(con))
            {
                Deselect(con);
            }
        }
    }
	
	/// <summary>
    /// Deselect all objects, or select all objects if none are already selected
    /// </summary>
	private void SelectDeselectAll(){
		if (HasSelectedObjects()) DeselectAll();
		else SelectAllFromBeat(0);
	}
	
	/// <summary>
    /// Select all objects on or after the specified beat
    /// </summary>
	private void SelectAllFromBeat(float beat){
		DeselectAll();
 		foreach (BeatmapObjectContainerCollection collection in collections) {
			foreach(BeatmapObjectContainer container in collection.LoadedContainers){
                if (container.objectData._time != null && container.objectData._time >= beat) Select(container, true, false, false);
			}
		}
		RefreshSelectionMaterial(true);
	}
	
	

    /// <summary>
    /// Can be very taxing. Use sparringly.
    /// </summary>
    internal static void RefreshSelectionMaterial(bool triggersAction = true)
    {
        SelectedObjects.RemoveWhere(x => x == null);
        foreach (BeatmapObjectContainer con in SelectedObjects)
        {
            con.OutlineVisible = true;
            con.SetOutlineColor(instance.selectedColor);
        }
        if (triggersAction) BeatmapActionContainer.AddAction(new SelectionChangedAction(SelectedObjects));
    }

    #endregion

    #region Manipulation
    
    /// <summary>
    /// Deletes and clears the current selection.
    /// </summary>
    public void Delete(bool triggersAction = true)
    {
        if (triggersAction) BeatmapActionContainer.AddAction(new SelectionDeletedAction(SelectedObjects));
        foreach (BeatmapObjectContainer con in SelectedObjects)
            foreach (BeatmapObjectContainerCollection container in collections) container.DeleteObject(con, false);
        SelectedObjects.Clear();
        RefreshMap();
        tracksManager.RefreshTracks();
    }
    
    /// <summary>
    /// Copies the current selection for later Pasting.
    /// </summary>
    /// <param name="cut">Whether or not to delete the original selection after copying them.</param>
    public void Copy(bool cut = false)
    {
        if (!HasSelectedObjects()) return;
        Debug.Log("Copied!");
        CopiedObjects.Clear();
        SelectedObjects = new HashSet<BeatmapObjectContainer>(SelectedObjects.OrderBy(x => x.objectData._time));
        float firstTime = SelectedObjects.First().objectData._time;
        foreach (BeatmapObjectContainer con in SelectedObjects)
        {
            con.SetOutlineColor(instance.copiedColor);
            BeatmapObject copy = BeatmapObject.GenerateCopy(con.objectData);
            copy._time -= firstTime;
            CopiedObjects.Add(copy);
        }
        if (cut) Delete();
    }

    /// <summary>
    /// Pastes any copied objects into the map, selecting them immediately.
    /// </summary>
    public void Paste(bool triggersAction = true)
    {
        DeselectAll();
        CopiedObjects = new HashSet<BeatmapObject>(CopiedObjects.OrderBy(x => x._time));
        HashSet<BeatmapObjectContainer> pasted = new HashSet<BeatmapObjectContainer>();
        foreach (BeatmapObject data in CopiedObjects)
        {
            if (data == null) continue;
            float newTime = data._time + atsc.CurrentBeat;
            BeatmapObject newData = BeatmapObject.GenerateCopy(data);
            newData._time = newTime;
            BeatmapObjectContainer pastedContainer = collections.Where(x => x.ContainerType == newData.beatmapType).FirstOrDefault()?.SpawnObject(newData, out _);
            pastedContainer.UpdateGridPosition();
            Select(pastedContainer, true, false, false);
            pasted.Add(pastedContainer);
        }
        if (triggersAction) BeatmapActionContainer.AddAction(new SelectionPastedAction(pasted, CopiedObjects, atsc.CurrentBeat));
        RefreshSelectionMaterial(false);
        RefreshMap();
        tracksManager.RefreshTracks();

        if (eventPlacement.objectContainerCollection.RingPropagationEditing)
            eventPlacement.objectContainerCollection.RingPropagationEditing = eventPlacement.objectContainerCollection.RingPropagationEditing;
        Debug.Log("Pasted!");
    }

    public void MoveSelection(float beats, bool snapObjects = false)
    {
        foreach (BeatmapObjectContainer con in SelectedObjects)
        {
            con.objectData._time += beats;
            if (snapObjects)
                con.objectData._time = Mathf.Round(beats / (1f / atsc.gridMeasureSnapping)) * (1f / atsc.gridMeasureSnapping);
            con.UpdateGridPosition();
            if (con is BeatmapEventContainer e && e.eventData.IsRotationEvent) tracksManager.RefreshTracks();
        }
    }

    public void ShiftSelection(int leftRight, int upDown)
    {
        foreach(BeatmapObjectContainer con in SelectedObjects)
        {
            if (con is BeatmapNoteContainer note)
            {
                if (note.mapNoteData._lineIndex >= 1000)
                {
                    note.mapNoteData._lineIndex += Mathf.RoundToInt((1f / atsc.gridMeasureSnapping) * 1000 * leftRight);
                    if (note.mapNoteData._lineIndex < 1000) note.mapNoteData._lineIndex = 1000;
                }
                else if (note.mapNoteData._lineIndex <= -1000)
                {
                    note.mapNoteData._lineIndex += Mathf.RoundToInt((1f / atsc.gridMeasureSnapping) * 1000 * leftRight);
                    if (note.mapNoteData._lineIndex > -1000) note.mapNoteData._lineIndex = -1000;
                }
                else note.mapNoteData._lineIndex += leftRight;
                note.mapNoteData._lineLayer += upDown;
            }
            else if (con is BeatmapObstacleContainer obstacle)
            {
                if (obstacle.obstacleData._lineIndex >= 1000)
                {
                    obstacle.obstacleData._lineIndex += Mathf.RoundToInt((1f / atsc.gridMeasureSnapping) * 1000 * leftRight);
                    if (obstacle.obstacleData._lineIndex < 1000) obstacle.obstacleData._lineIndex = 1000;
                }
                else if (obstacle.obstacleData._lineIndex <= -1000)
                {
                    obstacle.obstacleData._lineIndex += Mathf.RoundToInt((1f / atsc.gridMeasureSnapping) * 1000 * leftRight);
                    if (obstacle.obstacleData._lineIndex > -1000) obstacle.obstacleData._lineIndex = -1000;
                }
                else obstacle.obstacleData._lineIndex += leftRight;
            }
            else if (con is BeatmapEventContainer e)
            {
                if (eventPlacement.objectContainerCollection.RingPropagationEditing)
                {
                    int pos = -1 + leftRight;
                    if (con.objectData._customData != null && con.objectData._customData["_propID"].IsNumber)
                        pos = (con.objectData?._customData["_propID"]?.AsInt ?? -1) + leftRight;
                    if (e.eventData._type != MapEvent.EVENT_TYPE_RING_LIGHTS)
                    {
                        e.UpdateAlpha(0);
                        pos = -1;
                    }
                    else
                    {
                        if (pos < -1) pos = -1;
                        if (pos > 14) pos = 14;
                    }
                    con.transform.localPosition = new Vector3(pos + 0.5f, 0.5f, con.transform.localPosition.z);
                    if (pos == -1)
                    {
                        con.objectData._customData?.Remove("_propID");
                    }
                    else
                    {
                        con.objectData._customData["_propID"] = pos;
                    }
                }
                else
                {
                    if (e.eventData._customData != null && e.eventData._customData["_propID"] != null)
                        e.eventData._customData["_propID"] = e.eventData._customData["_propID"].AsInt + leftRight;
                    int modified = BeatmapEventContainer.EventTypeToModifiedType(e.eventData._type);
                    modified += leftRight;
                    if (modified < 0) modified = 0;
                    if (modified > 15) modified = 15;
                    e.eventData._type = BeatmapEventContainer.ModifiedTypeToEventType(modified);
                    e.RefreshAppearance();
                    if (e.eventData.IsRotationEvent || e.eventData._type - leftRight == MapEvent.EVENT_TYPE_LATE_ROTATION ||
                        e.eventData._type - leftRight == MapEvent.EVENT_TYPE_EARLY_ROTATION) tracksManager.RefreshTracks();
                }
            }
            con.UpdateGridPosition();
            if (eventPlacement.objectContainerCollection.RingPropagationEditing) 
                eventPlacement.objectContainerCollection.RingPropagationEditing = eventPlacement.objectContainerCollection.RingPropagationEditing;
        }
        RefreshMap();
    }

    public static void RefreshMap()
    {
        if (BeatSaberSongContainer.Instance.map != null)
        {
            Dictionary<BeatmapObject.Type, List<BeatmapObject>> newObjects = new Dictionary<BeatmapObject.Type, List<BeatmapObject>>();
            foreach (BeatmapObjectContainerCollection collection in instance.collections)
            {
                collection.SortObjects();
                newObjects.Add(collection.ContainerType, collection.LoadedContainers.Select(x => x.objectData).ToList());
            }
            if (Settings.Instance.Load_Notes)
                BeatSaberSongContainer.Instance.map._notes = newObjects[BeatmapObject.Type.NOTE].Cast<BeatmapNote>().ToList();
            if (Settings.Instance.Load_Obstacles)
                BeatSaberSongContainer.Instance.map._obstacles = newObjects[BeatmapObject.Type.OBSTACLE].Cast<BeatmapObstacle>().ToList();
            if (Settings.Instance.Load_Events)
                BeatSaberSongContainer.Instance.map._events = newObjects[BeatmapObject.Type.EVENT].Cast<MapEvent>().ToList();
            if (Settings.Instance.Load_Others)
                BeatSaberSongContainer.Instance.map._customEvents = newObjects[BeatmapObject.Type.CUSTOM_EVENT].Cast<BeatmapCustomEvent>().ToList();
        }
    }

    #endregion
	
	public void OnSelectDeselectAll(InputAction.CallbackContext context)
    {
        if (context.performed) SelectDeselectAll();
    }
	
    public void OnDeselectAll(InputAction.CallbackContext context)
    {
        if (context.performed) DeselectAll();
    }
	
	public void OnSelectAllFromBeat(InputAction.CallbackContext context)
	{
		if (context.performed) SelectAllFromBeat(atsc.CurrentBeat);
	}

    public void OnPaste(InputAction.CallbackContext context)
    {
        if (context.performed) Paste();
    }

    public void OnDeleteObjects(InputAction.CallbackContext context)
    {
        if (context.performed) Delete();
    }

    public void OnCopy(InputAction.CallbackContext context)
    {
        if (context.performed) Copy();
    }

    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed) Copy(true);
    }

    public void OnShiftinTime(InputAction.CallbackContext context)
    {
        if (!context.performed || !KeybindsController.ShiftHeld) return;
        float value = context.ReadValue<float>();
        MoveSelection(value * (1f / atsc.gridMeasureSnapping));
    }

    public void OnShiftinPlace(InputAction.CallbackContext context)
    {
        if (!context.performed || !KeybindsController.CtrlHeld) return;
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log(movement);
        ShiftSelection(Mathf.RoundToInt(movement.x), Mathf.RoundToInt(movement.y));
    }

}
