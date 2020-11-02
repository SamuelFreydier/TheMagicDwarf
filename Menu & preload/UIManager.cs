using System.Collections;
using UnityEngine;
#pragma warning disable 0649
public class UIManager : Singleton<UIManager> //Manager pour l'UI en général
{
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private Camera _dummyCamera;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private LoseMenu _loseMenu;
    [SerializeField] private WinMenu _winMenu;
    [SerializeField] private PlayerUI _uiMenu;
    public Events.EventFadeComplete OnMainMenuFadeComplete;
    public Events.EventFadeComplete OnUIFadeComplete;
    public Events.EventLoseWinFadeComplete OnLoseMenuFadeComplete;
    public Events.EventLoseWinFadeComplete OnWinMenuFadeComplete;
    private bool _waitAfterClick = false;
    public PlayerUI UIPlayer
    {
        get { return _uiMenu; }
    }
    public bool AfterClick
    {
        get { return _waitAfterClick; }
    }

    private void HandleGameStateChanged(GameManager.GameState currentstate, GameManager.GameState previousstate)
    {
        _pauseMenu.gameObject.SetActive(currentstate == GameManager.GameState.PAUSED); //Si le state PAUSE est actif, on active le menu de pause, sinon on le désactive
        _loseMenu.gameObject.SetActive(currentstate == GameManager.GameState.LOSE); //Pareil pour le menu de défaite...
        _winMenu.gameObject.SetActive(currentstate == GameManager.GameState.WIN); //Et pour le menu de victoire
        _uiMenu.gameObject.SetActive(currentstate == GameManager.GameState.RUNNING && previousstate != GameManager.GameState.PREGAME);
        if(_loseMenu.gameObject.activeInHierarchy)
        {
            _loseMenu.FadeIn(); //Si le menu de défaite a été activée, on déclenche l'entrée
        }
        if(_winMenu.gameObject.activeInHierarchy)
        {
            _winMenu.FadeIn(); //Pareil pour le menu de victoire
        }
        if(_uiMenu.gameObject.activeInHierarchy && previousstate != GameManager.GameState.PREGAME)
        {
            _uiMenu.FadeIn();
        }
        //Si l'ancien state était lose ou win et que le nouveau est running
        if((previousstate == GameManager.GameState.LOSE || previousstate == GameManager.GameState.WIN || previousstate == GameManager.GameState.PAUSED) && currentstate == GameManager.GameState.RUNNING)
        {
            SetDummyCameraActive(false); //on désactive la caméra UI
        }
        if(currentstate == GameManager.GameState.PREGAME)
        {
            if (previousstate == GameManager.GameState.CINEMATIC)
            {
                GameManager.Instance.UnloadLevel("OpenCinematic");
            }
            _mainMenu.gameObject.SetActive(true);
            _mainMenu.FadeIn();
        }

    }

    private void HandleEventFadeCompleted(bool fadeInfadeOut)
    {
        if(GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            _uiMenu.gameObject.SetActive(true);
            _uiMenu.FadeIn();
        }
        OnMainMenuFadeComplete.Invoke(fadeInfadeOut); //On envoie l'event globalement
    }

    private void HandleLoseEventFadeCompleted(bool fadeInfadeOut, bool choice)
    {
        OnLoseMenuFadeComplete.Invoke(fadeInfadeOut, choice); //Global
       
        if (fadeInfadeOut) //Si FadeOut
        {
            if(choice == true) //Si le choix est true
            {
                GameManager.Instance.RestartGame(); //On revient au menu
            }
            if(choice == false) //Sinon
            {
                GameManager.Instance.LoadLevel(GameManager.Instance.LevelName); //On recharge le niveau
            }
        }
    }

    private void HandleWinEventFadeCompleted(bool fadeInfadeOut, bool choice)
    {
        OnWinMenuFadeComplete.Invoke(fadeInfadeOut, choice); //Global
        if (!fadeInfadeOut) //FadeIn
        {
            GameManager.Instance.UnloadLevel(GameManager.Instance.LevelName); //Déchargement de niveau
        }
        if (fadeInfadeOut) //FadeOut
        {
            if (choice == true) //Si true
            {
                GameManager.Instance.RestartGame(); //On va au Menu
            }
            if (choice == false) //Sinon
            {
                GameManager.Instance.LoadLevel("Level2"); //On va au niveau suivant, ici c'est forcément le niveau 2
            }
        }
    }

    private void HandleLoadLevel(string levelname)
    {
        StartCoroutine(WaitAfterClick());
        if(GameManager.Instance.CurrentGameState != GameManager.GameState.PREGAME)
        {
            return;
        }
        GameManager.Instance.StartGame(levelname);
    }

    private void HandleUIEventFadeComplete(bool fadeinfadeout)
    {
        OnUIFadeComplete.Invoke(fadeinfadeout);
    }
    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        _mainMenu.OnMainMenuFadeComplete.AddListener(HandleEventFadeCompleted);
        _mainMenu.OnMainMenuLevelLoad.AddListener(HandleLoadLevel);
        _loseMenu.OnLoseMenuFadeComplete.AddListener(HandleLoseEventFadeCompleted);
        _winMenu.OnWinMenuFadeComplete.AddListener(HandleWinEventFadeCompleted);
        _uiMenu.OnUIFadeComplete.AddListener(HandleUIEventFadeComplete);
    }


    public void SetDummyCameraActive(bool active)
    {
        _dummyCamera.gameObject.SetActive(active);
    }


    IEnumerator WaitAfterClick()
    {
        _waitAfterClick = true;
        yield return new WaitForSeconds(0.5f);
        _waitAfterClick = false;
    }
}
