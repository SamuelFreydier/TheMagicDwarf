using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
public class MainMenu : MonoBehaviour
{
    [SerializeField] Animation _mainMenuAnimator;
    [SerializeField] AnimationClip _fadeOutAnimationClip;
    [SerializeField] AnimationClip _fadeInAnimationClip;
    [SerializeField] private Button level1Button; 
    [SerializeField] private Button level2Button; 
    [SerializeField] private Button quitButton;
    [SerializeField] private Button difficultyEasyButton;
    [SerializeField] private Text difficultyEasyText;
    [SerializeField] private Text difficultyHardText;
    [SerializeField] private Button difficultyHardButton;
    [SerializeField] private Text timelvl1Text;
    [SerializeField] private Text timelvl2Text;
    [SerializeField] private Text timeRunText;
    public Events.EventFadeComplete OnMainMenuFadeComplete;
    public Events.EventLoadLevel OnMainMenuLevelLoad;
    Coroutine musicfade;


    private void Update()
    {
        //On met à jour les meilleurs temps dans le menu
        timelvl1Text.text = "Meilleur temps dans le niveau 1 : " + ScoreManager.Instance.BestTimeLvl1.ToString("f2") + " s";
        timelvl2Text.text = "Meilleur temps dans le niveau 2 : " + ScoreManager.Instance.BestTimeLvl2.ToString("f2") + " s";
        timeRunText.text = "Meilleur parcours complet : " + ScoreManager.Instance.BestTimeRun.ToString("f2") + " s";
    }
    private void Start()
    {
        difficultyEasyText.color = new Color(255, 0, 0); //Le bouton Easy est rouge au départ car c'est la difficulté par défaut
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        GameManager.Instance.OnGameDifficultyChanged.AddListener(HandleGameDifficultyChanged);
        level1Button.onClick.AddListener(HandleLoadLevel1Clicked);
        level2Button.onClick.AddListener(HandleLoadLevel2Clicked);
        quitButton.onClick.AddListener(HandleQuitClicked);
        difficultyEasyButton.onClick.AddListener(HandleDifficultyEasyClicked);
        difficultyHardButton.onClick.AddListener(HandleDifficultyHardClicked);
    }

    private void HandleGameDifficultyChanged(GameManager.GameDifficulty currentdifficulty) //Si la difficulté a changé
    {
        if(currentdifficulty==GameManager.GameDifficulty.EASY) //Si la difficulté actuelle est Facile
        {
            difficultyEasyText.color = new Color(255, 0, 0); //Le bouton Easy devient Rouge
            difficultyHardText.color = new Color(255, 255, 255); //Le bouton Hard devient Blanc
        }
        if(currentdifficulty==GameManager.GameDifficulty.HARD) //Inversement pour la difficulté Difficile
        {
            difficultyHardText.color = new Color(255, 0, 0);
            difficultyEasyText.color = new Color(255, 255, 255);
        }
    }

    //Selon le bouton, on envoie un event avec le nom du Level chargé (1 ou 2)
    private void HandleLoadLevel1Clicked() 
    {
        if (!UIManager.Instance.AfterClick)
        {
            OnMainMenuLevelLoad.Invoke("Level1");
        }
    }

    private void HandleLoadLevel2Clicked()
    {
        if (!UIManager.Instance.AfterClick)
        {
            OnMainMenuLevelLoad.Invoke("Level2");
        }
    }



    private void HandleQuitClicked() //Le bouton Quit permet de quitter le jeu
    {
        Application.Quit();
    }

    private void HandleDifficultyEasyClicked() //Le bouton Easy change la difficulté vers Easy
    {
        GameManager.Instance.UpdateDifficulty(GameManager.GameDifficulty.EASY);
    }

    private void HandleDifficultyHardClicked() //Inversement pour le bouton Hard
    {
        GameManager.Instance.UpdateDifficulty(GameManager.GameDifficulty.HARD);
    }
    void HandleGameStateChanged(GameManager.GameState currentstate, GameManager.GameState previousstate)
    {
        if (previousstate==GameManager.GameState.PREGAME && currentstate==GameManager.GameState.RUNNING) //Si l'ancien State était pregame et le nouveau Running
        {
            FadeOut(); //On part du menu
        }
        if (previousstate != GameManager.GameState.PREGAME && currentstate == GameManager.GameState.PREGAME) //Si l'ancien n'était pas pregame et le nouveau si
        {
            if(previousstate != GameManager.GameState.LOSE && previousstate != GameManager.GameState.WIN)
            {
                GameManager.Instance.UnloadLevel(GameManager.Instance.LevelName); //On décharge le niveau
            }
            //FadeIn(); //On entre dans le menu
        }

    }

    public void OnFadeOutComplete() //Fin de sortie du menu
    {
        OnMainMenuFadeComplete.Invoke(true); //On envoie un event pour indiquer la fin de l'anim
        gameObject.SetActive(false); //On désactive le menu
        UIManager.Instance.SetDummyCameraActive(false); //On désactive la caméra UIManager
    }

    public void OnFadeInComplete() //Fin d'entrée dans le menu
    {
        OnMainMenuFadeComplete.Invoke(false); //On envoie un event pour indiquer la fin de l'anim
        musicfade = StartCoroutine(AudioManager.Instance.Play("MenuMusic", 1f, 1f)); //On joue la musique de menu
    }

    public void FadeIn() //Début d'entrée
    {
        Debug.Log("FadIN");
        UIManager.Instance.SetDummyCameraActive(true); //Activation de la caméra
        _mainMenuAnimator.Stop(); //Activation de l'animation
        _mainMenuAnimator.clip = _fadeInAnimationClip;
        _mainMenuAnimator.Play();
    }

    public void FadeOut() //Début de sortie
    {
        StopCoroutine(musicfade);
        StartCoroutine(AudioManager.Instance.StopFadeOut("MenuMusic", .5f)); //On stoppe la musique du menu
        _mainMenuAnimator.Stop(); //Activation de l'animation
        _mainMenuAnimator.clip = _fadeOutAnimationClip;
        _mainMenuAnimator.Play();
    }

    
}
