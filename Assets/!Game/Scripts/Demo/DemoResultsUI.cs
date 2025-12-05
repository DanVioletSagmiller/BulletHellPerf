using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Demo))]
public class DemoResultsUI : MonoBehaviour
{
    public Demo demo;
    public RectTransform panel;
    public Font Font;
    [Header("Update")]
    public float refreshInterval = 0.1f;

    [Header("Curve Textures")]
    public int curveWidth = 280;
    public int curveHeight = 40;
    public Color backgroundColor = Color.black;
    public Color cpuColor = Color.green;
    public Color gpuColor = Color.cyan;



    class Entry
    {
        public Demo.Results results;
        public GameObject root;
        public Text title;
        public Text cpuLabel;
        public Text gpuLabel;
        public RawImage cpuImage;
        public RawImage gpuImage;
    }

    readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>();
    float timer;
    private Color[] pixelMemory;

    void Awake()
    {
        if (demo == null)
            demo = GetComponent<Demo>();

        if (panel == null && demo != null)
            panel = demo.Panel;
    }

    void Start()
    {
        if (panel == null) return;

        // Make the panel behave like an inspector-style vertical list.
        var layout = panel.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
            layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();

        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.spacing = 8f;
        layout.padding = new RectOffset(4, 4, 4, 4);
    }

    void Update()
    {
        if (panel == null || demo == null) return;

        timer += Time.unscaledDeltaTime;
        if (timer < refreshInterval) return;
        timer = 0f;

        RefreshEntries();
        UpdateEntries();
    }

    void RefreshEntries()
    {
        var dict = demo.AllResultsReadOnly;
        if (dict == null) return;

        // Add new entries
        foreach (var kv in dict)
        {
            if (!entries.ContainsKey(kv.Key))
                entries[kv.Key] = CreateEntry(kv.Value);
        }

        // Remove entries that no longer exist
        var toRemove = new List<string>();
        foreach (var key in entries.Keys)
        {
            if (!dict.ContainsKey(key))
                toRemove.Add(key);
        }

        foreach (var key in toRemove)
        {
            if (entries[key].root != null)
                Destroy(entries[key].root);
            entries.Remove(key);
        }
    }

    Entry CreateEntry(Demo.Results res)
    {
        var rootGO = new GameObject("Result_" + res.Name, typeof(RectTransform));
        var rootRect = rootGO.GetComponent<RectTransform>();
        rootRect.SetParent(panel, false);
        rootRect.anchorMin = new Vector2(0, 1);
        rootRect.anchorMax = new Vector2(1, 1);
        rootRect.pivot = new Vector2(0.5f, 1f);
        rootRect.sizeDelta = new Vector2(0, 120);

        // Subtle background so groups are separated
        var bgImage = rootGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.25f);

        var vLayout = rootGO.AddComponent<VerticalLayoutGroup>();
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = true;
        vLayout.childForceExpandWidth = true;
        vLayout.childForceExpandHeight = false;
        vLayout.spacing = 2f;
        vLayout.padding = new RectOffset(4, 4, 4, 4);

        // Title
        var titleGO = CreateText("Title", rootRect, 14, FontStyle.Bold, TextAnchor.MiddleLeft);

        // CPU label + curve
        var cpuLabelGO = CreateText("CPU_Label", rootRect, 12, FontStyle.Normal, TextAnchor.MiddleLeft);
        var cpuCurveGO = CreateCurveImage("CPU_Curve", rootRect);

        // GPU label + curve
        var gpuLabelGO = CreateText("GPU_Label", rootRect, 12, FontStyle.Normal, TextAnchor.MiddleLeft);
        var gpuCurveGO = CreateCurveImage("GPU_Curve", rootRect);

        var entry = new Entry
        {
            results = res,
            root = rootGO,
            title = titleGO.GetComponent<Text>(),
            cpuLabel = cpuLabelGO.GetComponent<Text>(),
            gpuLabel = gpuLabelGO.GetComponent<Text>(),
            cpuImage = cpuCurveGO.GetComponent<RawImage>(),
            gpuImage = gpuCurveGO.GetComponent<RawImage>()
        };

        return entry;
    }

    GameObject CreateText(string name, RectTransform parent, int size, FontStyle style, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);

        var text = go.AddComponent<Text>();
        text.font = this.Font;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = align;
        text.color = Color.white;

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = size + 6;

        return go;
    }

    GameObject CreateCurveImage(string name, RectTransform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);

        var img = go.AddComponent<RawImage>();
        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = curveHeight;
        layout.preferredWidth = curveWidth;

        return go;
    }

    

    void UpdateEntries()
    {
        if (pixelMemory == null || pixelMemory.Length != curveWidth * curveHeight)
            pixelMemory = new Color[curveWidth * curveHeight];
        foreach (var kv in entries)
        {
            var e = kv.Value;
            var r = e.results;
            if (r == null) continue;

            r.AverageCPU = AverageOfCurve(r.CPU_Curve);
            r.AverageGPU = AverageOfCurve(r.GPU_Curve);

            e.title.text = $"{r.Name}  (Peak Objects: {r.PeekObjects})";
            e.cpuLabel.text = $"CPU Avg: {r.AverageCPU:0.00} ms";
            e.gpuLabel.text = $"GPU Avg: {r.AverageGPU:0.00} ms";

            if (r.CPU_Curve != null)
                e.cpuImage.texture = CurveTextureUtil.CurveToTexture(
                    r.CPU_Curve,
                    curveWidth,
                    curveHeight,
                    backgroundColor,
                    cpuColor,
                    tex: e.cpuImage.texture as Texture2D,
                    pixels: pixelMemory);

            if (r.GPU_Curve != null)
                e.gpuImage.texture = CurveTextureUtil.CurveToTexture(
                    r.GPU_Curve,
                    curveWidth,
                    curveHeight,
                    backgroundColor,
                    gpuColor,
                    tex: e.gpuImage.texture as Texture2D,
                    pixels: pixelMemory);
        }
    }

    public static float AverageOfCurve(AnimationCurve curve)
    {
        if (curve == null || curve.keys == null || curve.keys.Length == 0)
            return 0f;

        float sum = 0f;
        var keys = curve.keys;

        for (int i = 0; i < keys.Length; i++)
            sum += keys[i].value;

        return sum / keys.Length;
    }
}
