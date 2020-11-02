using UnityEngine;

public class WinPortal : MonoBehaviour //Portail de fin qui déclenche la victoire
{
    public GameObject gameCleaner;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gameCleaner.GetComponent<Timer>().Finish();
            GameManager.Instance.TriggerWin();
        }
    }
}
