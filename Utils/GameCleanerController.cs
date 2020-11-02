using UnityEngine;

public class GameCleanerController : MonoBehaviour //Classe gérant la chute d'éléments dans le vide
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player") //Si l'élément est le joueur...
        {
            GameManager.Instance.TriggerLose(); //... Il perd la partie (déclenché à l'aide d'un changement de State dans le GameManager)
        }
        if(collision.tag == "Enemy") //Si c'est un ennemi...
        {
            Destroy(collision.gameObject); //... Il est détruit sans aucune vergogne
        }
    }
}
