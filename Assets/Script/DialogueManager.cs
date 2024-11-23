using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Customize your dialogue content and actors (Game Objects) who speak it.")]
    public string[] Dialogues;
    [Header("For more than 2 actors, in any sequence:")]
    [Header("Put in the dialogue in sequence. Drag in the actor (GO) in the matching sequence.")]
    public GameObject[] Actors;
    [Header("For 1 or 2 actors, speaking by turn.")]
    public GameObject Actor1;
    [Header("For 1 actor, leave Actor2 blank.")]
    public GameObject Actor2;

    private enum DialogueState
    {
        moreNPC, oneNPC, twoNPC
    }
    private DialogueState dialogueState;

    void Start()
    {
        //check error:
        if (Actor1 == null && Actor2 == null && Actors.Length >= 1)
        {
            dialogueState = DialogueState.moreNPC;
        }
        else if (Actor1 != null && Actor2 == null && Actors.Length == 0)
        {
            dialogueState = DialogueState.oneNPC;
        }
        else if (Actor1 != null && Actor2 != null && Actors.Length == 0)
        {
            dialogueState = DialogueState.twoNPC;
        }
        else
        {
            Debug.Log("Wrong actor setting, please check inspector")
        }
    }


    void Update()
    {
        for(int i = 0; i < Dialogues.Length;)
        {
            
        }


    }
}
