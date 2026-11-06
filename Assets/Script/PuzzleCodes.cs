using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// สุ่มรหัส puzzle ทั้งหมดของฉากตอนโหลดฉาก (เริ่มเกม / Restart) และเก็บไว้ที่เดียว
// โจทย์ (คำใบ้) กับคำตอบสร้างจากค่าเดียวกันเสมอ
//   PuzzleCodes.Get("tavchai_pass")          -> "701310"
//   PuzzleCodes.Format("Code: {tavchai_hex}") -> "Code: AB37E"
[DefaultExecutionOrder(-100)]
public class PuzzleCodes : MonoBehaviour
{
    [Tooltip("0 = สุ่มใหม่ทุกครั้ง, ค่าอื่น = ได้รหัสเดิมทุกครั้ง (ใช้ทดสอบ)")]
    public int fixedSeed = 0;

    private static PuzzleCodes instance;
    private readonly Dictionary<string, string> codes = new Dictionary<string, string>();

    void Awake()
    {
        instance = this;
        var rng = fixedSeed != 0 ? new System.Random(fixedSeed) : new System.Random();

        GenerateTaVChai(rng);
        GenerateDigits(rng, "keypad1", 4); // Keypad ห้อง 1: เลขบนรูปห้องชั้นบน

#if UNITY_EDITOR
        foreach (var kv in codes) Debug.Log("[PuzzleCodes] " + kv.Key + " = " + kv.Value);
#endif
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // โน้ตห้องแรก: เลขฐาน 16 (5 หลัก มีตัวอักษร A-F อย่างน้อย 2 ตัว) -> รหัสคอม TaVChai เป็นฐาน 10
    void GenerateTaVChai(System.Random rng)
    {
        int value;
        string hex;
        do
        {
            value = rng.Next(0x10000, 0x100000);
            hex = value.ToString("X5");
        } while (CountHexLetters(hex) < 2);

        codes["tavchai_hex"] = hex;
        codes["tavchai_pass"] = value.ToString();
    }

    // รหัสตัวเลข n หลัก (ขึ้นต้นด้วย 0 ได้)
    void GenerateDigits(System.Random rng, string key, int length)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++) chars[i] = (char)('0' + rng.Next(10));
        codes[key] = new string(chars);
        codes[key + "_spaced"] = string.Join(" ", chars); // "3 9 0 8" สำหรับคำใบ้ที่เขียนเว้นวรรค
    }

    static int CountHexLetters(string hex)
    {
        int count = 0;
        foreach (char c in hex)
        {
            if (c >= 'A' && c <= 'F') count++;
        }
        return count;
    }

    static PuzzleCodes Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<PuzzleCodes>();
            return instance;
        }
    }

    public static string Get(string key)
    {
        if (Instance != null && Instance.codes.TryGetValue(key, out string value)) return value;
        Debug.LogError("[PuzzleCodes] ไม่พบรหัส '" + key + "' (มี PuzzleCodes อยู่ในฉากไหม?)");
        return null;
    }

    // แทน {key} ในข้อความด้วยรหัสจริง
    public static string Format(string template)
    {
        return Regex.Replace(template, @"\{(\w+)\}", m => Get(m.Groups[1].Value) ?? m.Value);
    }
}
