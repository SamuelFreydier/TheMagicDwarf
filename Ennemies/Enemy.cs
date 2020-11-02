using System.Collections;
using UnityEngine;


public enum EnemyType { //Enumération permettant de typer les ennemis. Sert surtout pour savoir quels sons déclencher en fonction du type.
    Mushroom,
    Crystal,
    CrystalBoss
}
public class Enemy : MonoBehaviour
{
    private Animator animator;
    public EnemyType type; //L'ennemi a un type
    public GameObject deathParticles; //Particules de sang déclenchées à la mort de l'ennemi
    public int maxHealth = 100; //Vie de départ de l'ennemi
    public bool attacked = false; //Est-ce que l'ennemi s'est pris un coup ?
    private int currentHealth; //Vie actuelle
    [SerializeField] private bool isWinTrigger = false; //Est-ce que tuer cet ennemi fait gagner la partie ?

    public float deathDuration = 0.1f; //Durée entre le moment où l'ennemi passe à 0 PV et le moment où il disparaît cruellement de ce monde

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        //Vie actuelle = Vie max au début
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) //Fonction de prise de dégâts de l'ennemi
    {
        
        if (!attacked) //S'il ne se fait pas déjà attaquer (utile pour un ennemi avec plusieurs colliders)
        {
            attacked = true; //On signale qu'il se fait attaquer
            currentHealth -= damage; //On lui enlève le montant de dégâts en PV

            if (currentHealth <= 0) //Si la vie actuelle arrive à 0...
            {
                Die(); //... L'ennemi meurt
            }
            StartCoroutine(WaitCollider()); //On attend un peu (problème des colliders)
        }
    }

    private void Die() //Fonction de mort de l'ennemi
    {
        animator.SetBool("isDead", true); //On fait l'animation de mort
        if(type == EnemyType.Crystal)
        {
            AudioManager.Instance.InstantPlay("CrystalDeath", 1f); //On joue le son de mort si c'est un cristal
        }
        GetComponent<Collider2D>().enabled = false; //On désactive le collider de l'ennemi pour permettre le déplacement du joueur directement à travers cet ennemi
        DestroyDeath(); //On détruit l'ennemi après une certaine durée
        if(isWinTrigger) //Si tuer cet ennemi était une condition de victoire
        {
            GetComponent<WallBoss>().DestroyWall(); //Le mur derrière lui est détruit
        }
    }

    private void DestroyDeath() //Fonction de destruction de l'ennemi à sa mort
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity); //On instancie les particules de sang
        Destroy(gameObject, deathDuration); //On détruit l'ennemi après une durée indiquée
    }

    IEnumerator WaitCollider()
    {
        yield return new WaitForSeconds(0.2f);
        attacked = false;
    }
}
