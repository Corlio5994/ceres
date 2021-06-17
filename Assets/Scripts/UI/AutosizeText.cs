using TMPro;
using UnityEngine;

[RequireComponent (typeof (TMP_Text))]
public class AutosizeText : MonoBehaviour {
    [SerializeField] float maxWidth = 300;
    TMP_Text text;
    RectTransform rectTransform;

    void Start () {
        text = GetComponent<TMP_Text> ();
        rectTransform = GetComponent<RectTransform> ();
    }

    void Update () {
        rectTransform.sizeDelta = new Vector2 (Mathf.Min(text.preferredWidth, maxWidth), text.preferredHeight);
    }
}