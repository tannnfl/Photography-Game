using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Customize your dialogue content and actors (Game Objects) who speak it.")]
    public string[] Dialogues;
    public int[] WaitTimes;
    public int SpawnTime;
    public int WaitTime;

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
        npcTextObjects = new TextMeshProUGUI[Actors.Length];

        for (int j = 0; j < Actors.Length; j++)
        {
            // Find or create a dialogue anchor for each NPC
            Transform anchorTransform = Actors[j].transform.Find("DialogueAnchor");
            if (anchorTransform == null)
            {
                // Create an anchor if it doesn't exist
                GameObject anchor = new GameObject("DialogueAnchor");
                anchorTransform = anchor.transform;
                anchorTransform.SetParent(Actors[j].transform);
                anchorTransform.localPosition = new Vector3(0, 2.5f, 0); // Position above the NPC's head
            }

            // Instantiate the text prefab and parent it to the anchor point
            GameObject textObject = Instantiate(textPrefab, anchorTransform);
            npcTextObjects[j] = textObject.GetComponent<TextMeshProUGUI>();

            // Set local position of text relative to the anchor (usually centered)
            textObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

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

