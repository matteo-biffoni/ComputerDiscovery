using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private Folder _folderReferred;

    public void SetFolderReferred(Folder folder)
    {
        _folderReferred = folder;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            (other.GetComponent(typeof(PlayerNavigatorManager)) as PlayerNavigatorManager)?.SetRoomIn(_folderReferred);
        }
    }
}
