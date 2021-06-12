using TMPro;
using UnityEngine;
//Hey! This is Corey... I'm probably going to write some extra verbose comments for this and the other UI stuff, 
//as I'm just learning which structures are which in Unity. Feel free to ignore the comments if you're already 
//familiar with unity modules and stuff! This is more just to help me develop an understanding (:

//This class uses a rectTransform to resize the text which appears in chatbox

//adds text to the autosizetext class using scripting API
[RequireComponent (typeof (TMP_Text))]
//all unity classes inherit from MonoBehaviour
public class AutosizeText : MonoBehaviour {
    //class has a text and a recttransform attribute
    private TMP_Text text;
    private RectTransform rectTransform;
    // [SerializeField] is used to tell Unity to show this variable in the editor
    [SerializeField] private float maxWidth = 300;
    //void signifies that the function has no return value 
    //Gets the text and rectTransform inputs
    void Start () {
        text = GetComponent<TMP_Text> ();
        rectTransform = GetComponent<RectTransform> ();
    }
    //updates by scaling the rectTransform to match the preferred height and width of the text
    void Update () {
        rectTransform.sizeDelta = new Vector2 (Mathf.Min(text.preferredWidth, maxWidth), text.preferredHeight);
    }
}