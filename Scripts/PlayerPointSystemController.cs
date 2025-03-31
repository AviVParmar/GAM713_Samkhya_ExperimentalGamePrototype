using System.Collections;
using UnityEngine;

public class PlayerPointSystemController : MonoBehaviour
{
    [Header("Initialisations")]
    public GameObject pointSystemObject;
    public PointSystem pointSystemScript;

    [Header("Point System Settings")]
    [SerializeField] public int brahman;
    [SerializeField][Range(1f, 10f)] private float pointIncreaseSpeed;
    [SerializeField][Range(0.1f, 1f)] private float pointDecreaseSpeed;

    [Header("Mandapa Settings")]
    [SerializeField] private GameObject mandapaTrigger;
    [SerializeField] private MandapaTrigger mandapaTriggerScript;
    [SerializeField] public bool canMeditate = true;
    [SerializeField] public bool canCreate = true;

    void Start()
    {
        brahman = 50;

        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center
        Cursor.visible = false; // Hides the cursor
    }

    void Update()
    {
        PointSystem();

        setTrue();
    }


    #region Abstract Crafting Functions

    public void AbstractCraftingSystem_DecreaseBrahman() {
        StartCoroutine(DecreaseBrahmanBy(10));
    }

    public void Meditation_IncreaseBrahman() {
        Debug.Log("increasing brahman");
        StartCoroutine(IncreaseBrahman());
    }

    #endregion

    #region Point System Functions
    void PointSystem() {

        pointSystemScript.SetBrahman(brahman);

    }

    void setTrue() {
        if (brahman > 0 && brahman < 100) {
            canCreate = true;
            canMeditate = true;
        }
    }

    IEnumerator IncreaseBrahman() {
        while (brahman <= 100) {
            brahman++;

            if (brahman == 100) {
                canMeditate = false;
                canCreate = true;

                yield break;
            }

            yield return new WaitForSeconds(pointIncreaseSpeed);
        }
    }

    IEnumerator DecreaseBrahmanBy(int amount) {
        int endBrahman = brahman - amount;
        while (brahman > endBrahman) {
            brahman--;

            if (brahman == 0) {
                canCreate = false;
                canMeditate = true;

                yield break;
            }

            yield return new WaitForSeconds(pointDecreaseSpeed);
        }
    }


    public void BrahmanDecreaseByAmount(int amount) {
        brahman -= amount;
    }
    #endregion

}
