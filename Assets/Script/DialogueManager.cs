using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Customize your dialogue content and actors (Game Objects) who speak it.")]
    public string[] Dialogues;
    public int[] WaitTimes;
    public int SpawnTime;
    public int WaitTime;
    public GameObject worldDialogueCanvas;

    [Header("Drag in the actor (GO) in the matching sequence.")]
    public GameObject[] Actors; // List of NPCs that will speak

    public GameObject textPrefab; // Prefab for the TextMeshPro dialogue

    private TextMeshProUGUI[] npcTextObjects; // TextMeshPro components for NPCs
    private int i; // Current dialogue index
    private float t; // Timer

    void Start()
    {
        i = -1;
        t = SpawnTime;

        if (Actors.Length == 0 || Dialogues.Length == 0)
        {
            Debug.LogError("Actors or Dialogues not assigned! Check the inspector.");
            return;
        }

        // Initialize TextMeshPro objects above each actor
        InitializeNPCTextObjects();
    }

    void Update()
    {
        t -= Time.deltaTime;

        if (t <= 0)
        {
            i++;

            if (i >= Dialogues.Length)
            {
                ResetDialogue();
                return;
            }

            if (WaitTimes.Length >= 1) t = WaitTimes[i];
            else if (WaitTime != 0) t = WaitTime;
            else Debug.LogError("WaitTime not set, check inspector and set WaitTime or WaitTimes");

            ShowDialogue(i);
        }
    }

    void InitializeNPCTextObjects()
    {
        if (worldDialogueCanvas == null)
        {
            Debug.LogError("WorldDialogueCanvas not assigned! Please assign it in the Inspector.");
            return;
        }

        npcTextObjects = new TextMeshProUGUI[Actors.Length];

        for (int j = 0; j < Actors.Length; j++)
        {
            // Instantiate the text prefab and parent it to the WorldDialogueCanvas
            GameObject textObject = Instantiate(textPrefab, worldDialogueCanvas.transform);
            npcTextObjects[j] = textObject.GetComponent<TextMeshProUGUI>();

            // Set up the text correctly
            textObject.transform.localScale = Vector3.one; // Ensure scale is correct
            textObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero; // Reset anchor position
            textObject.transform.position = Actors[j].transform.position + new Vector3(0, 2.5f, 0); // Position above the NPC's head
            textObject.transform.LookAt(Camera.main.transform); // Make sure it faces the camera
            textObject.transform.Rotate(0, 180f, 0); // Adjust rotation to face the correct way

            // Hide text initially
            npcTextObjects[j].text = "";
            npcTextObjects[j].gameObject.SetActive(false);
        }
    }


    void ShowDialogue(int index)
    {
        // Determine which NPC is speaking
        int actorIndex = Mathf.Min(index, Actors.Length - 1);

        // Show the dialogue above the current NPC
        npcTextObjects[actorIndex].text = Dialogues[index];
        npcTextObjects[actorIndex].gameObject.SetActive(true);

        // Hide the dialogue after the wait time
        StartCoroutine(HideDialogueAfterDelay(npcTextObjects[actorIndex], t));
    }

    IEnumerator HideDialogueAfterDelay(TextMeshProUGUI textObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (textObject != null)
        {
            textObject.text = "";
            textObject.gameObject.SetActive(false);
        }
    }

    void ResetDialogue()
    {
        i = -1;
        t = SpawnTime;

        foreach (var textObject in npcTextObjects)
        {
            textObject.text = "";
            textObject.gameObject.SetActive(false);
        }
    }


}

