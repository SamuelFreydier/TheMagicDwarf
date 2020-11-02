using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> //Classe permettant d'avoir une instance globale et une seule d'une autre classe choisie => Pour les managers
{
    private static T instance;
    public static bool isInitialized = false;

    public static T Instance
    {
        get { return instance; }
    }

    public static bool IsInitialized
    {
        get { return isInitialized; }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class.");
            isInitialized = true;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = (T)this;
            isInitialized = true;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance != null)
        {
            instance = null;
            isInitialized = false;
        }
    }
}