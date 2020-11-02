using UnityEngine;

public class TriggerDialogue : MonoBehaviour //Déclenchement d'un dialogue (via un NPC par exemple)
{
    public DialogueBase dialogue; //Dialogue à déclencher
    public GameObject pressFIcon; //Icône visuel permettant de montrer au joueur qu'il peut appuyer sur F pour déclencher le dialogue

    public void TriggerDialogues() //On met le dialogue dans la queue du DialogueManager, le dialogue est ainsi enclenché
    {
        DialogueManager.Instance.EnqueueDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            pressFIcon.SetActive(true); //Si le joueur est à portée pour déclencher le dialogue, un indicateur le lui montre
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetKeyDown(KeyCode.F) && GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            TriggerDialogues(); //Si le joueur presse F, le dialogue est enclenché
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            pressFIcon.SetActive(false); //Si le joueur sort de la portée, l'indicateur visuel disparaît
        }
    }
}
