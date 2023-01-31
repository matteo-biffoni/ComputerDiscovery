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
        var roomPosition = _roomIn != Folder.Garage ? _roomIn.GetContainer().transform.position : Vector3.zero;
        return new Vector3(position.x - roomPosition.x, 0, position.z - roomPosition.z);
    }
}
