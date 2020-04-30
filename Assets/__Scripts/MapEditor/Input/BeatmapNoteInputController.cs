using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BeatmapNoteInputController : BeatmapInputController<BeatmapNoteContainer>, CMInput.INoteObjectsActions
{
    private Dictionary<int, int> CutDirectionMovedForward = new Dictionary<int, int>
    {
        { BeatmapNote.NOTE_CUT_DIRECTION_ANY, BeatmapNote.NOTE_CUT_DIRECTION_ANY },
        { BeatmapNote.NOTE_CUT_DIRECTION_DOWN, BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT },
        { BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT, BeatmapNote.NOTE_CUT_DIRECTION_LEFT },
        { BeatmapNote.NOTE_CUT_DIRECTION_LEFT, BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT },
        { BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT, BeatmapNote.NOTE_CUT_DIRECTION_UP },
        { BeatmapNote.NOTE_CUT_DIRECTION_UP, BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT },
        { BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT, BeatmapNote.NOTE_CUT_DIRECTION_RIGHT },
        { BeatmapNote.NOTE_CUT_DIRECTION_RIGHT, BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT },
        { BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT, BeatmapNote.NOTE_CUT_DIRECTION_DOWN },
        { BeatmapNote.NOTE_CUT_DIRECTION_NONE, BeatmapNote.NOTE_CUT_DIRECTION_NONE }
    };

    private Dictionary<int, int> CutDirectionMovedBackward = new Dictionary<int, int>
    {
        { BeatmapNote.NOTE_CUT_DIRECTION_ANY, BeatmapNote.NOTE_CUT_DIRECTION_ANY },
        { BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT, BeatmapNote.NOTE_CUT_DIRECTION_DOWN },
        { BeatmapNote.NOTE_CUT_DIRECTION_LEFT, BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT },
        { BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT, BeatmapNote.NOTE_CUT_DIRECTION_LEFT },
        { BeatmapNote.NOTE_CUT_DIRECTION_UP, BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT },
        { BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT, BeatmapNote.NOTE_CUT_DIRECTION_UP },
        { BeatmapNote.NOTE_CUT_DIRECTION_RIGHT, BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT },
        { BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT, BeatmapNote.NOTE_CUT_DIRECTION_RIGHT },
        { BeatmapNote.NOTE_CUT_DIRECTION_DOWN, BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT },
        { BeatmapNote.NOTE_CUT_DIRECTION_NONE, BeatmapNote.NOTE_CUT_DIRECTION_NONE }
    };

    [SerializeField] private NoteAppearanceSO noteAppearanceSO;
	private bool upNote = false;
    private bool leftNote = false;
    private bool downNote = false;
    private bool rightNote = false;

    //Do some shit later lmao
    public void OnInvertNoteColors(InputAction.CallbackContext context)
    {
        if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
        if (!KeybindsController.AnyCriticalKeys)
        {
            RaycastFirstObject(out BeatmapNoteContainer note);
            if (note != null && note.mapNoteData._type != BeatmapNote.NOTE_TYPE_BOMB)
            {
                int newType = note.mapNoteData._type == BeatmapNote.NOTE_TYPE_A ? BeatmapNote.NOTE_TYPE_B : BeatmapNote.NOTE_TYPE_A;
                note.mapNoteData._type = newType;
                noteAppearanceSO.SetNoteAppearance(note);
            }
        }
    }
	
	private void UpdateNoteDirection(int _type){
		if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
		RaycastFirstObject(out BeatmapNoteContainer note);
		if (note != null)
		{
			note.mapNoteData._cutDirection = _type;
			note.Directionalize(note.mapNoteData._cutDirection);
		}
	}

    public void OnUpdateNoteDirection(InputAction.CallbackContext context)
    {
        if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
        if (KeybindsController.AltHeld)
        {
            bool shiftForward = context.ReadValue<float>() > 0;
            RaycastFirstObject(out BeatmapNoteContainer note);
            if (note != null)
            {
                if (shiftForward)
                    note.mapNoteData._cutDirection = CutDirectionMovedForward[note.mapNoteData._cutDirection];
                else note.mapNoteData._cutDirection = CutDirectionMovedBackward[note.mapNoteData._cutDirection];
                note.Directionalize(note.mapNoteData._cutDirection);
            }
        }
    }
	
	public void OnUpdateNoteDirectionDot(InputAction.CallbackContext context)
    {
		if(context.performed) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_ANY);
	}
	
	public void OnUpdateNoteDirectionDown(InputAction.CallbackContext context)
    {
        downNote = context.performed;
        if (!downNote) return;
        if (!leftNote && !rightNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_DOWN);
        else if (leftNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT);
        else if (rightNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT);
    }

    public void OnUpdateNoteDirectionLeft(InputAction.CallbackContext context)
    {
        leftNote = context.performed;
        if (!leftNote) return;
        if (!upNote && !downNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_LEFT);
        else if (upNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT);
        else if (downNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_DOWN_LEFT);
    }

    public void OnUpdateNoteDirectionUp(InputAction.CallbackContext context)
    {
        upNote = context.performed;
        if (!upNote) return;
        if (!leftNote && !rightNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_UP);
        else if (leftNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_UP_LEFT);
        else if (rightNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT);
    }

    public void OnUpdateNoteDirectionRight(InputAction.CallbackContext context)
    {
        rightNote = context.performed;
        if (!rightNote) return;
        if (!upNote && !downNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_RIGHT);
        else if (upNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_UP_RIGHT);
        else if (downNote) UpdateNoteDirection(BeatmapNote.NOTE_CUT_DIRECTION_DOWN_RIGHT);
    }
	
}
