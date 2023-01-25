using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNavigatorManager : MonoBehaviour
{
    private Folder _roomIn;

    public Folder GetRoomIn()
    {
        return _roomIn;
    }

    public void SetRoomIn(Folder roomIn)
    {
        var oldRoom = _roomIn;
        _roomIn = roomIn;
        RoomVisibilityManager.ChangedRoom(oldRoom, _roomIn);
    }
}
