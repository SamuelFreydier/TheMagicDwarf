using System.Collections;
using UnityEngine;

public class BossFollow : MonoBehaviour //Classe gérant le "pathfinding" d'un ennemi qui doit suivre le joueur, ici le boss car il peut jeter des boules de feu
{
    private Enemy enemy;
    private Animator animator;
    private Rigidbody2D rb;
    private GameObject target; //Cible des ennemis (ce sera le joueur)
    public Transform attackPoint;
    public float attackRange = 0.1f;
    public LayerMask playerLayer;

    public GameObject fireball;

    public float speed; //Vitesse de l'ennemi
    public float stoppingDistance; //Distance par rapport au joueur à laquelle l'ennemi arrêtera d'avancer
    public float visionDistance; //Champ de vision de l'ennemi
    public float hurtStopMovementTime; //Durée pendant laquelle l'ennemi arrête de bouger après avoir reçu un coup
    public float attackStopMovementTime; //Durée pendant laquelle l'ennemi arrête de bouger après avoir lancé une attaque.
    private bool canMove = true; //L'ennemi peut-il bouger ?
    public bool facingRight = false; //Où regarde l'ennemi ?
    Coroutine stopmoving;
    Coroutine attacking;

    private void Awake()
    {
        //Ajustement de la vitesse de l'ennemi selon la difficulté
        if(GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.HARD)
        {
            speed *= 1.2f;
        }
        
        target = GameObject.FindGameObjectWithTag("Player"); //La cible est le joueur
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!animator.GetBool("isDead")) //Si l'ennemi n'est pas considéré comme mort dans l'animator
        {
            if (enemy.attacked == true) //Si l'ennemi s'est pris un coup
            {
                if (stopmoving != null) //Il arrête de bouger pendant un court moment
                {
                    StopCoroutine(stopmoving);
                }
                stopmoving = StartCoroutine(StopMoving()); //Coroutine stoppant son mouvement
                animator.SetFloat("Speed", 0); //On met à jour ses animations en modifiant la vitesse dans l'animator
            }

            if (Vector2.Distance(transform.position, target.transform.position) < visionDistance) //Si le joueur est visible par l'ennemi
            {
                animator.SetBool("PlayerNear", true);
                if (animator.GetBool("SpellAvailable")) //Si son sort est disponible
                {
                    ThrowFireball(); //Il lance une boule de feu sur le joueur
                    if (attacking != null)
                    {
                        StopCoroutine(attacking);
                    }
                    attacking = StartCoroutine(StopMovingAttack()); //Il arrête de bouger pendant un court moment
                }
            }

            //Si la distance entre le joueur et l'ennemi est supérieure à la distance d'arrêt et inférieure au champ de vision de l'ennemi, et si l'ennemi peut bouger...
            if (Vector2.Distance(transform.position, target.transform.position) > stoppingDistance && Vector2.Distance(transform.position, target.transform.position) < visionDistance && canMove)
            {
                MoveEnemy(); //... On fait bouger l'ennemi
                animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x)); //On met à jour ses animations en conséquence
                if (rb.velocity.x >= 0.01f && !facingRight) //S'il va à droite et regarde à gauche, on le flip
                {
                    Flip();
                }
                else if (rb.velocity.x <= -0.01f && facingRight) //S'il va à gauche et regarde à droite, on le flip
                {
                    Flip();
                }
            }
            else if (Vector2.Distance(transform.position, target.transform.position) <= stoppingDistance) //Si l'ennemi atteint sa distance minimale avec le joueur
            {
                animator.SetTrigger("Attack"); //Il le frappe
                if (attacking != null)
                {
                    StopCoroutine(attacking);
                }
                attacking = StartCoroutine(StopMovingAttack()); //Puis arrête de bouger pendant un instant
            }
            else //Si rien de tout ça n'est validé, l'ennemi ne bouge pas et ne voit pas le joueur, il reste en Idle
            {
                animator.SetBool("PlayerNear", false);
                animator.SetFloat("Speed", 0);
            }
        }
    }

    private void ThrowFireball() //Fonction pour lancer une boule de feu
    {
        StartCoroutine(FireballCooldown()); //Déclenchement du cooldown
        Instantiate(fireball, transform.position, Quaternion.identity);
    }

    IEnumerator FireballCooldown()
    {
        animator.SetBool("SpellAvailable", false);
        yield return new WaitForSeconds(4f); //Cooldown de la boule de feu de 4 sec
        animator.SetBool("SpellAvailable", true);
    }
    public void Attack() //Fonction d'attaque du joueur
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer); //Tableau représentant tous les "joueurs" touchés par l'attaque (ici il n'y en a forcément qu'un)

        foreach (Collider2D enemy in hitEnemies) //Pour chaque ennemi touché
        {
            enemy.GetComponent<PlayerInventory>().LessLife(GetComponent<DamageEnemy>().damage); //On déclenche leur fonction qui leur fait perdre de la vie
            float buff = transform.position.x - enemy.transform.position.x; //On déclenche l'impulsion qui les repousse
            if (buff < 0)
            {
                buff = -GetComponent<DamageEnemy>().impulseX;
            }
            else
            {
                buff = GetComponent<DamageEnemy>().impulseX;
            }
            enemy.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            enemy.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-buff, GetComponent<DamageEnemy>().impulseY), ForceMode2D.Impulse);
        }
    }

    IEnumerator StopMoving() //Coroutine permettant de stopper le déplacement de l'ennemi après avoir reçu un coup
    {
        canMove = false;
        yield return new WaitForSeconds(hurtStopMovementTime);
        canMove = true;
    }

    IEnumerator StopMovingAttack() //Coroutine permettant de stopper le déplacement de l'ennemi après avoir mis un coup
    {
        canMove = false;
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(attackStopMovementTime);
        canMove = true;
    }

    private void MoveEnemy() //Fonction de déplacement de l'ennemi
    {
        if (target.transform.position.x < transform.position.x) //Si le joueur est à gauche de l'ennemi, celui-ci se déplace à gauche
        {
            if (rb.velocity.y > 0.2)
            {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(-speed, 0), visionDistance);
            }
            else
            {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(-speed, rb.velocity.y * 3), visionDistance);
            }
        }
        else //Et inversement
        {
            if (rb.velocity.y > 0.2)
            {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(speed, 0), visionDistance);
            }
            else
            {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(speed, rb.velocity.y * 3), visionDistance);
            }
        }
    }

    private void Flip() //Fonction permettant de faire regarder à gauche ou à droite l'ennemi selon son déplacement pour que cela soit cohérent
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
        facingRight = !facingRight;
    }

    private void OnDrawGizmosSelected() //Cette fonction sert à ajuster le rayon d'attackPoint visuellement sur Unity
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
