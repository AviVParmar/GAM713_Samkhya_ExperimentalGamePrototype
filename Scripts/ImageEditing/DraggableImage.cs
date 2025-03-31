using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DraggableImage : MonoBehaviour
{
    private Vector2 touchStartPos;
    private RectTransform rectTransform;
    private bool isDragging;
    private float initialDistance;
    private float initialScale;
    private float initialRotation;

   

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(InputAction.CallbackContext context) {
        Debug.Log("Drag");
        if (context.phase == InputActionPhase.Performed) {
            Vector2 touchPos = context.ReadValue<Vector2>();
            rectTransform.position = touchPos;
        }
    }

    public void OnPinch(InputAction.CallbackContext context) {
        Debug.Log("pinch");
        if (context.phase == InputActionPhase.Started) {
            initialDistance = Vector2.Distance(Touchscreen.current.touches[0].position.ReadValue(), Touchscreen.current.touches[1].position.ReadValue());
            initialScale = rectTransform.localScale.x;
        }

        else if (context.phase == InputActionPhase.Performed) {
            float currentDistace = Vector2.Distance(Touchscreen.current.touches[0].position.ReadValue(), Touchscreen.current.touches[1].position.ReadValue());
            float scaleFactor = currentDistace / initialDistance;
            rectTransform.localScale = new Vector3(initialScale * scaleFactor, initialScale * scaleFactor, 1);
        }
    }

    public void OnRotate(InputAction.CallbackContext context) {
        Debug.Log("rotate");
        if (context.phase == InputActionPhase.Started) {
            initialRotation = rectTransform.eulerAngles.z;
        }
        else if (context.phase == InputActionPhase.Performed) {
            float rotationInput = context.ReadValue<float>();
            rectTransform.eulerAngles = new Vector3(0, 0, initialRotation + rotationInput);
        }
    }
}
