using UnityEngine;
using TMPro;

// วาดรหัสสุ่มลงบนรูป (texture) ของวัตถุ 3D ตอนเริ่มเกม ตัวเลขจึงรับแสงเหมือนรูปเดิม
// ใน Editor วัตถุยังแสดงรูปเดิม พอกด Play จะเปลี่ยนเป็น background + รหัสจาก PuzzleCodes
[RequireComponent(typeof(Renderer))]
public class CodeTexture : MonoBehaviour
{
    public Texture2D background;           // รูปที่ลบตัวเลขออกแล้ว
    public string template = "{keypad1}";  // ใช้ {ชื่อรหัส} แบบเดียวกับ CodeHintText
    public TMP_FontAsset font;
    public Color color = Color.black;
    public float characterSpacing = 0;
    public FontStyles fontStyle = FontStyles.Normal;

    [Header("ตำแหน่งบนรูป (0-1, มุมซ้ายล่าง = 0,0)")]
    public Vector2 center = new Vector2(0.5f, 0.5f);
    public Vector2 maxSize = new Vector2(0.7f, 0.2f); // ขนาดสูงสุดของข้อความ เทียบกับขนาดรูป
    [Tooltip("สัดส่วนกว้าง/สูงของรูปตอนแสดงบนวัตถุ (0 = ใช้ขนาด texture) ใช้เมื่อ texture ถูก import เป็นขนาดอื่น เช่น 2048x1024")]
    public float displayAspect = 0;

    private Texture2D result;

    void Start()
    {
        GetComponent<Renderer>().material.mainTexture = Render(PuzzleCodes.Format(template));
    }

    // layer ที่ใช้ตอนวาด (ต้องไม่มีวัตถุอื่นในฉากใช้)
    const int RenderLayer = 31;

    public Texture2D Render(string text)
    {
        int w = background.width, h = background.height;

        // 1) วาดข้อความบนพื้นขาวลง RenderTexture ด้วยกล้อง orthographic ชั่วคราว (ไกลจากฉากจริง)
        var textRT = RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.ARGB32);
        float viewH = 10f, viewW = viewH * w / h;
        var root = new GameObject("CodeTexture.Render") { hideFlags = HideFlags.HideAndDontSave };
        root.transform.position = new Vector3(0, -10000, 0);

        var textGo = new GameObject("Text");
        textGo.layer = RenderLayer;
        textGo.transform.SetParent(root.transform, false);
        var tmp = textGo.AddComponent<TextMeshPro>();
        tmp.font = font;
        tmp.fontSize = 10;
        tmp.color = color;
        tmp.characterSpacing = characterSpacing;
        tmp.fontStyle = fontStyle;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.text = text;
        tmp.ForceMeshUpdate();

        // ปรับขนาดข้อความให้พอดีกรอบ maxSize แล้ววางที่ center
        // ถ้า texture ถูกยืด (displayAspect) ย่อ/ขยายแกน y ชดเชย ตัวอักษรจะไม่เพี้ยนตอนแสดงจริง
        Bounds b = tmp.textBounds;
        float yFactor = displayAspect > 0 ? displayAspect * h / w : 1f;
        float scale = Mathf.Min(maxSize.x * viewW / b.size.x, maxSize.y * viewH / (b.size.y * yFactor));
        Vector3 scaleVec = new Vector3(scale, scale * yFactor, 1);
        textGo.transform.localScale = scaleVec;
        textGo.transform.localPosition = new Vector3((center.x - 0.5f) * viewW, (center.y - 0.5f) * viewH, 0) - Vector3.Scale(b.center, scaleVec);

        var cam = new GameObject("Camera").AddComponent<Camera>();
        cam.transform.SetParent(root.transform, false);
        cam.transform.localPosition = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = viewH / 2f;
        cam.cullingMask = 1 << RenderLayer;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.white;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 20f;
        cam.targetTexture = textRT;
        cam.Render();
        cam.targetTexture = null;
        DestroyNow(root);

        // 2) อ่านพื้นหลังกับข้อความกลับมา แล้วผสมกัน: ยิ่งพิกเซลใกล้สีหมึก ยิ่งใช้สีหมึก
        var bgRT = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(background, bgRT);
        Color[] bgPx = ReadPixels(bgRT, w, h);
        Color[] textPx = ReadPixels(textRT, w, h);
        RenderTexture.ReleaseTemporary(bgRT);
        RenderTexture.ReleaseTemporary(textRT);

        float inkDistance = Mathf.Max(0.001f, (1 - color.r) + (1 - color.g) + (1 - color.b));
        for (int i = 0; i < bgPx.Length; i++)
        {
            Color t = textPx[i];
            float amount = Mathf.Clamp01(((1 - t.r) + (1 - t.g) + (1 - t.b)) / inkDistance);
            if (amount <= 0) continue;
            Color ink = new Color(color.r, color.g, color.b, 1);
            bgPx[i] = Color.Lerp(bgPx[i], ink, amount);
        }

        if (result == null) result = new Texture2D(w, h, TextureFormat.RGBA32, true);
        result.SetPixels(bgPx);
        result.Apply(true);
        return result;
    }

    static Color[] ReadPixels(RenderTexture rt, int w, int h)
    {
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;
        Color[] px = tex.GetPixels();
        DestroyNow(tex);
        return px;
    }

    static void DestroyNow(Object o)
    {
        if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
    }

    void OnDestroy()
    {
        if (result != null) DestroyNow(result);
    }
}
