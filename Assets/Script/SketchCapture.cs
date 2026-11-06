using UnityEngine;

// ถ่ายภาพจากมุมกล้องที่กำหนด แล้วแปลงเป็นภาพร่างดินสอบนกระดาษ
// ตั้งใจให้ดูยากนิดหน่อย: ใช้เฉพาะเส้นขอบ เส้นขาดบางช่วง สั่นเล็กน้อย และแรเงาเฉพาะส่วนมืด
public static class SketchCapture
{
    static readonly Color Paper = new Color(0.93f, 0.90f, 0.82f);
    static readonly Color Ink = new Color(0.22f, 0.20f, 0.19f);

    public static Texture2D Capture(Transform viewpoint, float fov, int width, int height, System.Random rng)
    {
        float[] lum = RenderLuminance(viewpoint, fov, width, height);
        AutoLevels(lum);
        lum = Blur(lum, width, height);
        float[] edge = Sobel(lum, width, height);

        // ค่า noise ความถี่ต่ำ ใช้ทำเส้นขาดเป็นช่วงๆ และให้เส้นสั่น
        float ox = (float)rng.NextDouble() * 1000f, oy = (float)rng.NextDouble() * 1000f;
        var px = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // สั่นตำแหน่งที่อ่านเส้นขอบ ±1.5 พิกเซล
                int jx = Mathf.Clamp(x + Mathf.RoundToInt((Mathf.PerlinNoise(ox + x * 0.05f, oy + y * 0.05f) - 0.5f) * 3f), 0, width - 1);
                int jy = Mathf.Clamp(y + Mathf.RoundToInt((Mathf.PerlinNoise(oy + x * 0.05f, ox + y * 0.05f) - 0.5f) * 3f), 0, height - 1);
                float e = edge[jy * width + jx];

                // เส้นขาดในบางช่วง
                float gap = Mathf.PerlinNoise(ox + x * 0.02f, oy + y * 0.02f);
                if (gap < 0.32f) e *= 0.15f;

                // แรเงาเส้นทแยงเฉพาะส่วนที่มืดมาก
                float tone = 1f - lum[y * width + x];
                float hatch = 0f;
                if (tone > 0.88f && ((x + y) % 9) == 0) hatch = 0.3f;

                float ink = Mathf.Clamp01(e + hatch);
                float grain = ((float)rng.NextDouble() - 0.5f) * 0.05f;
                Color c = Color.Lerp(Paper, Ink, ink);
                c.r += grain; c.g += grain; c.b += grain;

                // ขอบภาพมืดลงเล็กน้อยเหมือนกระดาษเก่า
                float dx = (x / (float)width - 0.5f) * 2f, dy = (y / (float)height - 0.5f) * 2f;
                float vignette = 1f - 0.18f * Mathf.Clamp01(dx * dx + dy * dy - 0.4f);
                c *= vignette;
                c.a = 1f;
                px[y * width + x] = c;
            }
        }

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, true);
        tex.SetPixels(px);
        tex.Apply(true);
        return tex;
    }

    static float[] RenderLuminance(Transform viewpoint, float fov, int width, int height)
    {
        var rt = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var go = new GameObject("SketchCapture.Camera") { hideFlags = HideFlags.HideAndDontSave };
        var cam = go.AddComponent<Camera>();
        cam.transform.SetPositionAndRotation(viewpoint.position, viewpoint.rotation);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 60f;
        cam.cullingMask = ~((1 << 5) | (1 << 31)); // ไม่เอา UI และ layer ชั่วคราวของ CodeTexture
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.targetTexture = rt;
        cam.Render();
        cam.targetTexture = null;
        if (Application.isPlaying) Object.Destroy(go); else Object.DestroyImmediate(go);

        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var read = new Texture2D(width, height, TextureFormat.RGBA32, false);
        read.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        read.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        Color[] c = read.GetPixels();
        if (Application.isPlaying) Object.Destroy(read); else Object.DestroyImmediate(read);
        var lum = new float[c.Length];
        for (int i = 0; i < c.Length; i++) lum[i] = c[i].r * 0.299f + c[i].g * 0.587f + c[i].b * 0.114f;
        return lum;
    }

    // ยืด contrast ให้ห้องที่มืดยังเห็นรูปทรง (ตัด 2% มืดสุด/สว่างสุด)
    static void AutoLevels(float[] lum)
    {
        var sorted = (float[])lum.Clone();
        System.Array.Sort(sorted);
        float lo = sorted[(int)(sorted.Length * 0.02f)], hi = sorted[(int)(sorted.Length * 0.98f)];
        float range = Mathf.Max(hi - lo, 0.01f);
        for (int i = 0; i < lum.Length; i++) lum[i] = Mathf.Clamp01((lum[i] - lo) / range);
    }

    static float[] Blur(float[] src, int w, int h)
    {
        var dst = new float[src.Length];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float sum = 0; int n = 0;
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int xx = x + dx, yy = y + dy;
                        if (xx < 0 || yy < 0 || xx >= w || yy >= h) continue;
                        sum += src[yy * w + xx]; n++;
                    }
                dst[y * w + x] = sum / n;
            }
        return dst;
    }

    // ความแรงของเส้นขอบ 0-1 (normalize ด้วย percentile 96 แล้วตัดเส้นจางทิ้ง)
    static float[] Sobel(float[] l, int w, int h)
    {
        var mag = new float[l.Length];
        for (int y = 1; y < h - 1; y++)
            for (int x = 1; x < w - 1; x++)
            {
                float gx = -l[(y - 1) * w + x - 1] - 2 * l[y * w + x - 1] - l[(y + 1) * w + x - 1]
                           + l[(y - 1) * w + x + 1] + 2 * l[y * w + x + 1] + l[(y + 1) * w + x + 1];
                float gy = -l[(y - 1) * w + x - 1] - 2 * l[(y - 1) * w + x] - l[(y - 1) * w + x + 1]
                           + l[(y + 1) * w + x - 1] + 2 * l[(y + 1) * w + x] + l[(y + 1) * w + x + 1];
                mag[y * w + x] = Mathf.Sqrt(gx * gx + gy * gy);
            }
        var sorted = (float[])mag.Clone();
        System.Array.Sort(sorted);
        float top = Mathf.Max(sorted[(int)(sorted.Length * 0.96f)], 0.0001f);
        for (int i = 0; i < mag.Length; i++) mag[i] = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((mag[i] / top - 0.35f) / 0.5f));
        return mag;
    }
}
