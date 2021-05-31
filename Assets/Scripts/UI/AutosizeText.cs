using TMPro;
using UnityEngine;

[RequireComponent (typeof (TMP_Text))]
public class AutosizeText : MonoBehaviour {
    private TMP_Text text;
    private RectTransform rectTransform;
    [SerializeField] private float maxWidth = 300;

    void Start () {
        text = GetComponent<TMP_Text> ();
        rectTransform = GetComponent<RectTransform> ();
    }

    void Update () {
        rectTransform.sizeDelta = new Vector2 (Mathf.Min(text.preferredWidth, maxWidth), text.preferredHeight);
    }
}