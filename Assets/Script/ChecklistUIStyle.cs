using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Shared construction for the checklist and the pre-game mode selector.
internal static class ChecklistUIStyle
{
    public static readonly Color Ink = new Color(.055f, .07f, .09f, .98f);
    public static readonly Color Panel = new Color(.1f, .13f, .16f);
    public static readonly Color Accent = new Color(1f, .77f, .39f);
    public static readonly Color Muted = new Color(.68f, .75f, .8f);

    public static RectTransform Box(Transform parent, string name, Vector2 min, Vector2 max, Color? color = null)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min; rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        if (color.HasValue) go.AddComponent<Image>().color = color.Value;
        return rect;
    }

    public static RectTransform Canvas(Transform parent, string name)
    {
        var rect = Box(parent, name, Vector2.zero, Vector2.one);
        var canvas = rect.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        var scaler = rect.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = .5f;
        rect.gameObject.AddComponent<GraphicRaycaster>();
        return rect;
    }

    public static TMP_Text Text(Transform parent, string name, string value, float size, Vector2 min, Vector2 max, Color? color = null)
    {
        var rect = Box(parent, name, min, max);
        var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value; text.fontSize = size;
        text.color = color ?? Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        return text;
    }

    public static Button Button(Transform parent, string name, string label, Vector2 min, Vector2 max, UnityAction action)
    {
        var rect = Box(parent, name, min, max, Panel);
        var button = rect.gameObject.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(.8f, .85f, .9f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        button.onClick.AddListener(action);
        var text = Text(rect, "Label", label, 22, new Vector2(.04f, .04f), new Vector2(.96f, .96f));
        text.alignment = TextAlignmentOptions.Center;
        return button;
    }
}
