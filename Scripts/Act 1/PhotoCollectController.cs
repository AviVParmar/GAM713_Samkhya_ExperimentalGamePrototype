using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PhotoCollectController : MonoBehaviour
{


    [Header("Variables")]
    [SerializeField] private RawImage displayImage;
    [SerializeField] private string filePath;

    public void PickImage(){
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) => {

            if (path != null) {
                Texture2D texture = LoadTextureFromFilePath(path);

                displayImage.texture = texture;

                SavePhoto(texture, "savedImage.png");
            }

        }, "Select an image");
    }

    private Texture2D LoadTextureFromFilePath(string path) {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return texture;
    }

    private void SavePhoto(Texture2D texture, string fileName) {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log("Image saved at: " + path);
    }
    
}
