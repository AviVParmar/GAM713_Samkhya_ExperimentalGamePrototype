using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.SceneManagement;


public class DrawOnTexture : MonoBehaviour
{
    #region Initialising variables
    [Header("Mouse or Touch")]
    [SerializeField] bool hasMouse;
    [SerializeField] bool hasTouch;

    [Header("Save Settings")]
    [SerializeField] private string fileName;

    [Header("Draw settings")]
    [SerializeField] private Color drawColor = Color.black;
    [SerializeField][Range(0f, 2f)] private float brushSpeed = 0.1f;
    [SerializeField] private float minBrushSize = 2f; // Smallest Brsuh Size
    [SerializeField] private float maxBrushSize = 5f; // Largest Brush Size
    [SerializeField][Range(1f, 25f)] private float brushSize = 5f; // Current Brush Size
    private Vector2 lastTouchPos;
    private bool isFirstTouch = true;
    private int textureSize = 512;
    
    [Header("links")]
    [SerializeField] private RawImage drawingCanvas;
    private Texture2D texture;

    [Header("SceneManager")]
    [SerializeField] private string nextScene;

    [Header("Button References")]
    public RectTransform resetButton;
    public RectTransform saveButton;
    #endregion
    
    #region Start, update
    private void Start()
    {
        // Create a blank white texture
        texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        ClearTexture();
        drawingCanvas.texture = texture;
    }

    private void Update()
    {
        if (hasTouch)
        HandleTouchInput();

        if (hasMouse)
        HandleMouseInput();
    }
    #endregion

    #region Input Handling

    private void HandleTouchInput() {
        // Get the current touchscreen
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch == null) return;

        var touch = Touchscreen.current.primaryTouch;
        
        // Check if there are any active touches
        if (touch.press.isPressed) {

                Vector2 touchPos = touch.position.ReadValue();
                Vector2 pixelPos = GetTouchPosition(touchPos);

                OnResetButton(touchPos);
                OnSaveButton(touchPos);

                ProcessDrawing(pixelPos);
        }

        else {
            isFirstTouch = true;
        }
    }

    private void HandleMouseInput() {
        if (Mouse.current == null) return;

        var mouse = Mouse.current;

        if (Mouse.current.leftButton.isPressed) {
            Vector2 mousePos = mouse.position.ReadValue();
            Vector2 pixelPos = GetTouchPosition(mousePos);

            OnResetButton(mousePos);
            OnSaveButton(mousePos);

            ProcessDrawing(pixelPos);
        }

        else {
            isFirstTouch = true;
        }
    }

    #endregion

    #region Drawing

    private void ProcessDrawing(Vector2 pixelPos) {
        if (isFirstTouch) {
            lastTouchPos = pixelPos;
            isFirstTouch = false;
            return; // Ensure we dont calculate speed on first touch
        }

        //Calculate drawing speed, and distance between previous and current position
        float speed = Vector2.Distance(pixelPos, lastTouchPos);
        brushSize = Mathf.Lerp(maxBrushSize, minBrushSize, speed/50f); // Adjust size based on speed

        DrawLine(lastTouchPos, pixelPos, brushSize);
        lastTouchPos = pixelPos;
    }

    void OnResetButton(Vector2 touchPosition)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(resetButton, touchPosition, null)) {
                Debug.Log("clicked on reset");

                //Resets the scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
    }

    void OnSaveButton(Vector2 touchPosition) {
        if (RectTransformUtility.RectangleContainsScreenPoint(saveButton, touchPosition, null)) {
                Debug.Log("clicked on save");

                SaveImage();
                SceneManager.LoadScene(nextScene);
            }
    }

    Vector2 GetTouchPosition(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            drawingCanvas.rectTransform, screenPos, null, out Vector2 localPoint);

        float x = (localPoint.x + drawingCanvas.rectTransform.rect.width / 2) / drawingCanvas.rectTransform.rect.width * textureSize;
        float y = (localPoint.y + drawingCanvas.rectTransform.rect.height / 2) / drawingCanvas.rectTransform.rect.height * textureSize;

        return new Vector2(x, y);
    }


    // Line Drawing Function (Bresenham’s Algorithm)
    void DrawLine(Vector2 start, Vector2 end, float size)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(start, end)); // Number of steps
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps; // Interpolation factor (0 to 1)
            Vector2 point = Vector2.Lerp(start, end, t); // Get interpolated point
            DrawPoint(point, size);
    }
}

    // Draw a single point with a given brush size
    void DrawPoint(Vector2 pos, float size)
    {
       int brushRadius = Mathf.RoundToInt(size / 2);

        for (int x = -brushRadius; x <= brushRadius; x++)
        {
            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                int pixelX = (int)pos.x + x;
                int pixelY = (int)pos.y + y;

                if (pixelX >= 0 && pixelX < textureSize && pixelY >= 0 && pixelY < textureSize)
                {
                    Color existingColor = texture.GetPixel(pixelX, pixelY);
                    Color blendedColor = Color.Lerp(existingColor, drawColor, drawColor.a);

                    texture.SetPixel(pixelX, pixelY, blendedColor);
                }
            }
        }
        texture.Apply();
    }

    public void ClearTexture()
    {
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color(1, 1, 1, 0);

        texture.SetPixels(pixels);
        texture.Apply();
    }    

    public void SaveImage()
    {
        // Chek if there are transparent pixels
        Color pixel = texture.GetPixel(0, 0); // Check a sample pixel
        Debug.Log("Alpha value: " + pixel.a); // Should be less than 1 for transparency

        // Convert the texture to PNG format
        byte[] bytes = texture.EncodeToPNG();
        
        // Define the file path
        fileName = fileName + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        // Save the file
        File.WriteAllBytes(filePath, bytes);
        
        // Confirm save
        Debug.Log("Image saved to: " + filePath);
    }
    #endregion
}
