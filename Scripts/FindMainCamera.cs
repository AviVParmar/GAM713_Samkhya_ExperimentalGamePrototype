using UnityEngine;

public class FindMainCamera : MonoBehaviour
{

    Canvas canvas;

    [Header("Culling Masks")]
    [SerializeField] private LayerMask UiCullingMask;
    [SerializeField] private LayerMask regularCullingMask;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();
        FindSceneCamera();
        IncreaseCanvasSortingOrder();
        DisableCullingMasks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IncreaseCanvasSortingOrder() {
        canvas.sortingOrder = 10;
    }

    void FindSceneCamera() {
        canvas.worldCamera = Camera.main;
    }

    void DisableCullingMasks() {
        Camera.main.cullingMask = UiCullingMask;
    }

    public void EnableCullingMasks() {
        Camera.main.cullingMask = regularCullingMask;
    }

}
