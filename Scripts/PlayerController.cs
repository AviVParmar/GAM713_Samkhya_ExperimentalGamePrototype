using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Initialisations
    [Header("References")]
    private CharacterController controller; // Character Controller
    private InputSystem_Actions inputSystem; // Input System
    private InputAction movementInputAction;
    private InputAction jumpInputAction;
    private InputAction interactInputAction;
    private InputAction attackInputAction;
    [SerializeField] private Transform holdPoint; // Empty object in front of the player
    [SerializeField] private Transform playerCamera; // Link to player camera
    public GameObject pressEToInteractObject;
    
    [Header("Interactable References")]
    [Header("Collage Interactable References")]
    public GameObject collageObject;
    public CollageInteractController collageInteractScript;

    [Header("Meditation Interactable References")]
    public GameObject meditationObject;
    public MeditationTrigger meditationInteractScript;

    [Header("season and ambience")]
    public GameObject seasonSwitcher;
    private GameObject audioAmbience;
    private AudioSource ambienceAudioSource;
    SeasonController seasonController;

    [Header("randomly generating objects")]
    GameObject randomlyGeneratingObject;
    

    [Header("Movement Settings")]
    [SerializeField] public bool canJump = false;
    [SerializeField] public bool canMove = true;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float turningSpeed = 5f;
    [SerializeField] private float jumpHeight = 500f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Hold Settings")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float holdDistance = 10f;
    public bool canPickup = true;
    private GameObject heldObject;
    private Rigidbody heldRb;

    private float verticalVelocity;

    [Header("Point System Settings")]
    PlayerPointSystemController playerPointSystemController;
    RandomlySpawnedObject randomlySpawnedObject_script;

    [Header("Input")]
    float moveInput;
    float turnInput;

    #endregion

    #region Awake, Enable, Disable
    private void Awake() {
        inputSystem = new InputSystem_Actions();

        pressEToInteractObject.SetActive(false);

        
    }

    private void OnEnable() {
        movementInputAction = inputSystem.Player.Move;
        movementInputAction.Enable();

        jumpInputAction = inputSystem.Player.Jump;
        jumpInputAction.Enable();
        jumpInputAction.performed += OnJump;

        interactInputAction = inputSystem.Player.Interact;
        interactInputAction.Enable();
        interactInputAction.performed += OnInteract;

        attackInputAction = inputSystem.Player.Attack;
        attackInputAction.Enable();
        attackInputAction.performed += OnPickup;
    }

    private void OnDisable() {
        movementInputAction.Disable();

        jumpInputAction.performed -= OnJump;
        jumpInputAction.Disable();

        interactInputAction.performed -=OnInteract;
        interactInputAction.Disable();

        attackInputAction.performed -= OnPickup;
        attackInputAction.Disable();
    }

    #endregion

    #region Start, Update
    private void Start() {
        controller = GetComponent<CharacterController>();

        playerPointSystemController = GetComponent<PlayerPointSystemController>();

        audioAmbience = GameObject.FindGameObjectWithTag("AmbienceAudio");
        ambienceAudioSource = audioAmbience.GetComponent<AudioSource>();

        seasonController = seasonSwitcher.GetComponent<SeasonController>();

        randomlyGeneratingObject = GameObject.FindGameObjectWithTag("RandomlySpawnedObjectInstantiater");
        randomlySpawnedObject_script = randomlyGeneratingObject.GetComponent<RandomlySpawnedObject>();
    }

    private void Update() {
        Movement_InputManagement();

        if (canMove)
        Movement();

        if (heldObject) {
            // Move the held object to the hold point
            heldObject.transform.position = holdPoint.transform.position;
        }
    }

    private void FixedUpdate() {
        EToInteractUI();

        if (!ambienceAudioSource.isPlaying && canMove)
        ambienceAudioSource.Play();
    }
    #endregion

    #region OnJump, OnInteract
    void OnJump(InputAction.CallbackContext context) {
        if (canJump)
        verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
    }

    void OnInteract(InputAction.CallbackContext context) {
        CollageMechanic();
        MeditationMechanic();
    }

    #region Interactable Functions

    void CollageMechanic() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.CompareTag("CollageInteractable") && playerPointSystemController.canCreate) {

                // Opening photo editing UI elements
                collageInteractScript.OpenPhotoEditor();

                //disable pickup
                canPickup = false;

                //disable movement
                canMove = false;

                //disable ambient audio
                if (ambienceAudioSource.isPlaying)
                ambienceAudioSource.Pause();

                // Increasing brahman points
                playerPointSystemController.AbstractCraftingSystem_DecreaseBrahman();
            }
        }
    }

    void MeditationMechanic() {
        if (meditationInteractScript.isNearMeditation) {
            canMove = false;
            canPickup = false;
            
            meditationInteractScript.OpenMeditationScene();
            playerPointSystemController.Meditation_IncreaseBrahman();

            if (ambienceAudioSource.isPlaying)
            ambienceAudioSource.Pause();

            seasonController.SwitchSeasons();

            randomlySpawnedObject_script.SpawnRandomObject();
        }
    }

    #endregion

    #endregion


    #region Movement Functions
    void Movement() {
        GroundMovement();

        CameraTurn_ThirdPerson();
    }

    void Movement_InputManagement() {
        //Input values
        Vector3 movementDirection = movementInputAction.ReadValue<Vector2>();

        //Assigning to move & turn
        moveInput = movementDirection.y;
        turnInput = movementDirection.x;
    }

    void GroundMovement() {
        Vector3 move = new Vector3(turnInput, 0, moveInput);

        // makes sure that the forward direction is the direction that the player is facing, and not the global direction
        move = transform.TransformDirection(move); // converts movement vector to player local axis
        

        //character gravity implementation
        move.y = VerticalForceCalculation();

        //applying movement speed
        move *= walkSpeed;

        //move the character
        controller.Move(move * Time.deltaTime);
    }

    private float VerticalForceCalculation() {
        if (controller.isGrounded) {
            verticalVelocity = 0f;
        }
        else {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }
    

    void CameraTurn_ThirdPerson() {
        // Prevent character rotation if stationary
        if (Mathf.Abs(turnInput) < 0 || Mathf.Abs(moveInput) > 0) {
            Vector3 currentLookDirection = playerCamera.forward; // Initialising current rotation
            currentLookDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection); // Initialisting target rotation

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turningSpeed); // Apply smooth rotation
        }
    }

    #endregion

    #region Holding Functions

    void OnPickup(InputAction.CallbackContext context) {
        if (heldObject) {
            Debug.Log("drop object");
            DropObject();
        }
        
        else if (!heldObject) {
                TryPickup();
            }   
    }

    void DropObject() {
        heldRb.isKinematic = false;
        heldObject.transform.SetParent(null);
        heldObject = null;
    }

    void TryPickup() {
        RaycastHit hit; // raycast to detect objects to pick up

        if (Physics.Raycast(transform.position, transform.forward, out hit, holdDistance)) {

            Debug.DrawRay(transform.position, transform.forward * holdDistance, Color.red, 2f);

            Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>(); // check for rigidbody
            //if (rb != null) {
                Debug.Log("pickup object");

                heldObject = hit.collider.gameObject;
                heldRb = heldObject.GetComponent<Rigidbody>();

                //Disable physics for holding
                heldRb.isKinematic = true;
                heldObject.transform.SetParent(holdPoint);
            //}
        }
    }
    #endregion

    #region e to interact

    void EToInteractUI() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.CompareTag("CollageInteractable")) {
                //Enable "press e to interact" UI element
                pressEToInteractObject.SetActive(true);
            }
            else  {

                if (meditationInteractScript.isNearMeditation) {
                    pressEToInteractObject.SetActive(true);
                }
                else if (!meditationInteractScript.isNearMeditation) {
                    pressEToInteractObject.SetActive(false);
                }
            }
        }
    }

    #endregion

}
