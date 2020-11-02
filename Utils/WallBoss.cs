using UnityEngine;

public class WallBoss : MonoBehaviour //Fonction associée au boss
{
    public GameObject wall;
    
    public void DestroyWall() //Si cette fonction est appelée (quand le boss meurt), le mur est détruit
    {
        Destroy(wall);
    }

}
