
using UnityEngine;

public class GarageTriggerCollider : MonoBehaviour
{
    public GameObject HouseGo;
    private void OnTriggerEnter(Collider other)
    {
        HouseGo.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        HouseGo.transform.GetChild(1).gameObject.SetActive(true);
    }
}
