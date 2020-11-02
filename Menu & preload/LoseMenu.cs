using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
public class LoseMenu : MonoBehaviour
{
    [SerializeField] Animation _loseMenuAnimator;
    [SerializeField] AnimationClip _fadeOutAnimationClip;
    [SerializeField] AnimationClip _fadeInAnimationClip;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button MenuButton;
    [SerializeField] private Button QuitButton;
    public Events.EventLoseWinFadeComplete OnLoseMenuFadeComplete;
    private bool choice;
    Coroutine musicfade;

    private void Start()
    {
        MenuButton.onClick.AddListener(HandleMenuClicked);
        RestartButton.onClick.AddListener(HandleRestartClicked);
        QuitButton.onClick.AddListener(HandleQuitClicked);
    }
    private void Update()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged); //On écoute les changements de State
    }
    private void HandleRestartClicked() //Si on clique sur le bouton Restart
    {
        choice = false; //false pour le choix "restart" qui relance le niveau
        FadeOut(); //On sort du menu de défaite
    }

    private void HandleMenuClicked() //Si on clique sur menu
    {
        choice = true; //true pour le choix "menu"
        FadeOut(); //On sort du menu de défaite
    }

    private void HandleQuitClicked() //Si on clique sur Quit
    {
        Application.Quit(); //On quitte le jeu
    }

    void HandleGameStateChanged(GameManager.GameState currentstate, GameManager.GameState previousstate)
    {
        if (previousstate == GameManager.GameState.LOSE && currentstate != GameManager.GameState.LOSE) //Si l'ancien State est LOSE et le nouveau n'est pas LOSE, on fait la transition de sortie
        {
            FadeOut();
        }
    }

    public void OnFadeOutComplete() //A la fin de l'animation de sortie du menu de défaite
    {
        OnLoseMenuFadeComplete.Invoke(true, choice); //On envoie un event indiquant que FadeOut a été fait pour un certain choix (Restart ou Menu)
        gameObject.SetActive(false); //On désactive le menu de défaite
    }

    public void OnFadeInComplete() //A la fin de l'animation d'entrée vers le menu de défaite
    {
        OnLoseMenuFadeComplete.Invoke(false, choice); //On envoie un event indiquant que FadeIn a été fait
        musicfade = StartCoroutine(AudioManager.Instance.Play("LoseMusic", 1f, 1f)); //On joue la musique de défaite
    }

    public void FadeIn() //Début de l'entrée dans le menu de défaite
    {
        gameObject.SetActive(true); //On active le menu
        UIManager.Instance.SetDummyCameraActive(true); //On active la caméra de UIManager
        GameManager.Instance.UnloadLevel(GameManager.Instance.LevelName);
        _loseMenuAnimator.Stop(); //On fait l'animation
        _loseMenuAnimator.clip = _fadeInAnimationClip;
        _loseMenuAnimator.Play();
    }

    public void FadeOut() //Début de la sortie du menu de défaite
    {
        StopCoroutine(musicfade);
        StartCoroutine(AudioManager.Instance.StopFadeOut("LoseMusic", .5f)); //On stoppe la musique de défaite
        _loseMenuAnimator.Stop(); //On fait l'animation
        _loseMenuAnimator.clip = _fadeOutAnimationClip;
        _loseMenuAnimator.Play();
    }
}
