using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookmarkManager : MonoBehaviour, CMInput.IBookmarksActions
{
    internal List<BookmarkContainer> bookmarkContainers = new List<BookmarkContainer>();
    [SerializeField] private GameObject bookmarkContainerPrefab;
    public AudioTimeSyncController atsc;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); //Wait for time
        foreach(BeatmapBookmark bookmark in BeatSaberSongContainer.Instance.map._bookmarks)
        {
            GameObject container = Instantiate(bookmarkContainerPrefab, transform);
            container.name = bookmark._name;
            container.GetComponent<BookmarkContainer>().Init(this, bookmark);
            bookmarkContainers.Add(container.GetComponent<BookmarkContainer>());
        }   
    }
	
	private void JumpToNextBookmark()
	{
		BookmarkContainer targetBookmark = bookmarkContainers.FindAll(f => f.data._time > atsc.CurrentBeat).OrderBy(o => o.data._time).FirstOrDefault();
		if (targetBookmark != null) atsc.MoveToTimeInBeats(targetBookmark.data._time);
		else Debug.Log("No future bookmarks found");
	}
	
	private void JumpToPreviousBookmark()
	{
		BookmarkContainer targetBookmark = bookmarkContainers.FindAll(f => f.data._time < atsc.CurrentBeat).OrderByDescending(o => o.data._time).FirstOrDefault();
		if (targetBookmark != null) atsc.MoveToTimeInBeats(targetBookmark.data._time);
		else Debug.Log("No past bookmarks found");
	}

    public void OnCreateNewBookmark(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PersistentUI.Instance.ShowInputBox("Enter the name of this new Bookmark.", HandleNewBookmarkName, "New Bookmark");
        }
    }

    private void HandleNewBookmarkName(string res)
    {
        if (string.IsNullOrEmpty(res) || string.IsNullOrWhiteSpace(res)) return;
        BeatmapBookmark newBookmark = new BeatmapBookmark(atsc.CurrentBeat, res);
        GameObject container = Instantiate(bookmarkContainerPrefab, transform);
        container.name = newBookmark._name;
        container.GetComponent<BookmarkContainer>().Init(this, newBookmark);
        bookmarkContainers.Add(container.GetComponent<BookmarkContainer>());
        BeatSaberSongContainer.Instance.map._bookmarks = bookmarkContainers.Select(x => x.data).ToList();
    }
	
	public void OnJumpToNextBookmark(UnityEngine.InputSystem.InputAction.CallbackContext context)
	{
		if (context.performed) JumpToNextBookmark();
	}
	
	public void OnJumpToPreviousBookmark(UnityEngine.InputSystem.InputAction.CallbackContext context)
	{
		if (context.performed) JumpToPreviousBookmark();
	}
}
