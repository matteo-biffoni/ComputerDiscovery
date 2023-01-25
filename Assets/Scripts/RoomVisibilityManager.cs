using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVisibilityManager : MonoBehaviour
{
    public static void ChangedRoom(Folder oldRoom, Folder newRoom)
    {
        if (newRoom == null || oldRoom == null) return;
        // Casi in cui va mostrata la MainRoom
        if (newRoom == Folder.MainRoom || newRoom == Folder.Root || newRoom.GetFather() == Folder.Root)
        {
            Folder.ShowMainRoom(true);
        }
        // Il player è entrato in un figlio
        if (oldRoom == newRoom.GetFather())
        {
            if (oldRoom.GetFather() != null && oldRoom.GetFather().GetFather() != null)
            {
                oldRoom.GetFather().GetFather().ActivateRoomComponents(false);
            }
        }
        // Il player è tornato nel padre
        else if (newRoom == oldRoom.GetFather())
        {
            if (newRoom.GetFather() != null && newRoom.GetFather().GetFather() != null)
            {
                newRoom.GetFather().GetFather().ActivateRoomComponents(true);
            }
        }
    }
}
