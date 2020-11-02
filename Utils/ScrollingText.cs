using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
public class ScrollingText : MonoBehaviour //Script gérant le scrolling text (OpenCinematic)
{

    [SerializeField] private RectTransform paneltransform;
    [SerializeField] private Button nextButton;
    [SerializeField] private Animation cinematicAnimator;
    [SerializeField] private AnimationClip cinematicFadeOut;
    [SerializeField] private Camera cinematicCamera;
    private bool Stop = false;

    void Start()
    {
        nextButton.onClick.AddListener(QuitCinematic); //Un bouton permet de quitter la cinématique prématurément
        StartCoroutine(WaitBegin()); //On attend au début avant de défiler le texte
        StartCoroutine(AudioManager.Instance.Play("IntroMusic", 1f, 1f)); //La musique est déclenchée
    }

    private void QuitCinematic() //Permet de quitter la cinématique
    {
        FadeOut();
    }

    private void FadeOut() //Fondu de sortie
    {
        cinematicAnimator.clip = cinematicFadeOut;
        cinematicAnimator.Play();
        Stop = true;
        StopAllCoroutines();
        StartCoroutine(AudioManager.Instance.StopFadeOut("IntroMusic", .5f)); //On arrête la musique
    }

    public void FadeOutComplete()
    {
        cinematicCamera.gameObject.SetActive(false);
        GameManager.Instance.RestartGame(); //On va sur le menu (PREGAME)
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!Stop)
        {
            if (paneltransform.localPosition.y < 1510) //Défilement
            {
                paneltransform.localPosition += new Vector3(0, 0.6f, 0);
            }
            else
            {
                //Si on arrive à la fin du défilement, le texte se stoppe avant de disparaître
                StartCoroutine(WaitEnd());
            }
        }
    }

    IEnumerator WaitBegin()
    {
        Stop = true;
        yield return new WaitForSeconds(2.5f);
        Stop = false;
    }
    IEnumerator WaitEnd()
    {
        Stop = true;
        yield return new WaitForSeconds(5f);
        FadeOut();
    }
}
