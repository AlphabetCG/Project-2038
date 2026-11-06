using UnityEngine;

// "จุดผิด" ที่ BossManager สุ่มเลือกได้ ใส่บนของในห้องบอส
//   Move   = เลื่อนตำแหน่ง
//   Rotate = หมุน
//   Scale  = ขนาดเปลี่ยน
//   Extra  = ของเกินที่ปกติซ่อนอยู่ (เปิดเมื่อถูกเลือก)
public class Anomaly : MonoBehaviour
{
    public enum Kind { Move, Rotate, Scale, Extra }

    public Kind kind = Kind.Move;
    public Vector3 moveOffset = new Vector3(0.6f, 0, 0); // world space
    public Vector3 rotateEuler = new Vector3(0, 90, 0);
    public float scaleMultiplier = 1.5f;

    public bool IsApplied { get; private set; }

    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    public void Apply()
    {
        if (IsApplied) return;
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;

        switch (kind)
        {
            case Kind.Move: transform.position += moveOffset; break;
            case Kind.Rotate: transform.rotation = Quaternion.Euler(rotateEuler) * rotation; break;
            case Kind.Scale: transform.localScale = scale * scaleMultiplier; break;
            case Kind.Extra: gameObject.SetActive(true); break;
        }
        IsApplied = true;
    }

    public void Revert()
    {
        if (!IsApplied) return;
        if (kind == Kind.Extra)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;
        }
        IsApplied = false;
    }
}
