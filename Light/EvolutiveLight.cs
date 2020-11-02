using UnityEngine;

public class EvolutiveLight : MonoBehaviour //Script de la lumière des sacs de spores évoluant au cours du temps
{

    public float evolutionIntensity; //On choisit l'intensité par frame
    public float evolutionRange; //On choisit l'évolution du radius de la lumière
    public float timeColorEvolution; //Combien de temps mettra-t-elle pour basculer totalement sur une lumière complètement verte

    private void Update()
    {
        GetComponent<Light>().intensity += evolutionIntensity;
        GetComponent<Light>().spotAngle += evolutionRange;
        GetComponent<Light>().color -= (Color.white / timeColorEvolution) * Time.deltaTime;
        GetComponent<Light>().color += (Color.green / timeColorEvolution) * Time.deltaTime;
    }
}
