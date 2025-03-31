using UnityEngine;

public class CollageHouse : MonoBehaviour
{
    public bool isInCollageHouse = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            isInCollageHouse = true;
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            isInCollageHouse = false;
        }
    }
}
