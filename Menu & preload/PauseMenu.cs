using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
public class PauseMenu : MonoBehaviour
{

    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button QuitButton;


    public void HandleResumeClicked() //Si on clique sur resume
    {
        GameManager.Instance.TogglePause(); //On repasse en State RUNNING donc en jeu
    }

    public void HandleRestartClicked() //Bouton Menu
    {
        GameManager.Instance.RestartGame(); //On va sur le menu et on décharge le level
    }

    public void HandleQuitClicked() //Bouton Quit
    {
        Application.Quit(); //On quitte le jeu
    }
    private void Start()
    {
        ResumeButton.onClick.AddListener(HandleResumeClicked);
        RestartButton.onClick.AddListener(HandleRestartClicked);
        QuitButton.onClick.AddListener(HandleQuitClicked);
    }

}