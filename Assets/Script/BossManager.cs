using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// บอส "หาจุดผิด": สุ่ม Anomaly จากของในห้อง ผู้เล่นเทียบกับภาพร่างหน้าห้องแล้วกด [E] ตรวจ
//   ตรวจถูก = หัวใจบอสแตก 1 ดวง (ของกลับเป็นปกติ)
//   ตรวจผิด = Strike, ครบ maxStrikes = แพ้
// เริ่มเมื่อ startTrigger ถูกเปิด (เก็บแฟลชไดรฟ์สุดท้ายแล้ว)
public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [Header("เริ่มบอส")]
    public GameObject startTrigger;          // Inventory/Endgame
    public Inspectable[] inspectables;       // ของในห้องที่ตรวจได้ทั้งหมด
    public int anomalyCount = 5;
    public int maxStrikes = 3;
    [Range(1, 45)] public float inspectAngle = 25f;

    [Header("UI")]
    public GameObject bossCanvas;            // canvas หัวใจบอส
    public GameObject[] hearts;              // ดับทีละดวงเมื่อตรวจถูก
    public GameObject inspectPrompt;         // "[E] Inspect"
    public TMP_Text strikeText;              // แสดง ✕ ตามจำนวน Strike
    public GameObject wrongFlash;            // แฟลชแดงตอนตรวจผิด (ไม่บังคับ)

    [Header("เสียง")]
    public AudioSource bossMusic;
    public AudioSource correctSound;
    public AudioSource wrongSound;

    [Header("จบ")]
    public GameObject winVideo1;             // FirstPersonController/Video_END
    public GameObject winVideo2;             // VideoPlayer_END (WinScreen ขึ้นเมื่อคัตซีนจบ)
    public GameObject loseScreen;            // EndScreen
    public TMP_Text loseText;
    [TextArea] public string loseMessage = "Game Over\nToo many mistakes";

    [Header("ภาพร่างอ้างอิง (ถ่ายห้องตอนปกติ)")]
    public Transform sketchViewpoint;        // ตำแหน่ง/ทิศกล้องที่ใช้ถ่าย
    public float sketchFov = 70f;
    public Renderer sketchFrame;             // รูปติดผนังหน้าห้อง
    public int sketchWidth = 1024;
    public int sketchHeight = 640;
    [Range(0, 1)] public float sketchGlow = 0.35f;

    public bool IsActive { get; private set; }
    public int Found { get; private set; }
    public int Strikes { get; private set; }

    private readonly List<Inspectable> inReach = new List<Inspectable>();
    private readonly HashSet<Inspectable> checkedItems = new HashSet<Inspectable>();
    private readonly List<Anomaly> active = new List<Anomaly>();
    private bool finished;
    private bool beginPending;
    private Texture2D sketch;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (sketch != null) Destroy(sketch);
    }

    void Start()
    {
        if (inspectPrompt != null) inspectPrompt.SetActive(false);
        if (bossCanvas != null) bossCanvas.SetActive(false);
        if (wrongFlash != null) wrongFlash.SetActive(false);
        UpdateStrikeText();

        // ถ่ายภาพร่างตอนห้องยังปกติ (ก่อนสุ่มจุดผิด)
        if (sketchViewpoint != null && sketchFrame != null)
        {
            sketch = SketchCapture.Capture(sketchViewpoint, sketchFov, sketchWidth, sketchHeight, PuzzleCodes.CreateRandom("boss-sketch"));
            var mat = sketchFrame.material;
            mat.mainTexture = sketch;
            // ทางเดินมืด: ให้กระดาษเรืองแสงอ่อนๆ พอมองเห็นโดยไม่ต้องส่องไฟ
            mat.SetTexture("_EmissionMap", sketch);
            mat.SetColor("_EmissionColor", Color.white * sketchGlow);
            mat.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if (GameFlow.GameEnded)
        {
            if (IsActive) Finish();
            return;
        }
        if (!IsActive && !finished && !beginPending && startTrigger != null && startTrigger.activeInHierarchy)
            StartCoroutine(BeginAfterCutscene());
        if (!IsActive) return;

        Inspectable target = CurrentTarget();
        if (inspectPrompt != null) inspectPrompt.SetActive(target != null && GameFlow.IsInGameplay());
        if (target != null && GameFlow.InteractPressed()) Inspect(target);
    }

    public void Begin()
    {
        if (IsActive || finished || GameFlow.GameEnded || StartingScreen.IsPlaying) return;
        IsActive = true;

        // สุ่มเลือกจุดผิด
        var rng = PuzzleCodes.CreateRandom("boss");
        var pool = new List<Anomaly>();
        foreach (var i in inspectables)
        {
            var a = i != null ? i.GetComponent<Anomaly>() : null;
            if (a != null) pool.Add(a);
        }
        for (int n = 0; n < anomalyCount && pool.Count > 0; n++)
        {
            int index = rng.Next(pool.Count);
            pool[index].Apply();
            active.Add(pool[index]);
            pool.RemoveAt(index);
            pool.RemoveAll(a => a.ConflictsWith(active[active.Count - 1]));
        }

        if (bossCanvas != null) bossCanvas.SetActive(true);
        foreach (var h in hearts) if (h != null) h.SetActive(true);
        UpdateStrikeText();
        if (bossMusic != null) bossMusic.Play();
#if UNITY_EDITOR
        foreach (var a in active) Debug.Log("[BossManager] anomaly: " + a.name + " (" + a.kind + ")");
#endif
    }

    IEnumerator BeginAfterCutscene()
    {
        beginPending = true;
        // Inventory activation starts its cutscene in Start on the following frame.
        yield return null;
        while (StartingScreen.IsPlaying && !GameFlow.GameEnded) yield return null;
        beginPending = false;
        if (startTrigger != null && startTrigger.activeInHierarchy) Begin();
    }

    public void EnterReach(Inspectable i)
    {
        if (!inReach.Contains(i)) inReach.Add(i);
    }

    public void ExitReach(Inspectable i)
    {
        inReach.Remove(i);
    }

    // เลือกชิ้นที่อยู่ใกล้กึ่งกลางสายตาที่สุด
    Inspectable CurrentTarget()
    {
        inReach.RemoveAll(i => i == null || !i.isActiveAndEnabled);
        if (inReach.Count == 0) return null;
        Camera cam = Camera.main;
        if (cam == null) return null;

        Inspectable best = null;
        float bestDot = Mathf.Cos(inspectAngle * Mathf.Deg2Rad);
        foreach (var i in inReach)
        {
            if (checkedItems.Contains(i)) continue;
            Renderer r = i.GetComponentInChildren<Renderer>();
            Vector3 p = r != null ? r.bounds.center : i.transform.position;
            float dot = Vector3.Dot(cam.transform.forward, (p - cam.transform.position).normalized);
            if (dot > bestDot && HasLineOfSight(cam, i, p)) { bestDot = dot; best = i; }
        }
        return best;
    }

    static bool HasLineOfSight(Camera cam, Inspectable target, Vector3 point)
    {
        Vector3 direction = point - cam.transform.position;
        float distance = direction.magnitude;
        foreach (var hit in Physics.RaycastAll(cam.transform.position, direction.normalized, distance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.IsChildOf(target.transform) || hit.transform.IsChildOf(cam.transform.root)) continue;
            if (hit.distance < distance - 0.02f) return false;
        }
        return true;
    }

    void Inspect(Inspectable target)
    {
        if (!IsActive || !GameFlow.IsInGameplay() || target == null || checkedItems.Contains(target)) return;
        checkedItems.Add(target);
        Anomaly a = target.Anomaly;
        if (a != null && active.Contains(a))
        {
            active.Remove(a);
            a.Revert();
            Found++;
            if (correctSound != null) correctSound.Play();
            for (int h = 0; h < hearts.Length; h++) if (hearts[h] != null) hearts[h].SetActive(h >= Found);
            if (active.Count == 0) Win();
        }
        else
        {
            // ตรวจของปกติ = Strike (ชิ้นเดิมนับครั้งเดียว)
            Strikes++;
            if (wrongSound != null) wrongSound.Play();
            if (wrongFlash != null) StartCoroutine(Flash());
            UpdateStrikeText();
            if (Strikes >= maxStrikes) Lose();
        }
    }

    IEnumerator Flash()
    {
        wrongFlash.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        wrongFlash.SetActive(false);
    }

    void UpdateStrikeText()
    {
        if (strikeText == null) return;
        var s = new System.Text.StringBuilder();
        for (int i = 0; i < maxStrikes; i++) s.Append(i < Strikes ? "X " : "- ");
        strikeText.text = "Mistakes  " + s.ToString().TrimEnd();
    }

    void Win()
    {
        ChecklistProgress.Complete(ChecklistTask.DefeatBoss);
        Finish();
        if (!GameFlow.EndGame()) return;
        StartingScreen.Play(winVideo2, winVideo1);
    }

    void Lose()
    {
        Finish();
        if (!GameFlow.EndGame()) return;
        if (loseText != null) loseText.text = loseMessage;
        if (loseScreen != null) loseScreen.SetActive(true);
    }

    void Finish()
    {
        IsActive = false;
        finished = true;
        if (inspectPrompt != null) inspectPrompt.SetActive(false);
        StopAllCoroutines();
        if (wrongFlash != null) wrongFlash.SetActive(false);
        if (bossMusic != null) bossMusic.Stop();
    }
}
