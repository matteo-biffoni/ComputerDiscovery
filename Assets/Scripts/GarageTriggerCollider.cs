
using UnityEngine;

public class GarageTriggerCollider : MonoBehaviour
{
    public GameObject HouseGo;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HouseGo.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HouseGo.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
