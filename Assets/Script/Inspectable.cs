using UnityEngine;

// ของในห้องบอสที่ผู้เล่นกด [E] ตรวจได้ ทุกชิ้นใช้ prompt เดียวกัน ไม่บอกว่าชิ้นไหนผิด
// สร้าง trigger ครอบตัวเองอัตโนมัติจากขนาดของ Renderer
public class Inspectable : MonoBehaviour
{
    public float padding = 0.35f;

    public Anomaly Anomaly { get; private set; }

    void Awake()
    {
        Anomaly = GetComponent<Anomaly>();
        AddTrigger();
    }

    void AddTrigger()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;

        // หา bounds ใน local space จากมุมทั้ง 8 ของทุก renderer
        bool first = true;
        Bounds local = new Bounds();
        foreach (var r in renderers)
        {
            Bounds b = r.bounds;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = b.center + Vector3.Scale(b.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                Vector3 p = transform.InverseTransformPoint(corner);
                if (first) { local = new Bounds(p, Vector3.zero); first = false; }
                else local.Encapsulate(p);
            }
        }

        Vector3 s = transform.lossyScale;
        Vector3 pad = new Vector3(padding / Mathf.Max(Mathf.Abs(s.x), 1e-4f), padding / Mathf.Max(Mathf.Abs(s.y), 1e-4f), padding / Mathf.Max(Mathf.Abs(s.z), 1e-4f));
        var box = gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = local.center;
        box.size = local.size + pad * 2;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && BossManager.Instance != null) BossManager.Instance.EnterReach(this);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach") && BossManager.Instance != null) BossManager.Instance.ExitReach(this);
    }

    void OnDisable()
    {
        if (BossManager.Instance != null) BossManager.Instance.ExitReach(this);
    }
}
