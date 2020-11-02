using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour //Boule de feu du joueur
{
    private GameObject player;
    private bool facingRight = true;
    public int fireballDamage = 30; //Dégâts de la boule de feu
    private bool entered = false;
    public float impulseX; //Impulsion de la boule de feu
    public float impulseY;
    private void Awake()
    {
        //Variation des dégâts en fonction de la difficulté
        if(GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.EASY)
        {
            fireballDamage = 50;
        }
        else
        {
            fireballDamage = 30;
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        AudioManager.Instance.InstantPlay("FireballThrow", 1f); //Son à l'instanciation de la boule de feu
        if (!player.GetComponent<PlayerController>().m_FacingRight) //La boule de feu part là où le joueur regarde
        {
            Vector3 theScale = gameObject.transform.localScale;
            theScale.x *= -1;
            gameObject.transform.localScale = theScale;
            facingRight = false;
        }
    }


    private void FixedUpdate()
    {
        //Déplacement de la boule de feu
        if (facingRight)
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
        
        if ((collision.tag == "Enemy"  || collision.tag == "Boss") && !entered) //Si la collision se fait avec un ennemi
        {
            entered = true;
            collision.gameObject.GetComponent<Enemy>().TakeDamage(fireballDamage); //L'ennemi prend les dégâts de la boule de feu
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
        if (collision.tag == "Ground") //Si elle rencontre un décor
        {
            DestroyFireball(); //Elle est détruite
        }
    }

    private void DestroyFireball() //Destruction de la boule de feu
    {
        AudioManager.Instance.InstantPlay("FireballHit", 1f); //Son de la destruction de la boule de feu
        Destroy(gameObject, 0.01f);
    }
    IEnumerator WaitCollider()
    {
        yield return new WaitForSeconds(0.1f);
        entered = false;
    }

}
