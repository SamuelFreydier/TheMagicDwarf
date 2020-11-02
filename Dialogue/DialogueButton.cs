using UnityEngine;

public class DialogueButton : MonoBehaviour //Script du bouton qui permet d'avancer dans le dialogue
{
    public void GetNextLine()
    {
        DialogueManager.Instance.DequeueDialogue(); //On fait sortir la prochaine ligne de la queue pour que la valeur actuelle du DialogueManager prenne les valeurs de la nouvelle ligne
    }
}
