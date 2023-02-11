using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ZipperHandler : MonoBehaviour
{
    private bool _actualRaycast;
    private Outline _outline;
    private Grabber _grabber;

    private bool _operating;
    
    public GameObject InteractCanvas;

    public KeyCode KeyPressed;
    public Transform InZipperPosition;

    public Transform ObjHolder;

    public Animator Animator;
    private static readonly int Zipping = Animator.StringToHash("zipping");

    public GameObject ZipPrefab;

    public FirstPersonCharacterController Player;
    public Magnet0Raycaster Magnet0Raycaster;

    public TMP_Text ZipText;
    public TMP_Text UnzipText;

    private static bool _immaginiEVideoZipped;
    public static bool FirstHalfOfZipQuestCompleted;

    private void Start()
    {
        _outline = transform.GetComponent<Outline>();
    }

    public bool GetActualRaycast()
    {
        return _actualRaycast;
    }

    public void SetActualRaycast(bool value, Grabber grabber)
    {
        _actualRaycast = value;
        _outline.enabled = value;
        InteractCanvas.SetActive(value);
        if (value && grabber != null && (grabber.GetReferred() is Folder || (grabber.GetReferred() is RoomFile roomFile && roomFile.GetFormat() == "zip")))
        {
            _grabber = grabber;
        }
        else
        {
            _grabber = null;
        }
    }

    private void Update()
    {
        if (_operating) return;
        switch (_actualRaycast)
        {
            case false:
                return;
            case true when _grabber == null && Input.GetKeyDown(KeyPressed):
                NotificationManager.Notify(Operation.BringFolderToUseZipper);
                break;
            case true when _grabber != null && Input.GetKeyDown(KeyPressed):
                if (HouseManager.ActualQuest < 6)
                {
                    NotificationManager.Notify(Operation.LockedFunctionality);
                }
                else if (_grabber.GetReferred() is RoomFile roomFile && roomFile.GetFormat() == "zip")
                {
                    NotificationManager.Notify(Operation.UnzipNotAllowed);
                }
                else
                {
                    if (_immaginiEVideoZipped || 
                            (_grabber.GetReferred() is Folder && 
                                (_grabber.GetReferred().GetIndex() == Folder.ImmaginiEVideoFolder.GetIndex() 
                                || _grabber.GetReferred().IsACopyOf(Folder.ImmaginiEVideoFolder))))
                    {
                        if (!_immaginiEVideoZipped)
                        {
                            if (FirstHalfOfZipQuestCompleted)
                            {
                                _immaginiEVideoZipped = true;
                                _operating = true;
                                InteractCanvas.SetActive(false);
                                Player.IgnoreInput();
                                StartCoroutine(ZipFolder());
                            }
                            else
                            {
                                NotificationManager.Notify(Operation.CompleteFirstHalfOfZip);
                            }
                        }
                        else
                        {
                            _operating = true;
                            InteractCanvas.SetActive(false);
                            Player.IgnoreInput();
                            StartCoroutine(ZipFolder());
                        }
                    }
                    else
                    {
                        NotificationManager.Notify(Operation.ShouldBringImmaginiEVideoFolderToZip);
                    }
                }
                break;
        }
    }

    private IEnumerator ZipFolder()
    {
        _grabber.TriggerLabelGrabbed(false, "");
        StartCoroutine(FlashScritta(true));
        var tr = _grabber.transform.parent.parent.parent.parent;
        var rotationNotReached = true;
        var positionNotReached = true;
        while (positionNotReached || rotationNotReached)
        {
            positionNotReached = Vector3.Distance(tr.position, InZipperPosition.position) > 0.01f;
            rotationNotReached = Quaternion.Angle(tr.rotation, Quaternion.Euler(0, 0, 0)) > 0.01f;
            if (positionNotReached)
                tr.position = Vector3.MoveTowards(tr.position, InZipperPosition.position, Time.deltaTime * 2f);
            if (rotationNotReached)
                tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 8f);
            yield return null;
        }
        tr.SetParent(transform);
        Animator.SetBool(Zipping, true);
        while (Animator.GetCurrentAnimatorClipInfo(0).Length == 0)
        {
            yield return null;
        }
        yield return new WaitUntil(() => Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Armature|Zipper_Action");
        Animator.SetBool(Zipping, false);
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        _grabber.GetReferred().SetParentOnDeletionAbsolutePath(_grabber.GetReferred().GetParent()?.GetAbsolutePath());
        var referred = _grabber.GetReferred();
        Destroy(tr.gameObject);
        var objInstantiated = Instantiate(ZipPrefab, InZipperPosition);
        tr = objInstantiated.transform;
        tr.SetParent(ObjHolder);
        tr.localScale *= 0.75f;
        var fileGrabber = objInstantiated.transform.GetComponent<Grabber>();
        var zipFile = new RoomFile(referred.GetName().Split(".")[0] + ".zip", "zip", -1, 0, null, referred.IsACopy() ? referred.GetCopyOf() : referred.GetIndex(), Guid.NewGuid().ToString());
        fileGrabber.SetReferred(zipFile);
        yield return new WaitForSeconds(2f);
        positionNotReached = true;
        rotationNotReached = true;
        while (positionNotReached || rotationNotReached)
        {
            positionNotReached = Vector3.Distance(tr.localPosition, Vector3.zero) > 0.01f;
            rotationNotReached = Quaternion.Angle(tr.localRotation, Quaternion.Euler(0f, 0f, 0f)) > 0.01f;
            if (positionNotReached)
                tr.localPosition = Vector3.MoveTowards(tr.localPosition, Vector3.zero, Time.deltaTime * 32f);
            if (rotationNotReached)
                tr.localRotation = Quaternion.Slerp(tr.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 32f);
            yield return null;
        }
        fileGrabber.TriggerLabelGrabbed(true, zipFile.GetName().Trim());
        if (referred.GetParentOnDeletionAbsolutePath() != null)
            referred.Recover();
        else
            Folder.TriggerReloading(Operation.Nop);
        _grabber = fileGrabber;
        Magnet0Raycaster.SetGrabbedFile(_grabber);
        InteractCanvas.SetActive(_actualRaycast);
        _operating = false;
    }

    private IEnumerator FlashScritta(bool zip)
    {
        TMP_Text text;
        var zipCol = ZipText.color;
        var unzipCol = UnzipText.color;
        if (zip)
        {
            text = ZipText;
            unzipCol.a = 0f;
            UnzipText.color = unzipCol;
        }
        else
        {
            text = UnzipText;
            zipCol.a = 0f;
            ZipText.color = zipCol;
        }
        var col = zip ? zipCol : unzipCol;
        while (_operating)
        {
            while (col.a > 0.01f)
            {
                col.a = Mathf.Lerp(col.a, 0f, Time.deltaTime * 10f);
                text.color = col;
                yield return null;
            }
            while (col.a < 0.99f)
            {
                col.a = Mathf.Lerp(col.a, 1f, Time.deltaTime * 10f);
                text.color = col;
                yield return null;
            }
        }
        zipCol.a = 1f;
        ZipText.color = zipCol;
        unzipCol.a = 1f;
        UnzipText.color = unzipCol;
    }
}
