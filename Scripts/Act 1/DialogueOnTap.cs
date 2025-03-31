using UnityEngine;
using UnityEngine.InputSystem;

using Ink.Runtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueOnTap : MonoBehaviour
{
    //INPUT SYSTEM
    [Header("Input System")]
    InputSystem_Actions inputSystem;
    InputAction tap;
    InputAction jump;

    //LINKS
    [Header("References")]
    [Header("INK JSON file")]
    [SerializeField] private TextAsset inkJson;

    [Header("UI Stuff")]
    [SerializeField] private Transform UiCanvas;  // Parent to place buttons in UI
    [SerializeField] private GameObject panelPrefab;        //UI prefab to spawn
    [SerializeField] private TextMeshProUGUI storyText;       // UI Text to display the story
    private Story currentStory;
    public bool dialogueIsOver = false;

    [Header("Scene Change Settings")]
    private bool storyIsOver = false;
    [SerializeField] private string nextScene;

    [Header("Raw image links")]
    [SerializeField] GameObject maya;
    [SerializeField] GameObject brahman;
    
    

    void Awake()
    {
        storyText.text = "<press space>";

        inputSystem = new InputSystem_Actions();

        storyIsOver = false;

        //hide logos
        maya.SetActive(false);
        brahman.SetActive(false);
    }

    void OnEnable()
    {
        tap = inputSystem.Player.TouchTap;
        tap.Enable();
        tap.performed += OnTap;

        jump = inputSystem.Player.Jump;
        jump.Enable();
        jump.performed += OnJump;
    }

    void OnDisable()
    {
        tap.performed -= OnTap;
        tap.Disable();

        jump.performed -= OnJump;
        jump.Disable();
    }

    void Start()
    {
        //Load the story from the JSON file
        currentStory = new Story(inkJson.text);
    }



    #region input
    void OnTap(InputAction.CallbackContext context) {
        Debug.Log("screen tapped");
        ContinueStory();

        if (storyIsOver) SceneChange();
    }

    void OnJump(InputAction.CallbackContext context) {
        Debug.Log("space pressed");
        ContinueStory();

        if (storyIsOver) SceneChange();
    }
    #endregion


    #region ink scripts
    void ContinueStory() {
        //Display the next portion of the story
        if (currentStory.canContinue) {
            storyText.text = currentStory.Continue();
            List<string> currentStory_Tags = currentStory.currentTags;

            maya.SetActive(false);
            brahman.SetActive(false);

            //Check for tags
            foreach (string tag in currentStory_Tags)
            {
                if (tag == "brahman")
                {
                    Debug.Log("found brahman tag");

                    maya.SetActive(false);
                    brahman.SetActive(true);
                }

                if (tag == "maya")
                {
                    Debug.Log("found maya tag");

                    brahman.SetActive(false);
                    maya.SetActive(true);
                }
            }
        }

        else {
            gameObject.SetActive(false);
            dialogueIsOver = true;
            Debug.Log("Sory is over");
            storyIsOver = true;
        }
    }

    void SceneChange() {
            SceneManager.LoadScene(nextScene);
    }

    #endregion

}
