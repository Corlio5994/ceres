using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour {
    static bool showing = false;
    static TooltipUI instance;

    [SerializeField] RectTransform tooltip;
    [SerializeField] TMP_Text tooltipTitleText;
    [SerializeField] TMP_Text subtitleText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] Vector2 screenPadding;
    [SerializeField] float spacing;
    Vector2 tooltipDirection = Vector2.one;

    public static void ShowTooltip (string title, string subtitle = "", string description = "") {
        instance.tooltipTitleText.text = title;
        instance.subtitleText.text = subtitle;
        instance.descriptionText.text = description;
        showing = true;
        instance.tooltip.gameObject.SetActive (true);
    }

    public static void SetTooltipRarity (Color32 color) {
        instance.tooltipTitleText.color = color;
    }

    public static void HideTooltip () {
        instance.tooltip.gameObject.SetActive (false);
        showing = false;
    }

    void Awake () {
        instance = this;
        HideTooltip ();
    }

    void Update () {
        if (showing) {
            Vector2 size = tooltip.sizeDelta + 2 * new Vector2 (spacing, spacing);
            Vector2 offset = size / 2;
            Vector2 mousePosition = Input.mousePosition;

            if (tooltipDirection.x == -1 && mousePosition.x < screenPadding.x + size.x)
                tooltipDirection.x = 1;

            if (tooltipDirection.y == -1 && mousePosition.y < screenPadding.y + size.y)
                tooltipDirection.y = 1;

            if (tooltipDirection.x == 1 && mousePosition.x > Screen.width - screenPadding.x - size.x)
                tooltipDirection.x = -1;

            if (tooltipDirection.y == 1 && mousePosition.y > Screen.height - screenPadding.y - size.y)
                tooltipDirection.y = -1;
            // offset.x += spacing;
            // offset.y += spacing;

            tooltip.position = Input.mousePosition + (Vector3) (offset * tooltipDirection);
            // Move tooltip to mouse pos 
        }
    }
}