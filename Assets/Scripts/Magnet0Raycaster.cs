using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet0Raycaster : MonoBehaviour
{
    public float RaycastDistance;
    public Magnet0Movement Player;

    private FileGrabber _grabbedFile;

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

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
            if (fileGrabber)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GrabFile(fileGrabber);
                    
                    //Debug.Log($"Raycast Hit Gameobject: {hit.transform.name}");
                }
            }
        }
        //Debug.DrawRay(transform.position, transform.forward * _raycastDistance, Color.red);
    }

    private void GrabFile(FileGrabber fileGrabber)
    {
        _grabbedFile = fileGrabber;
        // Settare la posizione corretta invece che transform
        fileGrabber.transform.SetParent(transform);
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
        roomIn.InsertFile(_grabbedFile);
        Destroy(_grabbedFile.gameObject);
        // Bisognerebbe anche valutare i casi in cui ci sono già 10 file sulla bacheca della cartella in cui ci troviamo
        _grabbedFile = null;
    }
}
