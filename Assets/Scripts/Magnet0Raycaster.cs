using System.Collections;
using System.Collections.Generic;
using CartoonFX;
using UnityEngine;

public class Magnet0Raycaster : MonoBehaviour
{
    public float RaycastDistance;
    public PlayerNavigatorManager Player;

    private FileGrabber _grabbedFile;

    private FileGrabber _previousPointedFile;

    public GameObject Explosion;

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(transform.position, transform.forward);

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
            }
            if (fileGrabber)
            {
                _previousPointedFile = fileGrabber;
                fileGrabber.transform.GetComponent<Outline>().OutlineWidth = 7f;
                if (Input.GetMouseButtonDown(0))
                {
                    fileGrabber.transform.GetComponent<Outline>().OutlineWidth = 0f;
                    GrabFile(fileGrabber);
                    
                    //Debug.Log($"Raycast Hit Gameobject: {hit.transform.name}");
                }
            }
        }
        else if (_previousPointedFile != null)
        {
            _previousPointedFile.transform.GetComponent<Outline>().OutlineWidth = 0f;
            _previousPointedFile = null;
        }
        //Debug.DrawRay(transform.position, transform.forward * _raycastDistance, Color.red);
    }

    private void GrabFile(FileGrabber fileGrabber)
    {
        _grabbedFile = fileGrabber;
        // Settare la posizione corretta invece che transform
        fileGrabber.transform.SetParent(transform.Find("ObjHolder"));
        fileGrabber.transform.localPosition = new Vector3(0f, 0f, 0f);
        var defaultRotation = fileGrabber.transform.rotation.eulerAngles;
        fileGrabber.transform.localRotation = Quaternion.Euler(60f, 150f, -30f);
        fileGrabber.transform.Rotate(defaultRotation);
        fileGrabber.transform.localScale *= 0.75f;
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

    private static IEnumerator DeleteExplosion(GameObject explosion)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(explosion);
    }
}
