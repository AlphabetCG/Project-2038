using UnityEngine;
using System.Collections.Generic;

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
    [Tooltip("Separate props that must rotate with this object, such as shelf contents.")]
    public Transform[] rotateTogether = new Transform[0];
    [Tooltip("Anomalies that cannot be selected together because their changed poses overlap.")]
    public Anomaly[] incompatibleWith = new Anomaly[0];

    public bool IsApplied { get; private set; }

    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;
    private struct CompanionState
    {
        public Transform target;
        public Vector3 position;
        public Quaternion rotation;
    }
    private readonly List<CompanionState> companions = new List<CompanionState>();

    public bool ConflictsWith(Anomaly other)
    {
        if (other == this) return true;
        return System.Array.IndexOf(incompatibleWith, other) >= 0
            || System.Array.IndexOf(other.incompatibleWith, this) >= 0
            || ContainsCompanion(other.transform) || other.ContainsCompanion(transform);
    }

    bool ContainsCompanion(Transform candidate)
    {
        foreach (var t in rotateTogether)
            if (t != null && candidate.IsChildOf(t)) return true;
        return false;
    }

    public void Apply()
    {
        if (IsApplied) return;
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;

        switch (kind)
        {
            case Kind.Move: transform.position += moveOffset; break;
            case Kind.Rotate:
                var delta = Quaternion.Euler(rotateEuler);
                transform.SetPositionAndRotation(position + moveOffset, delta * rotation);
                var seen = new HashSet<Transform>();
                foreach (var t in rotateTogether)
                {
                    if (t == null || t.IsChildOf(transform) || !seen.Add(t)) continue;
                    companions.Add(new CompanionState { target = t, position = t.position, rotation = t.rotation });
                    t.SetPositionAndRotation(transform.position + delta * (t.position - position), delta * t.rotation);
                }
                break;
            case Kind.Scale:
                transform.localScale = scale * scaleMultiplier;
                transform.position = position + moveOffset;
                break;
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
        foreach (var state in companions)
            if (state.target != null) state.target.SetPositionAndRotation(state.position, state.rotation);
        companions.Clear();
        IsApplied = false;
    }
}
