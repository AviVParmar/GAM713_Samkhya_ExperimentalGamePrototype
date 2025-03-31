using UnityEngine;
using System.IO;
using Unity.Mathematics;

public class TreeSpawner : MonoBehaviour
{

    [SerializeField] Vector3 spawnPointOffset;
    [SerializeField] float quadSize;
    [SerializeField] int numberOfPlanes;

    private void Start() {
        string fullPath = Path.Combine(Application.persistentDataPath, "Tree.png");

        float rotationDelta = 360/numberOfPlanes;
        float currentRotation = 0f;

        for (int i = 0; i < numberOfPlanes; i++) {
            if (File.Exists(fullPath)) {
                Debug.Log("Found file " + fullPath);
                Texture2D texture = LoadPNG(fullPath);

                if (texture != null ){
                    
                    

                    GameObject sprite3D = create3DSprite(texture);
                    sprite3D.transform.position = gameObject.transform.position + spawnPointOffset;
                    sprite3D.transform.rotation = Quaternion.Euler(0, currentRotation + rotationDelta, 0);

                    currentRotation += rotationDelta;
                }
            }

            else {
                Debug.LogError("Image not found at: " + fullPath);
            }
        }
        
    }


    Texture2D LoadPNG(string filePath) {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (texture.LoadImage(fileData)){
            texture.Apply();
            return texture;
        }
        return null;
    }

    GameObject create3DSprite(Texture2D texture) {
        GameObject treeSprite = GameObject.CreatePrimitive(PrimitiveType.Quad);
        treeSprite.transform.localScale = new Vector3(quadSize, quadSize, 1);

        // Load the URP shader
        Shader urpShader = Shader.Find("Universal Render Pipeline/Unlit");

        MeshCollider meshCollider = treeSprite.GetComponent<MeshCollider>();
        if (meshCollider != null) {
            Destroy(meshCollider);
        }

        if (urpShader == null) {
            urpShader = Shader.Find("Hidden/Universal Render Pipeline/Unlit");
        }

        if (urpShader == null) {
            Debug.LogWarning("URP Shader not found! Using default Unlit shader.");
            urpShader = Shader.Find("Unlit/Transparent"); // Use a fallback
        }

        if (urpShader == null) {
            Debug.LogError("URP Shader not found! Ensure URP is installed and active.");
            return null;
        }

        Material material = new Material(urpShader)
        {
            mainTexture = texture
        };

        // Enable transparency
        material.SetFloat("_Surface", 1); // Transparent surface
        material.SetFloat("_AlphaClip", 0); // Disable alpha clipping
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0); // Disable depth writing for transparency
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        treeSprite.GetComponent<Renderer>().material = material;
        return treeSprite;
    }
}
