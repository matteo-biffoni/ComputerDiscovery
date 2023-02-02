using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LavagnettaManager : MonoBehaviour
{
    
    private static LavagnettaManager Instance;
    public TMP_Text message1;
    public TMP_Text message2;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance.message1.text = "Ciao";
        Instance.message2.text = "Lavagnetta";
    }

    public static void WriteOnLavagnetta(List<string> messages)
    {
        if (messages.Count == 0)
        {
            Instance.message1.text = "";
            Instance.message2.text = "";
        }
        else if (messages.Count == 1)
        {
            Instance.message1.text = messages[0]; 
            Instance.message2.text = "";
        }
        else if (messages.Count == 2)
        {
            Instance.message1.text = messages[0]; 
            Instance.message2.text = messages[1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
