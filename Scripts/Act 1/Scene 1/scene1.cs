using UnityEngine;
using UnityEngine.SceneManagement;

public class scene1 : MonoBehaviour
{
    [SerializeField] private string nextScene;

    public GameObject dialogueOnTapObject;
    public DialogueOnTap dialogueOnTap;

    void Update()
    {
        if (dialogueOnTap.dialogueIsOver)
        Invoke("SceneChange", 3.5f);
    }

    private void SceneChange() {
        Debug.Log("Changing Scene to "+nextScene);
        SceneManager.LoadScene(nextScene);
    }
}
