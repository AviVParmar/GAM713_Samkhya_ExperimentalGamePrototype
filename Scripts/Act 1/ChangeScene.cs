using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private string instrctionScene;
    [SerializeField] private string URL;

    public void SceneChange() {
        SceneManager.LoadScene(nextScene);
    }

    public void InstructionsScene() {
        SceneManager.LoadScene(instrctionScene);
    }

    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenLink() {
        Application.OpenURL(URL);
    }
}
