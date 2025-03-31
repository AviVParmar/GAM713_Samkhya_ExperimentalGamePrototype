using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public bool isNearDoor = false;
    private bool isOpen = false;

    private Animator doorAnimator;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        if (doorAnimator != null) {
            if (isNearDoor && !isOpen) {
                StartCoroutine(OpenDoor());
            }
        }
    }

    IEnumerator OpenDoor() {
        Debug.Log("door is open");
        doorAnimator.SetTrigger("TriggerOpen");
        isOpen = true;
        yield return new WaitForSeconds(5f);

        Debug.Log("door is closed");
        isOpen = false;
        doorAnimator.SetTrigger("TriggerClose");
        yield break;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        isNearDoor = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        isNearDoor = false;
    }
}
