using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sounds //Représente un son (utile pour mettre des sons dans le prefab d'AudioManager)
{
    public string name;

    public AudioClip clip;
    public AudioMixerGroup group;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;


    [HideInInspector]
    public AudioSource source;
}
