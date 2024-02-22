using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    public DialogueTrigger dialogue;
    public Animator dialogueStartAnimator;
    public DialogueManager manager;
    public bool playerIsClose;
    public bool dialogueStarted;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        dialogueStartAnimator.SetBool("startOpen",true);
        playerIsClose = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        dialogueStartAnimator.SetBool("startOpen", false);
        manager.EndDialogue();
        playerIsClose = false;
        dialogueStarted = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose && !dialogueStarted)
        {
            dialogueStarted = true;
            dialogue.TriggerDialogue();
        }
        if (Input.GetKeyDown(KeyCode.E) && dialogueStarted)
        {
            manager.DisplayNextSentence();
        }
    }

}
