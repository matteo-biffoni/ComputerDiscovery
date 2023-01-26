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
    
    

    public Vector3 OffsetInTheRoom()
    {
        var position = transform.position;
        var roomPosition = _roomIn.GetContainer().transform.position;
        return new Vector3(position.x - roomPosition.x, 0, position.z - roomPosition.z);
    }
}
