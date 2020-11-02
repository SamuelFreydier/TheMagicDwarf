using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour //Script permettant d'auto détruire un objet peu après son instanciation
{
    private void Start()
    {
        Destroy(gameObject, 1f);
    }
}
