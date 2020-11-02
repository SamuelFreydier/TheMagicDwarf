using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public int Life = 10; //Vie du joueur
    public int maxShield = 10;
    
    private float fill; //Variable utile pour la diminution ou l'augmentation de la barre de vie (visuellement)
    private int LifeMax; //Valeur qui restera à Life dès le départ afin d'avoir un indicateur
    private int Armor = 0; //Montant d'armure
    private int armorDestroyed = 0; //armorDestroyed est un compteur qui s'incrémente à chaque coup pris quand le joueur a de l'armure.
    private int armorDestroyedRoof; //Quand le compteur armorDestroyed arrive à armorDestroyedRoof, le joueur perd 1 d'armure et armorDestroyed se réinitialise
    private void Awake()
    {
        //Selon la difficulté, on change quelques variables
        if (GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.EASY)
        {
            Life = 25;
            armorDestroyedRoof = 2;
        }
        if (GameManager.Instance.CurrentGameDifficulty == GameManager.GameDifficulty.HARD)
        {
            Life = 20;
            armorDestroyedRoof = 1;
        }
    }
    private void Start()
    { 
        LifeMax = Life; //On initalise LifeMax à Life. LifeMax restera alors toujours la même valeur jusqu'à ce que le niveau soit déchargé
    }

    
    public void AddArmor(int shieldarmor) //Fonction permettant l'ajout d'armure
    {
        Armor += shieldarmor;
        fill = (float)shieldarmor / maxShield;
        Debug.Log(fill);
        UIManager.Instance.UIPlayer.UpdateArmor(fill);
        if(Armor>maxShield)
        {
            Armor = maxShield;
        }
    }
    public void AddLife(int heartlife) //Fonction permettant l'ajout de vie
    {
        Life += heartlife;
        fill = (float)heartlife / LifeMax; //La barre de vie (l'image) se remplit en fonction du montant rendu / la vie max possible
        UIManager.Instance.UIPlayer.UpdateHealthBar(fill);       
        if (Life > LifeMax) //Evidemment, on ne peut pas dépasser la vie max
        {
            Life = LifeMax;
        }
    }
    public void LessLife(int damage) //Fonction permettant d'enlever de la vie
    {
        int damagewShield = damage - Armor; //On soustrait aux dégâts le montant d'armure du joueur
        if (damagewShield < 0) //Si les dégâts sont négatifs, cela ne veut pas dire que le joueur se soigne
        {
            damagewShield = 0;
        }
        Life -= damagewShield; //On enlève les PV
        fill = (float)damagewShield / LifeMax; //On met à jour la barre de vie
        UIManager.Instance.UIPlayer.UpdateHealthBar(-fill);
        AudioManager.Instance.InstantPlay("Hurt", 1f); //Son de prise de dégâts
        if (Armor > 0) //Si le joueur a de l'armure...
        {
            armorDestroyed++; //... Le compteur armorDestroyed s'incrémente
        }
        if(armorDestroyed == armorDestroyedRoof) //Si le compteur atteint son plafond...
        {
            Armor--; //... Le joueur perd 1 armure
            fill = 1.0f / maxShield;
            UIManager.Instance.UIPlayer.UpdateArmor(-fill);
            armorDestroyed = 0; //et le compteur se réinitialise
        }
        if (Life <= 0) //Si le joueur atteint 0 PV
        {
            AudioManager.Instance.InstantPlay("Death", 1f); //Son de mort
            GameManager.Instance.TriggerLose(); //La défaite est déclenchée à l'aide du State approprié dans le Game Manager
        }
    }
}
