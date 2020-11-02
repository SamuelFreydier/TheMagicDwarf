using UnityEngine;

public class Timer : MonoBehaviour //Fonction permettant d'enclencher le timer du niveau courant
{
    private float startTime;
    private bool finished = false;
    private float t;
    private float countdown;
    
    void Start()
    {
        startTime = Time.time;
        if(GameManager.Instance.LevelName == "Level2") //Si le niveau courant est le deuxième, on déclenche aussi le countdown
        {
            countdown = 130;
        }
    }

    void Update()
    {
        if(finished)
        {
            return;
        }
        countdown -= Time.deltaTime;
        t = Time.time - startTime;
        if (GameManager.Instance.LevelName == "Level2")
        {
            
            ScoreManager.Instance.UpdateCountdown(countdown); //Mise à jour du countdown (level 2)
            if (countdown <= 0)
            {
                GameManager.Instance.TriggerLose(); //Si le countdown atteint 0, le joueur perd
            }
        }
        ScoreManager.Instance.UpdateTime(GameManager.Instance.LevelName, t); //Mise à jour du temps courant
    }

    public void Finish() //Fonction appelée à la victoire
    {
        finished = true;
        ScoreManager.Instance.UpdateBestTime(GameManager.Instance.LevelName, t); //On met à jour les meilleurs temps
    }
}
