using UnityEngine;

public class StepwellTrigger : MonoBehaviour
{
    public GameObject playerObject;
    public PlayerController playerController;

    [Header("UI")]
    [SerializeField] private GameObject jumpHigherUi;

    private bool isNear;

    void Start()
    {
        jumpHigherUi.SetActive(false);
    }

    void Update()
    {
        if (isNear) {
            playerController.canJump = true;

            jumpHigherUi.SetActive(true);
        }
        else if (!isNear) {
            playerController.canJump = false;

            jumpHigherUi.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            isNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            isNear = false;
        }
    }
}
