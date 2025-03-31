using UnityEngine;
using UnityEngine.SceneManagement;

public class CollageInteractController : MonoBehaviour
{

    [SerializeField] private string photoEditingSceneName;
    public void OpenPhotoEditor() {
        SceneManager.LoadScene(photoEditingSceneName, LoadSceneMode.Additive);
    }
}
