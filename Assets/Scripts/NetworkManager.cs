using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkManager : MonoBehaviour
{
    private Outline _outline;
    private bool _actualRaycast;
    private bool _changeRaycast;
    private bool _startDialog;

    public KeyCode InteractWithAD5LKeyCode;
    public FirstPersonCharacterController Player;
    public NetworkBox NetworkBox;
    public GameObject InteractCanvas;
    public GameObject DialogueCanvas;
    public DialogueManager DialogueManager;
    public string ActorName;
    public Sprite ActorSprite;

    public string[] InfoDialog;

    public string[] OkInsertionInBox;

    public string[] ErrInsertionInBox;

    public string[] CameBackFromNetworkDialog;

    private Grabber _currentInserted;
    private bool _shouldLookAtPlayer = true;
    private bool _playerShouldLookAtMe;
    private Transform _lookAtMe;

    private AD5LNavController _ad5LNavController;
    private Quaternion _previousPlayerRotation;
    private Quaternion _previousCameraRotation;

    private bool _shouldListenForRaycastChanges = true;
    public static bool SendingScoperte;
    public static bool SendingImmaginiEVideoFolder;

    private void Awake()
    {
        SendingScoperte = false;
        SendingImmaginiEVideoFolder = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _outline = GetComponent<Outline>();
        _lookAtMe = transform.Find("AD5L_LookAt").transform;
        _ad5LNavController = GetComponent<AD5LNavController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_shouldListenForRaycastChanges)
        {
            if (_changeRaycast)
            {
                _changeRaycast = false;
                if (_actualRaycast)
                {
                    InteractCanvas.SetActive(true);
                    _startDialog = true;
                }
                else
                {
                    _startDialog = false;
                    InteractCanvas.SetActive(false);
                }

                AD5LOutline(_actualRaycast);
            }
        }

        if (_shouldLookAtPlayer)
        {
            var lookAtPlayer = Player.transform.position;
            lookAtPlayer.y = transform.position.y;
            transform.LookAt(lookAtPlayer);
        }

        if (_playerShouldLookAtMe)
        {
            var lookAtMeCorr = _lookAtMe.position;
            lookAtMeCorr.y = Player.transform.position.y;
            Player.transform.LookAt(lookAtMeCorr);
        }

        if (_startDialog)
        {
            if (Input.GetKeyDown(InteractWithAD5LKeyCode))
            {
                _startDialog = false;
                Player.IgnoreInput();
                AD5LOutline(false);
                InteractCanvas.SetActive(false);
                StartCoroutine(SimpleInteract());
            }
        }
    }

    private IEnumerator SimpleInteract()
    {
        _playerShouldLookAtMe = false;
        yield return SmoothTurnToAD5L();
        DialogueCanvas.SetActive(true);
        DialogueManager.OpenDialogue(EndDialogue, InfoDialog, ActorName, ActorSprite);
    }

    private IEnumerator SmoothTurnToAD5L()
    {
        _previousPlayerRotation = Player.transform.rotation;
        _previousCameraRotation = Player.transform.GetComponentInChildren<Camera>().transform.localRotation;
        var ad5LookAt = transform.Find("AD5L_LookAt").transform.position;
        var direction = (ad5LookAt - Player.transform.position).normalized;
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        var lookRotationCamera = Quaternion.LookRotation(direction);
        direction.y = 0;
        var lookRotation = Quaternion.LookRotation(direction);
        var cameraTo = Quaternion.Euler(new Vector3(lookRotationCamera.eulerAngles.x, cameraT.localRotation.eulerAngles.y,
            cameraT.localRotation.eulerAngles.z));
        while (Quaternion.Angle(Player.transform.rotation, lookRotation) > 0.1f)
        {
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, lookRotation, Time.deltaTime * 8f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, cameraTo, Time.deltaTime * 8f);
            yield return null;
        }
    }

    private IEnumerator SmoothReturnToPreviousOrientation()
    {
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        while (Quaternion.Angle(Player.transform.rotation, _previousPlayerRotation) > 0.1f)
        {
            Player.transform.rotation =
                Quaternion.Slerp(Player.transform.rotation, _previousPlayerRotation, Time.deltaTime * 12f);
            cameraT.localRotation =
                Quaternion.Slerp(cameraT.localRotation, _previousCameraRotation, Time.deltaTime * 12f);
            yield return null;
        }
        InteractCanvas.SetActive(_actualRaycast);
        AD5LOutline(_actualRaycast);
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        _shouldListenForRaycastChanges = true;
    }

    public IEnumerator FileInsertedInBox(Grabber grabber)
    {
        _shouldListenForRaycastChanges = false;
        _currentInserted = grabber;
        InteractCanvas.SetActive(false);
        yield return SmoothTurnToAD5L();
        DialogueCanvas.SetActive(true);
        switch (_currentInserted.GetReferred())
        {
            case Folder:
                DialogueManager.OpenDialogue(EndDialogueErr, ErrInsertionInBox, ActorName, ActorSprite);
                break;
            case RoomFile:
                DialogueManager.OpenDialogue(EndDialogueOk, OkInsertionInBox, ActorName, ActorSprite);
                break;
        }
    }

    public void SetActualRaycast(bool value)
    {
        _changeRaycast = true;
        _actualRaycast = value;
    }

    public bool GetActualRaycast()
    {
        return _actualRaycast;
    }

    public void CameBackFromNetwork()
    {
        Destroy(_currentInserted.gameObject);
        _currentInserted = null;
        _shouldLookAtPlayer = true;
        DialogueCanvas.SetActive(true);
        DialogueManager.OpenDialogue(EndDialogue, CameBackFromNetworkDialog, ActorName, ActorSprite);
    }
    

    private void AD5LOutline(bool show)
    {
        _outline.OutlineWidth = show ? 5f : 0f;
        _outline.enabled = show;
    }

    private void EndDialogue()
    {
        DialogueCanvas.SetActive(false);
        _playerShouldLookAtMe = false;
        StartCoroutine(SmoothReturnToPreviousOrientation());
        NetworkBox.ReOpenBox();
        if (SendingScoperte)
        {
            SendingScoperte = false;
            RoomFile.ScoperteFile = null;
            HouseManager.ActualQuest = 6;
            NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
            List<string> messagesToSend = new List<string>(new string[]
            {
                "Grazie Magnet0, ho ricevuto i tuoi documenti, non vedo l'ora di leggerli!"
            });
            LavagnettaManager.SpecialWriteOnLavagnetta( "BEN FATTO!", "Messaggio da ELECTR4", messagesToSend); 
        }
        else if (SendingImmaginiEVideoFolder)
        {
            SendingImmaginiEVideoFolder = false;
            Folder.ImmaginiEVideoFolder = null;
            HouseManager.ActualQuest = 7;
            GenerateMaliciousStructure();
            Player.EnableSlowMovementAndShakeCamera();
            NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Sta succedendo qualcosa di strano! Vai da Lamp per saperne di più!"));
        }
    }

    private static void GenerateMaliciousStructure()
    {
        var documentiGrabbable = Folder.FindGrabbableWithIndexFromRoot(Folder.DocumentiIndex);
        Folder documenti;
        if (documentiGrabbable is not Folder)
        {
            documenti = new Folder("Documenti", Folder.Root, null, Guid.NewGuid().ToString());
            Folder.DocumentiIndex = documenti.GetIndex();
        }
        else
        {
            documenti = documentiGrabbable as Folder;
        }
        var lavoro = new Folder("Lavoro", documenti, null, Guid.NewGuid().ToString());
        var ricerche = new Folder("Ricerche", lavoro, null, Guid.NewGuid().ToString());
        var newEra1320 = new Folder("1320 Nuova Era", ricerche, null, Guid.NewGuid().ToString());
        var newEra1320Luoghi = new Folder("Luoghi", newEra1320, null, Guid.NewGuid().ToString());
        var apex21 = new RoomFile("Apex 21.docx", "doc", -1, 70, newEra1320Luoghi, null, Guid.NewGuid().ToString());
        var venut = new RoomFile("Venut.docx", "doc", -1, 70, newEra1320Luoghi, null, Guid.NewGuid().ToString());
        var combor14 = new RoomFile("Combor 14.docx", "doc", -1, 70, newEra1320Luoghi, null, Guid.NewGuid().ToString());
        var cocun = new RoomFile("Cocun.docx", "doc", -1, 70, newEra1320Luoghi, null, Guid.NewGuid().ToString());
        var malicious1 = new RoomFile("101001.zip", "zip", -1, 70, newEra1320Luoghi, null, Guid.NewGuid().ToString());
        var newEra1320LuoghiFiles = new List<RoomFile> { apex21, venut, combor14, cocun, malicious1 };
        newEra1320Luoghi.SetFiles(newEra1320LuoghiFiles);
        var newEra1320Specie = new Folder("Specie", newEra1320, null, Guid.NewGuid().ToString());
        var rettili1 = new RoomFile("Rettili.pdf", "pdf", -1, 70, newEra1320Specie, null, Guid.NewGuid().ToString());
        var anfibi = new RoomFile("Anfibi.pdf", "pdf", -1, 70, newEra1320Specie, null, Guid.NewGuid().ToString());
        var amangore = new RoomFile("Amangore.pdf", "pdf", -1, 70, newEra1320Specie, null, Guid.NewGuid().ToString());
        var sferilli = new RoomFile("Sferilli.pdf", "pdf", -1, 70, newEra1320Specie, null, Guid.NewGuid().ToString());
        var newEra1320SpecieFiles = new List<RoomFile> { rettili1, anfibi, amangore, sferilli };
        newEra1320Specie.SetFiles(newEra1320SpecieFiles);
        var newEra1321 = new Folder("1321 Nuova Era", ricerche, null, Guid.NewGuid().ToString());
        var newEra1321Luoghi = new Folder("Luoghi", newEra1321, null, Guid.NewGuid().ToString());
        var ratat = new RoomFile("Ratat.docx", "doc", -1, 70, newEra1321Luoghi, null, Guid.NewGuid().ToString());
        var menep = new RoomFile("Menep.docx", "doc", -1, 70, newEra1321Luoghi, null, Guid.NewGuid().ToString());
        var peril = new RoomFile("Peril.docx", "doc", -1, 70, newEra1321Luoghi, null, Guid.NewGuid().ToString());
        var tagheri = new RoomFile("Tagheri.docx", "doc", -1, 70, newEra1321Luoghi, null, Guid.NewGuid().ToString());
        var newEra1321LuoghiFiles = new List<RoomFile> { ratat, menep, peril, tagheri };
        newEra1321Luoghi.SetFiles(newEra1321LuoghiFiles);
        var newEra1321Specie = new Folder("Specie", newEra1321, null, Guid.NewGuid().ToString());
        var righettiAlati = new RoomFile("RighettiAlati.pdf", "pdf", -1, 70, newEra1321Specie, null,
            Guid.NewGuid().ToString());
        var rettili2 = new RoomFile("Rettili.pdf", "pdf", -1, 70, newEra1321Specie, null,
            Guid.NewGuid().ToString());
        var serperossa = new RoomFile("Serperossa.pdf", "pdf", -1, 70, newEra1321Specie, null,
            Guid.NewGuid().ToString());
        var malicious2 = new RoomFile("011010.zip", "zip", -1, 70, newEra1321Specie, null, Guid.NewGuid().ToString());
        var newEra1321SpecieFiles = new List<RoomFile> { righettiAlati, rettili2, serperossa, malicious2 };
        newEra1321Specie.SetFiles(newEra1321SpecieFiles);
        var articoliImportanti = new Folder("Articoli importanti", lavoro, null, Guid.NewGuid().ToString());
        var scopertaDiCiviltà132 = new RoomFile("Scoperta della civilità 10Uni.pdf", "pdf", -1, 70, articoliImportanti,
            null, Guid.NewGuid().ToString());
        var potenzialitàMacchineALaser = new RoomFile("Potenzialità macchine a laser.pdf", "pdf", -1, 70,
            articoliImportanti, null, Guid.NewGuid().ToString());
        var ad5LAiutoOMinaccia = new RoomFile("Robot AD5L: aiuti o minacce?.pdf", "pdf", -1, 70, articoliImportanti,
            null, Guid.NewGuid().ToString());
        var articoliImportantiFiles = new List<RoomFile>
            { scopertaDiCiviltà132, potenzialitàMacchineALaser, ad5LAiutoOMinaccia };
        articoliImportanti.SetFiles(articoliImportantiFiles);
        var personali = new Folder("Personali", documenti, null, Guid.NewGuid().ToString());
        var diariDiBordo = new Folder("Diari di bordo", personali, null, Guid.NewGuid().ToString());
        var viaggio10Dcem = new Folder("Viaggio su 10DCem", diariDiBordo, null, Guid.NewGuid().ToString());
        var luoghiEsplorati1 = new RoomFile("Luoghi esplorati.docx", "doc", -1, 70, viaggio10Dcem, null,
            Guid.NewGuid().ToString());
        var lingueLocali1 = new RoomFile("Lingue locali.docx", "doc", -1, 70, viaggio10Dcem, null,
            Guid.NewGuid().ToString());
        var pensieriNotturni1 = new RoomFile("Pensieri notturni.txt", "txt", -1, 70, viaggio10Dcem, null,
            Guid.NewGuid().ToString());
        var viaggio10DCemFiles = new List<RoomFile> { luoghiEsplorati1, lingueLocali1, pensieriNotturni1 };
        viaggio10Dcem.SetFiles(viaggio10DCemFiles);
        var viaggioaCakdet2 = new Folder("Viaggio a Cakdet2", diariDiBordo, null, Guid.NewGuid().ToString());
        var luoghiEsplorati2 = new RoomFile("Luoghi esplorati.docx", "doc", -1, 70, viaggioaCakdet2, null,
            Guid.NewGuid().ToString());
        var lingueLocali2 = new RoomFile("Lingue locali.docx", "doc", -1, 70, viaggioaCakdet2, null,
            Guid.NewGuid().ToString());
        var pensieriNotturni2 = new RoomFile("Pensieri notturni.txt", "txt", -1, 70, viaggioaCakdet2, null,
            Guid.NewGuid().ToString());
        var malicious3 = new RoomFile("010100.zip", "zip", -1, 70, viaggioaCakdet2, null, Guid.NewGuid().ToString());
        var viaggioaCakdet2Files = new List<RoomFile>
            { luoghiEsplorati2, lingueLocali2, pensieriNotturni2, malicious3 };
        viaggioaCakdet2.SetFiles(viaggioaCakdet2Files);
        var letteraturaTerrestre = new Folder("Letteratura Terrestre", personali, null, Guid.NewGuid().ToString());
        var sullaStrada = new RoomFile("Sulla strada.pdf", "pdf", -1, 70, letteraturaTerrestre, null,
            Guid.NewGuid().ToString());
        var cuoreDiTenebre = new RoomFile("Cuore di tenebre.pdf", "pdf", -1, 70, letteraturaTerrestre, null,
            Guid.NewGuid().ToString());
        var ilDesertoDeiTartari = new RoomFile("Il deserto dei tartari.pdf", "pdf", -1, 70, letteraturaTerrestre, null,
            Guid.NewGuid().ToString());
        var letteraturaTerrestreFiles = new List<RoomFile> { sullaStrada, cuoreDiTenebre, ilDesertoDeiTartari };
        letteraturaTerrestre.SetFiles(letteraturaTerrestreFiles);
        diariDiBordo.GetChildren().Add(viaggio10Dcem);
        diariDiBordo.GetChildren().Add(viaggioaCakdet2);
        newEra1320.GetChildren().Add(newEra1320Luoghi);
        newEra1320.GetChildren().Add(newEra1320Specie);
        newEra1321.GetChildren().Add(newEra1321Luoghi);
        newEra1321.GetChildren().Add(newEra1321Specie);
        ricerche.GetChildren().Add(newEra1320);
        ricerche.GetChildren().Add(newEra1321);
        personali.GetChildren().Add(diariDiBordo);
        personali.GetChildren().Add(letteraturaTerrestre);
        lavoro.GetChildren().Add(ricerche);
        lavoro.GetChildren().Add(articoliImportanti);
        documenti.GetChildren().Add(lavoro);
        documenti.GetChildren().Add(personali);
        Folder.TriggerReloading(Operation.Nop);
    }

    private void EndDialogueOk()
    {
        DialogueCanvas.SetActive(false);
        _shouldLookAtPlayer = false;
        _playerShouldLookAtMe = true;
        _ad5LNavController.enabled = true;
        _ad5LNavController.StartCoroutine(_ad5LNavController.SendBoxInNetwork());
    }

    private void EndDialogueErr()
    {
        DialogueCanvas.SetActive(false);
        if (_currentInserted.GetReferred().GetParent() != null)
        {
            _currentInserted.GetReferred().SetParentOnDeletionAbsolutePath(_currentInserted.GetReferred().GetParent().GetAbsolutePath());
            _currentInserted.Recover();
        }
        Destroy(_currentInserted.transform.parent.parent.parent.parent.gameObject);
        NetworkBox.ReOpenBox();
        _currentInserted = null;
        StartCoroutine(SmoothReturnToPreviousOrientation());
    }
}
