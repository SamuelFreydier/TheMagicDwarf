using System.Collections;
using UnityEngine;
public class TimelineStop : MonoBehaviour //Permet de stopper une timeline (cinématique)
{

    private bool stoptimeline = false;
    private void Start()
    {
        StartCoroutine(WaitTimeline());
    }

    private void Update()
    {
        if(stoptimeline)
        {
            GameManager.Instance.SwitchToCinematic(); //Si la timeline est stoppée, on repasse en Running
            Destroy(gameObject); //On détruit la timeline car on ne veut pas que le joueur redéclenche la cinématique
        }
    }

    private IEnumerator WaitTimeline()
    {
        yield return new WaitForSeconds(7f);
        stoptimeline = true;
    }
}
