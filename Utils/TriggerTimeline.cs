using UnityEngine;

public class TriggerTimeline : MonoBehaviour //Permet de déclencher une cinématique
{
    public GameObject pressFIcon; //Indicateur visuel (touche F pour déclencher)
    public GameObject timeline;

    public void TriggerTimelines() //Activation de la timeline
    {
        timeline.SetActive(true);
        GameManager.Instance.SwitchToCinematic(); //On passe de RUNNING à CINEMATIC
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            pressFIcon.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetKeyDown(KeyCode.F) && GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            TriggerTimelines();
            Destroy(pressFIcon);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            pressFIcon.SetActive(false);
        }
    }
}
