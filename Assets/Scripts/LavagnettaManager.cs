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
    public TMP_Text message7;
    public TMP_Text message8;
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
                Instance.message7.text = "";
                Instance.message8.text = "";
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
                    Instance.message7.text = "";
                    Instance.message8.text = "";
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
                    Instance.message7.text = "";
                    Instance.message8.text = "";
                }
                else if (messages.Count == 2)
                {
                    Instance.infoCorretti.text = "2 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = "";
                    Instance.message4.text = "";
                    Instance.message5.text = "";
                    Instance.message6.text = "";
                    Instance.message7.text = "";
                    Instance.message8.text = "";
                }
                else if (messages.Count == 3)
                {
                    Instance.infoCorretti.text = "3 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = messages[2];
                    Instance.message4.text = "";
                    Instance.message5.text = "";
                    Instance.message6.text = "";
                    Instance.message7.text = "";
                    Instance.message8.text = "";
                }
                else if (messages.Count == 4)
                {
                    Instance.infoCorretti.text = "4 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = messages[2];
                    Instance.message4.text = messages[3];
                    Instance.message5.text = "";
                    Instance.message6.text = "";
                    Instance.message7.text = "";
                    Instance.message8.text = "";
                }
                else if (messages.Count == 5)
                {
                    Instance.infoCorretti.text = "5 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = messages[2];
                    Instance.message4.text = messages[3];
                    Instance.message5.text = messages[4];
                    Instance.message6.text = "";
                    Instance.message7.text = "";
                    Instance.message8.text = "";
                }
                else if (messages.Count == 6)
                {
                    Instance.infoCorretti.text = "6 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = messages[2];
                    Instance.message4.text = messages[3];
                    Instance.message5.text = messages[4];
                    Instance.message6.text = messages[5];
                    Instance.message7.text = "";
                    Instance.message8.text = "";
                }
                else if (messages.Count == 7)
                {
                    Instance.infoCorretti.text = "7 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = messages[2];
                    Instance.message4.text = messages[3];
                    Instance.message5.text = messages[4];
                    Instance.message6.text = messages[5];
                    Instance.message7.text = messages[6];
                    Instance.message8.text = "";
                }
                else if (messages.Count == 8)
                {
                    Instance.infoCorretti.text = "8 problemi rilevati";
                    Instance.message1.text = messages[0];
                    Instance.message2.text = messages[1];
                    Instance.message3.text = messages[2];
                    Instance.message4.text = messages[3];
                    Instance.message5.text = messages[4];
                    Instance.message6.text = messages[5];
                    Instance.message7.text = messages[6];
                    Instance.message8.text = messages[7];
            
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
