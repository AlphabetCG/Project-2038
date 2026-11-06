using UnityEngine;
using TMPro;

// ข้อความคำใบ้ที่มีรหัสสุ่ม ใส่ {ชื่อรหัส} ใน template เช่น "{tavchai_hex}."
[RequireComponent(typeof(TMP_Text))]
public class CodeHintText : MonoBehaviour
{
    [TextArea(3, 12)]
    public string template;

    void OnEnable()
    {
        GetComponent<TMP_Text>().text = PuzzleCodes.Format(template);
    }
}
