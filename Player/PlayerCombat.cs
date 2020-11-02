using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Animator animator;

    public Transform attackPoint; //Représente l'épée du joueur par un cercle
    public LayerMask enemyLayers; //Le layer qui sera affecté par les coups d'épée


    private SpriteRenderer playerRenderer;
    private Transform playerTransform;

    public GameObject fireball;

    public int attackDamage = 20; //Montant de dégâts à chaque coup
    private int startAttackDamage; //Montant de départ, c'est juste un indicateur
    public float attackRange = 0.5f; //Portée d'attaque, rayon de attackPoint
    public float attackPulseX; //Impulsion de l'attaque en X sur les ennemis
    public float attackPulseY; //Impulsion de l'attaque en Y

    public float attackRate = 2f; //Cadence d'attaque
    private float startAttackRate; //Indicateur : cadence de départ
    public float stopMovement = 1.5f; //Temps pendant lequel le joueur ne peut pas se déplacer après avoir attaqué
    public bool canMove = true; //Le joueur peut-il se déplacer ?
    private float nextAttackTime = 0f; //Temps restant avant la prochaine attaque

    private bool canCastFireball = true; //Est-ce que la boule de feu est disponible ?

    private Color playerBaseColor = new Color(155, 155, 155, 255);
    private Coroutine attackRateAxe;

    private void Awake()
    {
        //Ajustement de variables selon la difficulté
        if (GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.EASY)
        {
            attackDamage = 50;
            attackRange = 1.04f;
            attackRate = 2.2f;
        }
        if (GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.HARD)
        {
            attackDamage = 40;
            attackRange = 0.90f;
            attackRate = 2f;
        }
    }
    private void Start()
    {
        startAttackDamage = attackDamage;
        startAttackRate = attackRate;
        playerRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GetComponent<Transform>();
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            if (Time.time >= nextAttackTime) //Si une attaque est possible
            {
                if (Input.GetKeyDown(KeyCode.E) && animator.GetBool("isGrounded")) //Si le joueur appuie sur E et est au sol
                {
                    animator.SetTrigger("Attack"); //Il attaque
                    AudioManager.Instance.InstantPlay("AxeSwing", 1f); //Le bruit du swing de la hache est déclenché
                    nextAttackTime = Time.time + 1f / attackRate; //Le timer avant la prochaine attaque se déclenche
                    if (attackRateAxe != null)
                    {
                        StopCoroutine(attackRateAxe);
                    }
                    attackRateAxe = StartCoroutine(StopMovement()); //Une coroutine va stopper le déplacement du joueur pendant un temps donné
                }

            }
            if (Input.GetKeyDown(KeyCode.R) && animator.GetBool("isGrounded") && canCastFireball) //Si le joueur est au sol, appuie sur R et que la boule de feu n'est pas en CD
            {
                ThrowFireball(); //La boule de feu est lancé
                UIManager.Instance.UIPlayer.TriggerCooldown(); //Le CD est enclenché visuellement sur l'UI
                StartCoroutine(StopMovement()); //Le joueur arrête de bouger un instant
                StartCoroutine(FireballCooldown()); //Le CD est enclenché
            }
        }
    }

    IEnumerator FireballCooldown() //CD de la boule de feu : 6 sec
    {
        canCastFireball = false;
        yield return new WaitForSeconds(6f);
        canCastFireball=true;
    }
    IEnumerator StopMovement() //Coroutine stoppant momentanément le joueur après une attaque
    {
        canMove = false;
        yield return new WaitForSeconds(stopMovement);
        canMove = true;
    }

    private void ThrowFireball() //Lance une boule de feu
    {
        animator.SetTrigger("MagicAttack"); //L'animator réagit
        Instantiate(fireball, transform.position, Quaternion.identity); //La boule de feu est instanciée
    }
    public void Attack() //Fonction d'attaque du joueur
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers); //Tableau représentant tous les ennemis touchés par l'épée
        if(hitEnemies.Length != 0) //Si au moins un ennemi est touché, on instancie le son de hache qui tranche dans le lard !
        {
            AudioManager.Instance.InstantPlay("AxeHit", 1f);
        }

        foreach(Collider2D enemy in hitEnemies) //Pour chaque ennemi touché
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage); //On déclenche leur fonction qui leur fait perdre de la vie
            float buff = transform.position.x - enemy.transform.position.x; //On déclenche l'impulsion qui les repousse
            if (buff < 0)
            {
                buff = -attackPulseX;
            }
            else
            {
                buff = attackPulseX;
            }
            enemy.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            enemy.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-buff, attackPulseY), ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected() //Cette fonction sert à ajuster le rayon d'attackPoint visuellement sur Unity
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
