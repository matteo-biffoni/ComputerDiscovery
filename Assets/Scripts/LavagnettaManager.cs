using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LavagnettaManager : MonoBehaviour
{
    
    private static LavagnettaManager Instance;
    public TMP_Text message1;
    public TMP_Text message2;
    public TMP_Text message3;
    public TMP_Text message4;
    public TMP_Text message5;
    public TMP_Text message6;
    public TMP_Text infoGenerali;
    public TMP_Text infoCorretti;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance.infoGenerali.text = "BENTORNATO!";
    }

    public static void WriteOnLavagnetta(List<string> messages, string info)
    {
        Instance.infoGenerali.text = info;
        switch (messages)
        {
            case null:
                Instance.infoCorretti.text = "";
                Instance.message1.text = "";
                Instance.message2.text = "";
                Instance.message3.text = "";
                Instance.message4.text = "";
                Instance.message5.text = "";
                Instance.message6.text = "";
                break;
            default:
            {
                if (messages.Count == 0)
                {
                    Instance.infoCorretti.text = "Nessun problema rilevato";
                    Instance.message1.text = "";
                    Instance.message2.text = "";
                    Instance.message3.text = "";
                    Instance.message4.text = "";
                    Instance.message5.text = "";
                    Instance.message6.text = "";
                }
                else if (messages.Count == 1)
                {
                    Instance.infoCorretti.text = "1 problema rilevato";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = "";
                    Instance.message3.text = "";
                    Instance.message4.text = "";
                    Instance.message5.text = "";
                    Instance.message6.text = "";
                }
                else if (messages.Count == 2)
                {
                    Instance.infoCorretti.text = "2 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = "*  *  *  *  *  *";
                    Instance.message3.text = messages[1];
                    Instance.message4.text = "";
                    Instance.message5.text = "";
                    Instance.message6.text = "";
                }
                else if (messages.Count == 3)
                {
                    Instance.infoCorretti.text = "3 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = "*  *  *  *  *  *";
                    Instance.message3.text = messages[1];
                    Instance.message4.text = "*  *  *  *  *  *";
                    Instance.message5.text = messages[2];
                    Instance.message6.text = "";
                }
                else if (messages.Count > 3)
                {
                    Instance.infoCorretti.text = $"{messages.Count} problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = "*  *  *  *  *  *";
                    Instance.message3.text = messages[1];
                    Instance.message4.text = "*  *  *  *  *  *";
                    Instance.message5.text = messages[2];
                    Instance.message6.text = ".....altri";
                }
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
