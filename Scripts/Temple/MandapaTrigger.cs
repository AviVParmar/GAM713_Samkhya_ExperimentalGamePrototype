using UnityEngine;

public class MandapaTrigger : MonoBehaviour
{
    public bool isInMandapa = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            isInMandapa = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            isInMandapa = false;
        }
    }
}
