using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour {
    private static bool showing = false;
    private static TooltipUI instance;

    [SerializeField] private RectTransform tooltip;
    [SerializeField] private TMP_Text tooltipTitleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Vector2 screenPadding;
    [SerializeField] private float spacing;
    private Vector2 tooltipDirection = Vector2.one;

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
}