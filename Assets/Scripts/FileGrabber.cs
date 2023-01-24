using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileGrabber : MonoBehaviour
{
    private RoomFile _file;

    public void SetFile(RoomFile file)
    {
        _file = file;
    }

    public RoomFile GetFile()
    {
        return _file;
    }
}
