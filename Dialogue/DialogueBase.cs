using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue", menuName ="Dialogues")]
public class DialogueBase : ScriptableObject //Scriptable Object permettant de générer des dialogues facilement
{
    [System.Serializable]
    public class Info //La classe info représente une ligne de dialogue (nom de l'interlocuteur, son portrait, le doublage...)
    {
        public string myName;
        public Sprite myPortrait;
        [TextArea(4, 8)]
        public string myText;
        public string mySound;
    }

    public Info[] dialogueInfo; //Un dialogue est un ensemble de lignes de dialogue
    
}
