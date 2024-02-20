using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    //public Animator dialoguestartAnimator;
    public Animator dialogueAnimator;
    public DialogueManager manager;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        dialogueAnimator.SetBool("dialogueOpen",true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        dialogueAnimator.SetBool("dialogueOpen", false);
        manager.EndDialogue();
    }
}
