using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class CollageController : MonoBehaviour
{
    #region Initialisations
    [Header("Initialisations")]
    public RectTransform collageArea; // Attach UI canvas
    public GameObject imagePrefab; // Prefab with the image component
    private List<GameObject> images = new List<GameObject>();
    public GameObject player;
    public PlayerController playerController;
    public TextMeshProUGUI instructionsForSurrealGames;
    [SerializeField] private string[] instructionSets;

    [Header("Culling Mask")]
    public FindMainCamera canvasScript;

    [Header("Frame Settings")]
    public GameObject framePrefab; // Prefab for the photo frame gameobject
    public GameObject frameSpawner;
    [SerializeField] private Rect cameraCrop;

    #endregion

    void Start()
    {
        canvasScript = collageArea.GetComponent<FindMainCamera>();

        Cursor.lockState = CursorLockMode.None;  
        Cursor.visible = true;
        
        RandomSurrealistGame();
    }


    #region Functions
    public void PickImage() {
        Debug.Log("Called PickImage()");
        StartCoroutine(SelectImageFromGallery());
    }   

    public void SaveCollage() {
        Debug.Log("Called SaveCollage()");
        StartCoroutine(CaptureAndSave());
    }

    private IEnumerator SelectImageFromGallery() {
        if (NativeGallery.IsMediaPickerBusy()) {
            yield break;
        }

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) => {
            if (path != null) {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024);
                if (texture == null) return;

                GameObject newImage = Instantiate(imagePrefab, collageArea);
                newImage.GetComponent<RawImage>().texture = texture;
                newImage.AddComponent<DraggableImage>();
                images.Add(newImage);
            }
        }, "Select an image");

        yield return null;
    } 

    private IEnumerator CaptureAndSave() {
        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = rt;
        Camera.main.Render();
        RenderTexture.active = rt;

        Texture2D collageTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        collageTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        collageTexture.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        NativeGallery.SaveImageToGallery(collageTexture, "CollageApp", "Collage.png");
        Destroy(collageTexture);
    }

    private Texture2D CaptureCollage()
    {
        Debug.Log("Called CaptureCollage()");
        // Get the collage area dimensions
        Vector2 collageSize = collageArea.rect.size;
        int width = Mathf.RoundToInt(collageSize.x);
        int height = Mathf.RoundToInt(collageSize.y);

        // Determine square size (smallest of width/height)
        int squareSize = Mathf.Min(width, height);

        // Create a square RenderTexture
        RenderTexture rt = new RenderTexture(squareSize, squareSize, 24);
        RenderTexture.active = rt;

        // Render the UI to the texture
        Canvas canvas = collageArea.GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;

        if (uiCamera == null) 
        {
            Debug.LogError("No UI camera found! Make sure your Canvas is set to 'Screen Space - Camera'.");
            return null;
        }

        // Force UI to update before rendering
        Canvas.ForceUpdateCanvases();

        uiCamera.targetTexture = rt;
        uiCamera.Render();
        uiCamera.targetTexture = null;

        // Read pixels correctly from RenderTexture (start at 0,0 to avoid out-of-bounds error)
        Texture2D collageTexture = new Texture2D(squareSize, squareSize, TextureFormat.RGB24, false);
        collageTexture.ReadPixels(new Rect(0, 0, squareSize, squareSize), 0, 0);
        collageTexture.Apply();

        // Clean up
        RenderTexture.active = null;
        rt.Release();
        Destroy(rt);

        return collageTexture;
    }

    private Texture2D CaptureCroppedCollage(Rect captureRegion)
    {
        Debug.Log("Called CaptureCroppedCollage()");

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        int width = Mathf.RoundToInt(captureRegion.width);
        int height = Mathf.RoundToInt(captureRegion.height);
        int startX = Mathf.RoundToInt(captureRegion.x);
        int startY = Mathf.RoundToInt(captureRegion.y);

        // Ensure capture region is within screen bounds
        startX = Mathf.Clamp(startX, 0, screenWidth - width);
        startY = Mathf.Clamp(startY, 0, screenHeight - height);
        width = Mathf.Clamp(width, 1, screenWidth - startX);
        height = Mathf.Clamp(height, 1, screenHeight - startY);

        // Create a RenderTexture that captures the entire screen
        RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24);
        RenderTexture.active = rt;

        // Render the UI to the texture
        Canvas canvas = collageArea.GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.worldCamera;

        if (uiCamera == null) 
        {
            Debug.LogError("No UI camera found! Make sure your Canvas is set to 'Screen Space - Camera'.");
            return null;
        }

        // Force UI to update before rendering
        Canvas.ForceUpdateCanvases();

        uiCamera.targetTexture = rt;
        uiCamera.Render();
        uiCamera.targetTexture = null;

        // Read only the specified portion from the RenderTexture
        Texture2D collageTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        collageTexture.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        collageTexture.Apply();

        // Clean up
        RenderTexture.active = null;
        rt.Release();
        Destroy(rt);

        return collageTexture;
    }

    public void ExportAsPhotoFrame() {
        Debug.Log("Called ExportAsPhotoFrame()");

        // Texture2D collageTexture = CaptureCollage();
        Texture2D collageTexture = CaptureCroppedCollage(cameraCrop);
    
        // Convert Texture2D to a Sprite (for 2D use cases)
        Sprite newSprite = Sprite.Create(collageTexture, new Rect(0, 0, collageTexture.width, collageTexture.height), new Vector2(0.5f, 0.5f));

        // Find Frame Spawner
        frameSpawner = GameObject.FindWithTag("Frame Spawner");

        // Instantiate the frame prefab
        GameObject newFrame = Instantiate(framePrefab, frameSpawner.transform.position, Quaternion.identity);

        // Find the SpriteRenderer in the child object
        SpriteRenderer spriteRenderer = newFrame.GetComponentInChildren<SpriteRenderer>();

        Debug.Log("instantiated frame object " +newFrame.tag);

        // Assign texture to a SpriteRenderer (for 3D scene use)
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }

        //Disable Canvas
        gameObject.SetActive(false);

        // Find player object in additive scene
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        //Change to previous scenes defaults
        ChangeToDefalts();

        //Switch back to previous scene
        SceneManager.UnloadSceneAsync("Collage_CraftingSystem");
    }

    void ChangeToDefalts() {
        canvasScript.EnableCullingMasks();
        playerController.canMove = true;

        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center
        Cursor.visible = false; // Hides the cursor

        //canvasScript pickup
        playerController.canPickup = true;
    }

    public void RandomSurrealistGame()
    {
        if (instructionSets.Length == 0) return; // Prevent errors if the array is empty

        int randomIndex = Random.Range(0, instructionSets.Length); // Pick a random object
        instructionsForSurrealGames.text = instructionSets[randomIndex];
    }
    #endregion
}
