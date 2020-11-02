using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Animation _playerUIAnimator;
    [SerializeField] AnimationClip _fadeOutAnimationClip;
    [SerializeField] AnimationClip _fadeInAnimationClip;
    [SerializeField] private GameObject _highscorePanel;
    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private GameObject _countdownPanel;
    [SerializeField] private GameObject _alterationPanel;
    [SerializeField] private Text Timer;
    [SerializeField] private Text Countdown;
    [SerializeField] private Text HighScore;
    [SerializeField] private Text CurrentScore;
    [SerializeField] private Text BoostDuration;
    [SerializeField] private Image ShieldBar;
    [SerializeField] private Image healthBar; //Barre de vie
    [SerializeField] private Image fireballImage;
    public float cooldownFireball = 6.0f;
    private float timer = 0;
    public Events.EventFadeComplete OnUIFadeComplete;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void Update()
    {
        UpdateTimerText(); //On update le timer de l'UI
        UpdateCooldown(cooldownFireball); //On update le cooldown (visuellement) avec l'image de la boule de feu
        if(GameManager.Instance.LevelName == "Level2")
        {
            UpdateCountdownText(); //Si c'est le level 2 qui a un countdown, on l'update aussi
        }
    }

    private void UpdateCountdownText()
    {
        Countdown.text = ((int)ScoreManager.Instance.Countdown / 60).ToString()
                        + ":"
                        + (ScoreManager.Instance.Countdown % 60).ToString("f1");
        if(ScoreManager.Instance.Countdown > 20) //En dessous de 20 sec, le countdown vire au rouge
        {
            Countdown.color = new Color(255, 255, 255);
        }
        else
        {
            Countdown.color = new Color(255, 0, 0);
        }
    }
    public void TriggerCooldown() //Début du CD de la boule de feu, on met son fillamount à 0
    {
        fireballImage.fillAmount = 0;
        timer = 0;
    }
    public void UpdateCooldown(float cd) //On update le fillamount à chaque seconde pour la boule de feu
    {
        if (fireballImage.fillAmount < 1.0f)
        {
            timer += Time.deltaTime;
            float fill = timer / cd;
            fireballImage.fillAmount = fill;
        }
    }
    public void UpdateHealthBar(float fill) //On update la barre de vie
    {
        healthBar.fillAmount += fill;
    }

    public void UpdateArmor(float amount) //On update la barre d'armure
    {
        ShieldBar.fillAmount += amount;
    }

    void HandleGameStateChanged(GameManager.GameState currentstate, GameManager.GameState previousstate)
    {
        if (previousstate == GameManager.GameState.RUNNING && currentstate != GameManager.GameState.RUNNING) //Si le précédent state était WIN et le nouveau ne l'est pas
        {
            FadeOut(); //On sort de l'UI
        }
        if (previousstate!= GameManager.GameState.RUNNING && previousstate != GameManager.GameState.PAUSED && currentstate == GameManager.GameState.RUNNING) //Quand on repasse en RUNNING depuis un menu
        {
            healthBar.fillAmount = 1.0f; //On réinitialise les barres de vie et d'armure
            ShieldBar.fillAmount = 0;
        }
    }

    private void UpdateTimerText() //Update du timer
    {
        switch (GameManager.Instance.LevelName)
        {
            case "Level1":
                {
                    Timer.text = ((int)ScoreManager.Instance.TimeLvl1 / 60).ToString()
                        + ":"
                        + (ScoreManager.Instance.TimeLvl1 % 60).ToString("f0");
                }
                break;
            case "Level2":
                {
                    Timer.text = ((int)ScoreManager.Instance.TimeLvl2 / 60).ToString()
                        + ":"
                        + (ScoreManager.Instance.TimeLvl2 % 60).ToString("f0");
                }break;
        }
    }

    public void OnFadeOutComplete() //Fin de sortie
    {
        OnUIFadeComplete.Invoke(true); //On envoie l'event avec le choix entre false -> prochain niveau et true -> menu
        gameObject.SetActive(false); //On désactive le menu
    }

    public void OnFadeInComplete() //Fin d'entrée
    {
        OnUIFadeComplete.Invoke(false); //On envoie l'event
    }

    public void FadeIn() //Début d'entrée
    {
        Debug.Log("FadeInUI");
        gameObject.SetActive(true); //On active l'objet
        CheckLevel();
        _playerUIAnimator.Stop(); //On active l'anim
        _playerUIAnimator.clip = _fadeInAnimationClip;
        _playerUIAnimator.Play();
        fireballImage.fillAmount = 1f;
    }

    public void FadeOut() //Début de sortie
    {
        _playerUIAnimator.Stop(); //On active l'anim
        _playerUIAnimator.clip = _fadeOutAnimationClip;
        _playerUIAnimator.Play();
    }

    private void CheckLevel()
    {
        switch(GameManager.Instance.LevelName) //On check en fonction du niveau les panels à activer ou non
        {
            case "Level1":
                {
                    _timerPanel.SetActive(true);
                    _countdownPanel.SetActive(false);
                }break;
            case "Level2":
                {
                    _countdownPanel.SetActive(true);
                    _timerPanel.SetActive(true);
                }
                break;
        }
    }
}
