using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helpers para construir UI uGUI por código, sin depender de assets cableados en el Inspector.
/// Usado por el HUD y la pantalla de resultados.
/// </summary>
public static class UiFactory
{
    static readonly Vector2 Half = new Vector2(0.5f, 0.5f);

    /// <summary>Fuente builtin de Unity, para que el texto renderice sin importar assets.</summary>
    public static Font DefaultFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            // Fallback para versiones anteriores de Unity.
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        return font;
    }

    /// <summary>Sprite sólido de un color, generado en memoria (para paneles, barras, botones).</summary>
    public static Sprite SolidSprite(Color color)
    {
        const int size = 4;
        var texture = new Texture2D(size, size);
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), Half, size);
    }

    public static Canvas CreateCanvas(string name, int sortOrder)
    {
        var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    public static Text CreateText(Transform parent, string content, int fontSize, Color color, TextAnchor anchor)
    {
        var go = new GameObject("Text", typeof(Text));
        go.transform.SetParent(parent, false);

        var text = go.GetComponent<Text>();
        text.font = DefaultFont();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    public static Image CreatePanel(Transform parent, Color color)
    {
        var go = new GameObject("Panel", typeof(Image));
        go.transform.SetParent(parent, false);

        var image = go.GetComponent<Image>();
        image.sprite = SolidSprite(Color.white);
        image.color = color;
        return image;
    }

    public static Button CreateButton(Transform parent, string label, Color background, Color foreground, Action onClick)
    {
        var go = new GameObject("Button", typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var image = go.GetComponent<Image>();
        image.sprite = SolidSprite(Color.white);
        image.color = background;

        var button = go.GetComponent<Button>();
        if (onClick != null)
        {
            button.onClick.AddListener(() => onClick());
        }

        Text caption = CreateText(go.transform, label, 36, foreground, TextAnchor.MiddleCenter);
        RectTransform captionRect = caption.rectTransform;
        captionRect.anchorMin = Vector2.zero;
        captionRect.anchorMax = Vector2.one;
        captionRect.offsetMin = Vector2.zero;
        captionRect.offsetMax = Vector2.zero;
        return button;
    }

    /// <summary>Coloca un Graphic con anclas/pivote/posición/tamaño explícitos.</summary>
    public static RectTransform Place(Graphic graphic, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform rect = graphic.rectTransform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        return rect;
    }

    /// <summary>Estira un Graphic para cubrir todo su contenedor.</summary>
    public static void Stretch(Graphic graphic)
    {
        RectTransform rect = graphic.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
