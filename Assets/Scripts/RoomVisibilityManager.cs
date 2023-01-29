using UnityEngine;

public class RoomVisibilityManager : MonoBehaviour
{
    public static void ChangedRoom(Folder oldRoom, Folder newRoom)
    {
        if (newRoom == null || oldRoom == null) return;
        // Casi in cui va mostrata la MainRoom
        if (newRoom == Folder.MainRoom || newRoom == Folder.Root || newRoom.GetParent() == Folder.Root)
        {
            Folder.ShowMainRoom(true);
        }
        // Il player è entrato in un figlio
        if (oldRoom == newRoom.GetParent())
        {
            if (oldRoom.GetParent() != null && oldRoom.GetParent().GetParent() != null)
            {
                oldRoom.GetParent().GetParent().ActivateRoomComponents(false);
            }
        }
        // Il player è tornato nel padre
        else if (newRoom == oldRoom.GetParent())
        {
            if (newRoom.GetParent() != null && newRoom.GetParent().GetParent() != null)
            {
                newRoom.GetParent().GetParent().ActivateRoomComponents(true);
            }
        }
    }
}
