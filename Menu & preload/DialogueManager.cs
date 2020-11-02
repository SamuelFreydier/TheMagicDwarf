using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager> //Manager gérant les dialogues
{
    public GameObject dialogueBox; //Boîte de dialogue

    public Text dialogueName; //Nom de l'interlocuteur
    public Text dialogueText; //Ligne de texte
    public Image dialoguePortrait; //Portrait
    public string dialogueSound = ""; //Doublage de la ligne

    public Queue<DialogueBase.Info> dialogueInfo; //Queue permettant "d'amener" un dialogue au DialogueManager

    private bool isTyping; //Est-ce que le texte est en train d'être tapé ?
    private string completeText;
    private void Start()
    {
        dialogueInfo = new Queue<DialogueBase.Info>();
    }
    public void EnqueueDialogue(DialogueBase db) //Fonction permettant d'ajouter un dialogue entier à la queue dans l'ordre des répliques
    {
        Debug.Log("Enqueuing");
        dialogueBox.SetActive(true);
        GameManager.Instance.SwitchToDialogue();
        dialogueInfo.Clear();

        foreach(DialogueBase.Info info in db.dialogueInfo)
        {
            dialogueInfo.Enqueue(info);
        }

        DequeueDialogue();
    }

    public void DequeueDialogue() //Affiche la première réplique disponible dans la queue, avec ses caractéristiques, la fonction est enclenchée au début du dialogue et quand on appuie sur le bouton next
    {

        if (isTyping == true) //Si le texte est en train d'être tapé
        {
            CompleteText(); //Le bouton complète le texte au lieu de passer au suivant
            StopAllCoroutines();
            isTyping = false; //Le texte n'est plus en train d'être tapé
            return;
        }

        if (dialogueInfo.Count == 0) //Si la queue est vide
        {
            AudioManager.Instance.InstantStop(dialogueSound);
            EndofDialogue(); //Le dialogue est terminé, on le quitte
            return;
        }

        if (dialogueSound != "")
        {
            AudioManager.Instance.InstantStop(dialogueSound); //On stoppe le doublage du texte lorsqu'on passe au suivant
        }
        DialogueBase.Info info = dialogueInfo.Dequeue();
        //On prend toutes les caractéristiques de la réplique en cours
        dialogueSound = info.mySound;
        AudioManager.Instance.InstantPlay(dialogueSound, 1);
        completeText = info.myText;

        dialogueName.text = info.myName;
        dialogueText.text = info.myText;
        dialoguePortrait.sprite = info.myPortrait;

        dialogueText.text = "";
        StartCoroutine(TypeText(info));
    }

    private void CompleteText() //Permet de compléter un texte en cours d'écriture pour aller plus vite
    {
        dialogueText.text = completeText;
    }
    IEnumerator TypeText(DialogueBase.Info info) //Permet d'afficher le texte petit à petit
    {
        isTyping = true;
        dialogueText.text = "";
        foreach(char c in info.myText.ToCharArray())
        {
            yield return null;
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    public void EndofDialogue() //Fin du dialogue, on repasse en RUNNING et on désactive la boîte de dialogue
    {
        dialogueBox.SetActive(false);
        GameManager.Instance.SwitchToDialogue();
    }
}
