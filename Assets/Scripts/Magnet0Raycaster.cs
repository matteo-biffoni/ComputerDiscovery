using UnityEngine;

public class Magnet0Raycaster : MonoBehaviour
{
    public float RaycastDistance;
    
    public PlayerNavigatorManager Player;

    private FileGrabber _grabbedFile;

    private FileGrabber _previousPointedFile;

    public GameObject Explosion;

    // Update is called once per frame
    private void Update()
    {
        var t = transform;
        var ray = new Ray(t.position, t.forward);

        // Rilascio del file


        if (_grabbedFile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DropFile();
            }
            return;
        }
        if (Physics.Raycast(ray, out var hit, RaycastDistance))
        {
            var fileGrabber = hit.transform.GetComponent<FileGrabber>();
            if (_previousPointedFile != null && _previousPointedFile != fileGrabber)
            {
                _previousPointedFile.transform.GetComponent<Outline>().OutlineWidth = 0f;
                _previousPointedFile.TriggerLabel(false, null);
            }
            if (fileGrabber)
            {
                _previousPointedFile = fileGrabber;
                fileGrabber.transform.GetComponent<Outline>().OutlineWidth = 7f;
                fileGrabber.TriggerLabel(true, Player.transform.GetComponentInChildren<Camera>().transform);
                if (Input.GetMouseButtonDown(0))
                {
                    fileGrabber.transform.GetComponent<Outline>().OutlineWidth = 0f;
                    _grabbedFile = fileGrabber;
                    _grabbedFile.GrabFile(Player.transform.GetComponentInChildren<Camera>().transform, transform.Find("ObjHolder"));
                }
            }
        }
        else if (_previousPointedFile != null)
        {
            _previousPointedFile.transform.GetComponent<Outline>().OutlineWidth = 0f;
            _previousPointedFile.TriggerLabel(false, null);
            _previousPointedFile = null;
        }
    }

    private void DropFile()
    {
        if (!_grabbedFile) return;
        // Trovare la stanza in cui si trova il giocatore (ricordarsi che qui siamo nella camera, non in magnet0
        var roomIn = Player.GetRoomIn();
        // Una volta trovata la stanza prendere la bacheca di quella stanza e aggiungere correttamente il File
        // Se ci troviamo in MainRoom fare in modo che venga messo sulla bacheca del desktop
        if (Folder.MainRoom == roomIn)
        {
            roomIn = Folder.Root;
        }
        _grabbedFile.DropFile(Player.transform, roomIn, Explosion);
        _grabbedFile = null;
    }
}
