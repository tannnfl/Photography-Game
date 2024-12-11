using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartInstruction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText; // Reference to the TextMeshProUGUI object
    [SerializeField] private float fadeDuration = 1f; // Duration for fade in/out
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private CameraCapture cameraCapture;

    private enum State {intro, move, camera, camera0, camera2, openblog, aboard}
    private State currentState = State.intro;
    private string[] instructions = { };
    private int instructionPos;
    private Coroutine typingCoroutine;

    private bool cameraOpened;
    public Image fadeImage; // Assign your FadeImage in the inspector
    public float fadeImageDuration = 1.0f; // Duration of the fade effect

    private void Start()
    {
        SwitchState(State.intro);
        instructionPos = 0;
        UpdateInstruction();
    }
    private void Update()
    {
        print(currentState);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) UpdateInstruction();
        switch (currentState)
        {
            case State.intro:
                if (instructionPos == instructions.Length - 1) SwitchState(State.move);
                break;
            case State.move:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                    StartCoroutine(moveCompleted());
                break;
            case State.camera:
                if (Input.GetKeyDown(KeyCode.C)) cameraOpened = true;
                if (cameraOpened) SwitchState(State.camera0); 
                break;
            case State.camera0:
                if (cameraCapture.capturedPhotos.Count != 0 && cameraCapture.currentCMode != CameraCapture.CMode.ViewPhoto) SwitchState(State.camera2);
                break;
            case State.camera2:
                if (instructionPos == instructions.Length - 1) SwitchState(State.openblog);
                break;
            case State.openblog:
                if (Input.GetKeyDown(KeyCode.B)) SwitchState(State.aboard);
                break;
            case State.aboard:
                if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene("Week3", LoadSceneMode.Single); 
                break;
        }
    }

    private void SwitchState(State newState)
    {
        switch (newState)
        {
            case State.intro:
                instructions = new string[]
                { 
                    "\"Finally, a break after 300 days of endless work\"",
                    "\"I finally got the money to quit, and the one-day trip to...\"",
                    "\"WonderIsland.\"",
                    "\"I'm so excited.\"",
                    "\"I won't forget this precious memory, forever.\"",
                    ""
                };
                break;
            case State.move:
                instructions = new string[]
                {
                    "Move camera with wsad."
                };
                break;
            case State.camera:
                instructions = new string[]
                {
                    "Grab your camera [C], zoom to adjust [Scroll], and take a picture [Click]."
                };
                break;
            case State.camera0:
                instructions = new string[]
                {
                    ""
                };
                break;
            case State.camera2:
                instructions = new string[]
                {
                    "You have a limited film. You can only take 12 shots.",
                    ""
                };
                break;
            case State.openblog:
                instructions = new string[]
                {
                    "\"Nice pic. Posted.\"",
                    "Photos you shot will be posted on your blog.",
                    "More attractive the objects are, more views of the post.",
                    "You can always check your blog. [B]"
                };
                break;
            case State.aboard:
                instructions = new string[]
                {
                    "Aboard and enjoy your trip [Space]."
                };
                break;
        }

        instructionPos = 0;
        currentState = newState;
        UpdateInstruction();
    }

    // Method to update instruction
    public void UpdateInstruction()
    {
        if (instructionPos == instructions.Length - 1) instructionPos = 0;
        else instructionPos++;

        // Stop any currently running coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start a new typing coroutine
        typingCoroutine = StartCoroutine(TypingEffectCoroutine(instructions[instructionPos]));
    }


    private IEnumerator UpdateInstructionCoroutine(string newInstruction)
    {
        // Fade out the current text
        yield return StartCoroutine(FadeTextAlpha(0f));

        // Change the text to the new instruction
        instructionText.text = newInstruction;

        // Fade in the new text
        yield return StartCoroutine(FadeTextAlpha(1f));
    }

    private IEnumerator FadeTextAlpha(float targetAlpha)
    {
        // Get the current alpha value
        Color currentColor = instructionText.color;
        float startAlpha = currentColor.a;

        // Fade over time
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            instructionText.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        // Ensure final alpha is set correctly
        instructionText.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }

    private IEnumerator TypingEffectCoroutine(string newInstruction)
    {
        // Clear the current text
        instructionText.text = "";

        // Type out the new instruction one character at a time
        foreach (char character in newInstruction)
        {
            instructionText.text += character;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Nullify the coroutine reference when finished
        typingCoroutine = null;
    }

    private IEnumerator moveCompleted()
    {
        yield return new WaitForSeconds(3f);
        SwitchState(State.camera);
    }

    

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeToBlack(sceneName));
    }

    private IEnumerator FadeToBlack(string sceneName)
    {
        // Gradually increase the alpha of the image to fade to black
        float elapsed = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeColor.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }

        // Ensure it's completely black
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Optionally fade from black in the new scene
    }
}
