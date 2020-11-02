using System.Collections;
using UnityEngine;

public class EvilFireball : MonoBehaviour //Boule de feu déclenchée par le boss champignon
{
    private GameObject boss;
    private bool facingRight = true;
    public int fireballDamage = 30; //Dégâts de la boule de feu
    private bool entered = false;
    public float impulseX; //Impulsion de la boule de feu à l'explosion
    public float impulseY;
    private void Awake()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
    }

    private void Start()
    {
        AudioManager.Instance.InstantPlay("FireballThrow", 1f); //Son déclenché par la boule de feu à l'instanciation
        if (!boss.GetComponent<BossFollow>().facingRight) //La boule de feu part dans la direction dans laquelle l'ennemi regarde
        {
            Vector3 theScale = gameObject.transform.localScale;
            theScale.x *= -1;
            gameObject.transform.localScale = theScale;
            facingRight = false;
        }
    }


    private void FixedUpdate()
    {
        if (facingRight) //Déplacement de la boule de feu
        {
            gameObject.transform.position += new Vector3(0.4f, 0, 0);
        }
        else
        {
            gameObject.transform.position -= new Vector3(0.4f, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player" && !entered) //Si la collision se fait avec le joueur
        {
            entered = true;
            collision.gameObject.GetComponent<PlayerInventory>().LessLife(fireballDamage); //Il prend des dégâts
            float buffx = transform.position.x - collision.transform.position.x; //Il subit une impulsion
            float buffy = transform.position.y - collision.transform.position.y;
            if (buffx < 0)
            {
                buffx = -impulseX;
            }
            else
            {
                buffx = impulseX;
            }
            if (buffy < 0)
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
            DestroyFireball(); //La boule de feu est détruite
        }
        if (collision.tag == "Ground") //Si elle rentre en contact avec un décor
        {
            DestroyFireball(); //Elle est détruite
        }
    }

    private void DestroyFireball() //Destruction de la boule de feu
    {
        AudioManager.Instance.InstantPlay("FireballHit", 1f); //Son de l'explosion
        Destroy(gameObject, 0.01f);
    }
    IEnumerator WaitCollider()
    {
        yield return new WaitForSeconds(0.1f);
        entered = false;
    }

}
