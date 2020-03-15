using UnityEngine;

public class BeatmapNoteContainer : BeatmapObjectContainer {

    public override BeatmapObject objectData { get => mapNoteData; set => mapNoteData = (BeatmapNote)value; }

    public BeatmapNote mapNoteData;
    public bool isBomb;

    [SerializeField] MeshRenderer modelRenderer;
    [SerializeField] SpriteRenderer dotRenderer;
    [SerializeField] MeshRenderer arrowRenderer;
    [SerializeField] SpriteRenderer swingArcRenderer;
    [SerializeField] NoteAppearanceSO noteAppearance; //Combining properties with SerializeField is bad idea

    private void Start()
    {
        SetArcVisible(NotesContainer.ShowArcVisualizer);
    }

    public void Directionalize(int cutDirection)
    {
        Vector3 directionEuler = Vector3.zero;
        switch (cutDirection)
        {
            case BeatmapNote.NOTE_CUT_DIRECTION_UP: directionEuler += new Vector3(0, 0, 180); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_DOWN: directionEuler += new Vector3(0, 0, 0); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_LEFT: directionEuler += new Vector3(0, 0, -90); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_RIGHT: directionEuler += new Vector3(0, 0, 90); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT: directionEuler += new Vector3(0, 0, 135); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT: directionEuler += new Vector3(0, 0, -135); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT: directionEuler += new Vector3(0, 0, -45); break;
            case BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT: directionEuler += new Vector3(0, 0, 45); break;
        }
        if (cutDirection >= 1000) directionEuler += new Vector3(0, 0, 360 - (cutDirection - 1000));
        if (transform != null) transform.localEulerAngles = directionEuler;
    }

    public void SetModelMaterial(Material m) {
        modelRenderer.sharedMaterial = m;
    }

    public void SetDotVisible(bool b) {
        dotRenderer.enabled = b;
    }

    public void SetArrowVisible(bool b) {
        arrowRenderer.enabled = b;
    }

    public void SetDotSprite(Sprite sprite) {
        dotRenderer.sprite = sprite;
    }

    public void SetArcVisible(bool ShowArcVisualizer)
    {
        if (swingArcRenderer != null) swingArcRenderer.enabled = ShowArcVisualizer;
    }

    public static BeatmapNoteContainer SpawnBeatmapNote(BeatmapNote noteData, AudioTimeSyncController atsc, ref GameObject notePrefab, ref GameObject bombPrefab, ref NoteAppearanceSO appearanceSO) {
        bool isBomb = noteData._type == BeatmapNote.NOTE_TYPE_BOMB;
        BeatmapNoteContainer container = Instantiate(isBomb ? bombPrefab : notePrefab).GetComponent<BeatmapNoteContainer>();
        container.isBomb = isBomb;
        container.mapNoteData = noteData;
        appearanceSO.SetNoteAppearance(container);
        container.noteAppearance = appearanceSO;
        container.Directionalize(noteData._cutDirection);
		container.audioTimeSyncController = atsc;
        return container;
    }

    public override void UpdateGridPosition() {
        float position = mapNoteData._lineIndex - 1.5f;
        float layer = mapNoteData._lineLayer + 0.5f;
        if (mapNoteData._lineIndex >= 1000)
            position = (mapNoteData._lineIndex / 1000f) - 2.5f;
        else if (mapNoteData._lineIndex <= -1000)
            position = (mapNoteData._lineIndex / 1000f) - 0.5f;
        if (mapNoteData._lineLayer >= 1000 || mapNoteData._lineLayer <= -1000)
            layer = (mapNoteData._lineLayer / 1000f) - 0.5f;
        transform.localPosition = new Vector3(
            position,
            layer,
            mapNoteData._time * EditorScaleController.EditorScale
            );

        if (modelRenderer.material.HasProperty("_Rotation"))
            modelRenderer.material.SetFloat("_Rotation", AssignedTrack?.RotationValue ?? 0);
    }

    internal override void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(2) && !KeybindsController.ShiftHeld && mapNoteData._type != BeatmapNote.NOTE_TYPE_BOMB)
        {
            if (mapNoteData is BeatmapChromaNote chroma) mapNoteData = chroma.originalNote; //Revert Chroma status, then invert types
            if (mapNoteData != null)
                mapNoteData._type = mapNoteData._type == BeatmapNote.NOTE_TYPE_A ? BeatmapNote.NOTE_TYPE_B : BeatmapNote.NOTE_TYPE_A;
            else Debug.LogWarning("Data attached to this note object is somehow null! This shouldn't happen!");
            noteAppearance.SetNoteAppearance(this);
        }else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        { // Edit existing note directions with Alt + Mouse Scroll
            if (KeybindsController.AltHeld)
            {
                if (mapNoteData._cutDirection == BeatmapNote.NOTE_CUT_DIRECTION_ANY) return;
                mapNoteData._cutDirection += (Input.GetAxis("Mouse ScrollWheel") > 0 ? -1 : 1);
                if (mapNoteData._cutDirection == -1) mapNoteData._cutDirection = 7;
                else if (mapNoteData._cutDirection == 8) mapNoteData._cutDirection = 0;
                Directionalize(mapNoteData._cutDirection);
            }
        } else if (KeybindsController.AltHeld && !KeybindsController.ShiftHeld && !KeybindsController.CtrlHeld && mapNoteData._type != BeatmapNote.NOTE_TYPE_BOMB) { // Edit existing note directions with Alt + WASD
			int oldCutDirection = mapNoteData._cutDirection;
			Debug.Log("oldCutDirection Initial: " + oldCutDirection);
			Debug.Log("mapNoteData._cutDirection Initial: " + mapNoteData._cutDirection);
			if (Input.GetKeyDown(KeyCode.W)) {
				if (Input.GetKey(KeyCode.A)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT;
				else if (Input.GetKey(KeyCode.D)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT;
				else mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_UP;
			} else if (Input.GetKeyDown(KeyCode.S)) {
				if (Input.GetKey(KeyCode.A)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT;
				else if (Input.GetKey(KeyCode.D)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT;
				else mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_DOWN;
			} else if (Input.GetKeyDown(KeyCode.A)) {
				if (Input.GetKey(KeyCode.W)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT;
				else if (Input.GetKey(KeyCode.S)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT;
				else mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_LEFT;
			} else if (Input.GetKeyDown(KeyCode.D)) {
				if (Input.GetKey(KeyCode.W)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT;
				else if (Input.GetKey(KeyCode.S)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT;
				else mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_RIGHT;
			} else if (Input.GetKeyDown(KeyCode.F)) mapNoteData._cutDirection = BeatmapNote.NOTE_CUT_DIRECTION_ANY;
			Debug.Log("oldCutDirection Final: " + oldCutDirection);
			Debug.Log("mapNoteData._cutDirection Final: " + mapNoteData._cutDirection);
			if (oldCutDirection != mapNoteData._cutDirection) Directionalize(mapNoteData._cutDirection);
		}
        else base.OnMouseOver();
    }
}
