using System.Collections;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private int armorGain; //Gain d'armure
    private bool entered = false; //Même souci que pour HeartController

    private void Awake()
    {
        //Le gain d'armure varie selon la difficulté
        if (GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.EASY)
        {
            armorGain = Random.Range(2, 5);
        }
        if (GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.HARD)
        {
            armorGain = 3;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !entered) //Si le joueur va sur le bouclier
        {
            entered = true;
            Destroy(gameObject.transform.root.gameObject); //Bouclier détruit
            collision.gameObject.GetComponent<PlayerInventory>().AddArmor(armorGain); //Appel de la fonction de gain d'armure du joueur
            AudioManager.Instance.InstantPlay("ArmorGain", 1f); //On joue le son de gain d'armure
            StartCoroutine(WaitCollider());
        }
    }
    IEnumerator WaitCollider()
    {
        yield return new WaitForSeconds(0.1f);
        entered = false;
    }
}
