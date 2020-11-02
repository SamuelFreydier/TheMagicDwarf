using System.Collections;
using UnityEngine;

public class EnemyFollow : MonoBehaviour //Classe gérant le "pathfinding" d'un ennemi qui doit suivre le joueur
{
    private Enemy enemy;
    private Animator animator;
    private Rigidbody2D rb;
    private GameObject target; //Cible des ennemis (ce sera le joueur)
    public Transform attackPoint;
    public float attackRange = 0.1f;
    public LayerMask playerLayer;

    public float speed; //Vitesse de l'ennemi
    public float stoppingDistance; //Distance par rapport au joueur à laquelle l'ennemi arrêtera d'avancer
    public float visionDistance; //Champ de vision de l'ennemi
    public float hurtStopMovementTime; //Durée pendant laquelle l'ennemi arrête de bouger après avoir reçu un coup
    public float attackStopMovementTime;
    private bool canMove = true; //L'ennemi peut-il bouger ?
    private bool facingRight = false; //Où regarde l'ennemi ?

    private void Awake()
    {
        //Ajustement des statistiques de l'ennemi selon la difficulté
        if(GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.HARD)
        {
            speed *= 1.2f;
            attackRange *= 1.2f;
            visionDistance *= 1.2f;
        }
        
        target = GameObject.FindGameObjectWithTag("Player"); //La cible est le joueur
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!animator.GetBool("isDead")) //Si l'ennemi n'est pas considéré comme mort par l'animator
        {
            if (enemy.attacked == true) //Si l'ennemi s'est pris un coup
            {
                StopAllCoroutines();
                StartCoroutine(StopMoving()); //Coroutine stoppant son mouvement
                animator.SetFloat("Speed", 0); //On met à jour ses animations en modifiant la vitesse dans l'animator
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
            else if (Vector2.Distance(transform.position, target.transform.position) <= stoppingDistance) //Si la distance minimale entre le joueur et l'ennemi est atteinte
            {
                rb.velocity = new Vector2(0, 0); //L'ennemi arrête de bouger
                animator.SetTrigger("Attack"); //Et frappe le joueur
                StopAllCoroutines();
                StartCoroutine(StopMovingAttack());
            }
            else //Sinon, l'ennemi arrête juste de bouger et reste en Idle
            {
                animator.SetFloat("Speed", 0);
            }
        }
    }

    public void Attack() //Fonction d'attaque du joueur
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer); //Tableau représentant tous les "joueurs" touchés
        if (hitEnemies.Length != 0) //Si le joueur est touché, on déclenche les sons en fonction du type de l'ennemi
        {
            if(enemy.type == EnemyType.Mushroom)
            {
                AudioManager.Instance.InstantPlay("MushroomHit", 1f);
            }
            else if (enemy.type == EnemyType.Crystal)
            {
                AudioManager.Instance.InstantPlay("CrystalHit", 1f);
            }
            else
            {
                AudioManager.Instance.InstantPlay("CrystalBossHit", 1f);
            }
        }

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

    IEnumerator StopMovingAttack() //Coroutine permettant de stopper le déplacement de l'ennemi après avoir reçu un coup
    {
        canMove = false;
        yield return new WaitForSeconds(attackStopMovementTime);
        canMove = true;
    }

    private void MoveEnemy() //Fonction de déplacement de l'ennemi
    {
        if(target.transform.position.x < transform.position.x) //Si le joueur est à gauche de l'ennemi, celui-ci se déplace à gauche
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
