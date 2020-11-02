using System;
using System.Collections;
using UnityEngine;
#pragma warning disable 0649
public class AudioManager : Singleton<AudioManager> //Manager gérant les sons, musiques et doublages
{
    [SerializeField] private Sounds[] sounds; //On range tous les sons dans le manager avant de lancer le jeu (dans un prefab)
    Coroutine mainMusic;
    private void Start()
    {
        foreach (Sounds s in sounds) //Lorsque le jeu se lance, on crée les audiosource avec les caractéristiques propres à chaque son
        {
            GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.group;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState currentstate, GameManager.GameState previousstate) //Transition entre des musiques en fonction du GameState
    {
        if(currentstate == GameManager.GameState.RUNNING && previousstate != GameManager.GameState.RUNNING && previousstate != GameManager.GameState.DIALOGUE && previousstate != GameManager.GameState.PAUSED && previousstate != GameManager.GameState.CINEMATIC)
        {
            PlayMusicHandler(GameManager.Instance.LevelName); //On lance la musique du level courant
        }
        if(currentstate != GameManager.GameState.RUNNING && currentstate !=GameManager.GameState.DIALOGUE && currentstate != GameManager.GameState.CINEMATIC && currentstate !=GameManager.GameState.PAUSED && (previousstate == GameManager.GameState.RUNNING || previousstate == GameManager.GameState.PAUSED))
        {
            StopMusicHandler(GameManager.Instance.LevelName); //On stoppe la musique du level courant
        }
    }
    private void StopMusicHandler(string levelname) //Stoppe la musique du level courant
    {
        switch(levelname)
        {
            case "Level1":
                {
                    if (mainMusic != null)
                    {
                        StopCoroutine(mainMusic);
                    }
                    mainMusic = StartCoroutine(StopFadeOut("Level1Music", 1f));
                }break;
            case "Level2":
                {
                    if (mainMusic != null)
                    {
                        StopCoroutine(mainMusic);
                    }
                    mainMusic = StartCoroutine(StopFadeOut("Level2Music", 1f));
                }break;
        }
    }

    public void InstantPlay(string name, float maxvolume) //Lance un son sans transition
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found !");
        }
        s.source.Play();
    }

    public void InstantStop(string name) //Stoppe un son sans transition
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found !");
        }
        s.source.Stop();
    }
    private void PlayMusicHandler(string levelname) //Lance la musique du level courant
    {
        switch(levelname)
        {
            case "Level1":
                {
                    if (mainMusic != null)
                    {
                        StopCoroutine(mainMusic);
                    }
                    mainMusic = StartCoroutine(Play("Level1Music", 0.489f, 2f));
                }break;
            case "Level2":
                {
                    if (mainMusic != null)
                    {
                        StopCoroutine(mainMusic);
                    }
                    mainMusic = StartCoroutine(Play("Level2Music", 1f, 2f));
                }break;
        }
    }

    public IEnumerator Play(string name, float maxvolume, float time) //Lance un son avec une transition de fondu
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found !");
        }
        Debug.Log("Beginning play of " + name);
        float startVolume = 0.2f;
        maxvolume = s.volume;
        s.source.volume = 0;
        s.source.Play();
        while(s.source.volume < maxvolume)
        {
            s.source.volume += startVolume * Time.deltaTime / time;
            yield return null;
        }
        s.source.volume = maxvolume;
        Debug.Log("End Play of " + name);
    }

    public IEnumerator StopFadeOut(string name, float time) //Stoppe un son avec une transition de fondu
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found !");
        }
        Debug.Log("FadeOut of " + name);
        float startVolume = s.source.volume;
        while(s.source.volume > 0)
        {
            s.source.volume -= startVolume * Time.deltaTime / time;
            yield return null;
        }
        s.source.Stop();
        s.source.volume = startVolume;
        Debug.Log("End FadeOut of " + name);
    }
}