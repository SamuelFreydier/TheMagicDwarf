using System.Collections;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    public int healthGain = 5; //Gain de vie
    private bool entered = false; //Booléen servant à déclencher une mini coroutine pour attendre après un contact. Le joueur ayant deux collider, si les deux rentrent en contact avec le coeur, on ne veut pas qu'il regagne doublement de la vie
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !entered) //Si le joueur touche le coeur
        {
            entered = true;
            Destroy(gameObject.transform.root.gameObject); //Le coeur est détruit
            collision.gameObject.GetComponent<PlayerInventory>().AddLife(healthGain); //La fonction d'ajout de vie du joueur est appelée
            AudioManager.Instance.InstantPlay("LifeGain", 1f); //On joue le son de gain de vie
            StartCoroutine(WaitCollider()); //On déclenche la mini coroutine pour éviter le contact avec plusieurs collider
        }
    }
    IEnumerator WaitCollider()
    {
        yield return new WaitForSeconds(0.1f);
        entered = false;
    }
}
