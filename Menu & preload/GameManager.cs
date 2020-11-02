using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] SystemPrefabs; //Liste d'objets système (d'autres managers) à instancier dès le début
    public Events.EventGameState OnGameStateChanged; //Event qui se lance quand le State change
    public Events.EventGameDifficulty OnGameDifficultyChanged; //Evente qui se lance quand la difficulté change
    private List<AsyncOperation> _loadOperations; //Liste d'opérations de chargement
    private List<GameObject> _instanciedSystemPrefabs; //Liste des systemPrefabs qui ont été instanciés
    private string _currentLevelName = string.Empty; //Nom du niveau actuel
    private GameState _currentGameState = GameState.CINEMATIC; //State actuel
    private GameDifficulty _currentGameDifficulty = GameDifficulty.EASY; //Difficulté actuelle
    

    public enum GameState //Ensemble des State
    {
        PREGAME, //Menu essentiellement
        RUNNING, //En train de jouer, en plein niveau
        PAUSED, //Jeu en pause (pendant un RUNNING)
        LOSE, //Le joueur a perdu
        WIN, //Le joueur a gagné
        DIALOGUE, //Un dialogue est en cours
        CINEMATIC //Une cinématique est en cours
    }

    public GameState CurrentGameState //Renvoie le State actuel
    {
        get { return _currentGameState; }
    }

    public enum GameDifficulty //Ensemble des difficultés
    {
        EASY,
        HARD
    }

    public GameDifficulty CurrentGameDifficulty //Renvoie la difficulté actuelle
    {
        get { return _currentGameDifficulty; }
    }

    public void UpdateDifficulty(GameDifficulty difficulty) //Fonction de mise à jour de la difficulté
    {
        switch(difficulty)
        {
            case GameDifficulty.EASY:
                {
                    _currentGameDifficulty = GameDifficulty.EASY;
                }break;
            case GameDifficulty.HARD:
                {
                    _currentGameDifficulty = GameDifficulty.HARD;
                }break;
        }
        OnGameDifficultyChanged.Invoke(_currentGameDifficulty);
    }

    public string LevelName //Renvoie le nom du niveau actuel
    {
        get { return _currentLevelName; }
    }

    
    public void RestartGame() //Retour au menu
    {
        if (_currentGameState != GameState.PREGAME)
        {
            UpdateState(GameState.PREGAME);
        }
    }
    public void TogglePause() //Passage du jeu en pause ou retour au jeu depuis une pause
    {
        if (_currentGameState == GameState.RUNNING)
        {
            UpdateState(GameState.PAUSED);
        }
        else if (_currentGameState == GameState.PAUSED)
        {
            UpdateState(GameState.RUNNING);
        }
    }

    public void TriggerLose() //Déclenchement de la défaite du joueur
    {
        if(_currentGameState != GameState.LOSE)
        {
            UpdateState(GameState.LOSE);
        }
    }

    public void TriggerWin() //Déclenchement de la victoire du joueur
    {
        if(_currentGameState != GameState.WIN)
        {
            UpdateState(GameState.WIN);
        }
    }
    public void SwitchToDialogue() //Permet de switch entre RUNNING et DIALOGUE à volonté, pratique pour entrer et sortir d'un dialogue
    {
        if(_currentGameState == GameState.RUNNING)
        {
            UpdateState(GameState.DIALOGUE);
        }
        else if(_currentGameState == GameState.DIALOGUE)
        {
            UpdateState(GameState.RUNNING);
        }
    }

    public void SwitchToCinematic() //Permet de switch entre RUNNING et CINEMATIC à volonté, pratique pour les cutscenes
    {
        if(_currentGameState == GameState.RUNNING)
        {
            UpdateState(GameState.CINEMATIC);
        }
        else if (_currentGameState == GameState.CINEMATIC)
        {
            UpdateState(GameState.RUNNING);
        }
    }

    private void UpdateState(GameState state) //Mise à jour du State actuel
    {
        GameState previousGameState = _currentGameState;
        switch (state)
        {
            case GameState.PREGAME:
                {
                    _currentGameState = GameState.PREGAME;
                    Time.timeScale = 1f;
                    Debug.Log("Pregame");
                }
                break;
            case GameState.RUNNING:
                {
                    _currentGameState = GameState.RUNNING;
                    Time.timeScale = 1f;
                    Debug.Log("Running");
                }
                break;
            case GameState.PAUSED:
                {
                    _currentGameState = GameState.PAUSED;
                    Time.timeScale = 0;
                    Debug.Log("Pause");
                }
                break;
            case GameState.LOSE:
                {
                    _currentGameState = GameState.LOSE;
                    Time.timeScale = 1f;
                    Debug.Log("Lose");
                }break;
            case GameState.WIN:
                {
                    _currentGameState = GameState.WIN;
                    Time.timeScale = 1f;
                    Debug.Log("Win");
                }break;
            case GameState.DIALOGUE:
                {
                    _currentGameState = GameState.DIALOGUE;
                    Time.timeScale = 1f;
                    Debug.Log("Dialogue");
                }break;
            case GameState.CINEMATIC:
                {
                    _currentGameState = GameState.CINEMATIC;
                    Time.timeScale = 1f;
                    Debug.Log("Cinematic");
                }break;
            default:
                {
                    _currentGameState = GameState.PREGAME;
                    Time.timeScale = 1f;
                    Debug.Log("Pregame");
                }
                break;
        }
        OnGameStateChanged.Invoke(_currentGameState, previousGameState);
    }

    public void LoadLevel(string levelName) //Chargement d'un niveau
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level" + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName) //Déchargement d'un niveau
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level" + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }

    private void OnLoadOperationComplete(AsyncOperation ao) //Fin de chargement de niveau
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);
        }
        if (_loadOperations.Count == 0)
        {
            UpdateState(GameState.RUNNING);
        }
        Debug.Log("Load Complete");
    }
    private void OnLoadOperationCompleteCinematic(AsyncOperation ao) //Fin de chargement de niveau
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);
        }
        Debug.Log("Load Complete");
    }

    private void LoadCinematic() //Au début du lancement du jeu, on charge OpenCinematic
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("OpenCinematic", LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level OpenCinematic");
            return;
        }
        ao.completed += OnLoadOperationCompleteCinematic;
        _loadOperations.Add(ao);
        _currentLevelName = "OpenCinematic";
    }
    private void OnUnloadOperationComplete(AsyncOperation ao) //Fin de déchargement de niveau
    {
        Debug.Log("Unload Complete");
    }

    private void Start()
    {
        _loadOperations = new List<AsyncOperation>();
        _instanciedSystemPrefabs = new List<GameObject>();
        InstantiateSystemPrefabs();
        LoadCinematic(); //Chargement de la cinématique de départ
    }

    void InstantiateSystemPrefabs() //On instancie tous les éléments nécessaires au système de jeu (Les managers comme UIManager)
    {
        GameObject prefabInstance;
        for (int i = 0; i < SystemPrefabs.Length; i++)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instanciedSystemPrefabs.Add(prefabInstance);
        }
    }

    protected override void OnDestroy()
    {
        for (int i = 0; i < _instanciedSystemPrefabs.Count; i++)
        {
            Destroy(_instanciedSystemPrefabs[i]);
        }
        _instanciedSystemPrefabs.Clear();
        base.OnDestroy();
    }

    public void StartGame(string levelname) //Lance un niveau
    {
        LoadLevel(levelname);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Si on appuie sur echap...
        {

            if (_currentGameState != GameState.RUNNING && _currentGameState != GameState.PAUSED) //... et que le State actuel est Pause ou Running
            {
                return;
            }
            TogglePause(); //On met le jeu en pause ou on le reprend depuis une pause
        }
        
    }


}