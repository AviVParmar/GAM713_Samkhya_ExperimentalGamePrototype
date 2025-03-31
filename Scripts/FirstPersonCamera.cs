using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public GameObject playerBody;
    public Transform playerTransform;
    public PlayerController playerController;
    public float mouseSensitivity = 100f;

    private Vector2 lookInput;
    private InputSystem_Actions inputSystem;
    private InputAction lookAction;
    float xRotation;

    void Awake()
    {
        inputSystem = new InputSystem_Actions();
    }

    void OnEnable()
    {
        lookAction = inputSystem.Player.Look;
        lookAction.Enable();
        lookAction.performed += OnLook;
    }

    void OnDisable()
    {
        lookAction.performed -= OnLook;
        lookAction.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center
    }

    void OnLook(InputAction.CallbackContext context) {
        lookInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        
    }
}
