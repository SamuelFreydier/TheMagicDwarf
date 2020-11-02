using System.Collections;
using UnityEngine;

public class DamageEnemy : MonoBehaviour //Comme DamageObstacle mais ici pour des ennemis
{
    public int damage; //Dégâts infligés par l'ennemi
    public float impulseX; //Impulsion à la suite des dégâts
    public float impulseY;
    private bool entered = false; //Voir HeartController
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !entered) //Collision avec le joueur
        {
            entered = true;
            collision.gameObject.GetComponent<PlayerInventory>().LessLife(damage); //Le joueur perd des PV
            float buffx = transform.position.x - collision.transform.position.x; //Il subit une impulsion
            if(buffx < 0)
            {
                buffx = -impulseX;
            }
            else
            {
                buffx = impulseX;
            }
            
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-buffx, impulseY), ForceMode2D.Impulse);
            StartCoroutine(WaitCollider(0.3f));
            
        }

    }

    private void OnTriggerStay2D(Collider2D collision) //Pareil qu'au-dessus mais c'est si le joueur reste à côté de l'ennemi (par exemple s'il est acculé contre un mur)
    {
        if (collision.tag == "Player" && !entered)
        {
            entered = true;
            collision.gameObject.GetComponent<PlayerInventory>().LessLife(damage);
            print("Vie restante : " + collision.gameObject.GetComponent<PlayerInventory>().Life);
            float buffx = transform.position.x - collision.transform.position.x;
            if (buffx < 0)
            {
                buffx = -impulseX;
            }
            else
            {
                buffx = impulseX;
            }

            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-buffx, impulseY), ForceMode2D.Impulse);
            StartCoroutine(WaitCollider(0.3f));

        }
    }

    IEnumerator WaitCollider(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        entered = false;
    }
}

