using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Customize your dialogue content and actors (Game Objects) who speak it.")]

    [Header("1. Put in the dialogue in sequence.")]
    public string[] Dialogues;
    public int[] WaitTimes;
    public int SpawnTime;
    public int WaitTime;
    
    [Header("2. Drag in the actor (GO) in the matching sequence.")]
    [Header("For more than 2 actors, in any sequence:")]
    public GameObject[] Actors;
    [Header("For 1 or 2 actors, speaking by turn.")]
    public GameObject Actor1;
    [Header("For 1 actor, leave Actor2 blank.")]
    public GameObject Actor2;

    public TextMeshProUGUI TextMP;
    public GameObject TextObject;
    

    enum DialogueState
    {
        moreNPC, oneNPC, twoNPC
    }
    private DialogueState dialogueState;
    private int i;
    private float t;



    void Start()
    {
        i = -1;
        t = SpawnTime;
        TextMP.alpha = 0;

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
            Debug.Log("Wrong actor setting, please check inspector");
        }
    }


    void Update()
    {
        //print dialogues here
        t -= Time.deltaTime;
        if(t <= 0)
        {
            i++;
            if(i >= Dialogues.Length)
            {
                i = -1;
                t = SpawnTime;
                TextMP.alpha = 0;
            }
            if (i == 0) TextMP.alpha = 1;
            if (WaitTimes.Length >= 1) t = WaitTimes[i];
            else if (WaitTime != 0) t = WaitTime;
            else Debug.Log("WaitTime not set, check inspecter and set WaitTime or WaitTimes");
        }



        if(i >= 0) TextMP.text = Dialogues[i];

    }
}
