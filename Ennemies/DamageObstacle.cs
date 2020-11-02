using System.Collections;
using UnityEngine;

public class DamageObstacle : MonoBehaviour //Fonction de dégâts quand un obstacle et autre chose rentrent en collision
{
    public int damage; //Dégâts infligés par l'obstacle
    public float impulseX; //Impulsion en x
    public float impulseY; //et en y
    private bool entered = false; //Même problème que pour HeartController
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !entered) //Si la collision se fait avec le joueur
        {
            entered = true;
            collision.gameObject.GetComponent<PlayerInventory>().LessLife(damage); //Il perd des PV
            float buffx = transform.position.x - collision.transform.position.x; //Il subit une impulsion
            float buffy = transform.position.y - collision.transform.position.y;
            if(buffx < 0)
            {
                buffx = -impulseX;
            }
            else
            {
                buffx = impulseX;
            }
            if(buffy < 0)
            {
                buffy = impulseY;
            }
            else
            {
                buffy = -impulseY;
            }
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-buffx, buffy), ForceMode2D.Impulse);
            StartCoroutine(WaitCollider());
            
        }
        if(collision.tag == "Enemy") //Si la collision se fait avec un ennemi
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(500); //L'ennemi prend tarif
        }
    }

    IEnumerator WaitCollider()
    {
        yield return new WaitForSeconds(0.1f);
        entered = false;
    }
}

