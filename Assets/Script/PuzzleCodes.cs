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
        GeneratePrefixPuzzle(rng, "keypad2"); // Keypad ห้อง 2: รหัสที่หายไป 1 หลัก = ผลของ prefix expression
        GenerateCombination(rng, "bomb");     // ระเบิด: รหัส = C(n,k)

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

    // โจทย์รูปแบบเดิม "+ - * a b c / pow d e f" = (a*b - c) + (d^e / f) ได้คำตอบเป็นเลขหลักเดียว
    // รหัส 4 หลักสุ่ม แล้วซ่อน 1 หลัก (ตำแหน่งสุ่ม) ให้ผู้เล่นหาจากโจทย์
    //   key        = รหัสเต็ม เช่น "1038"
    //   key_expr   = โจทย์ เช่น "+ - * 2 3 5 / pow 2 3 4"
    //   key_hint   = รหัสที่ซ่อน 1 หลัก เช่น "1 0 _ 8"
    void GeneratePrefixPuzzle(System.Random rng, string key)
    {
        int a, b, c, d, e, f, answer;
        while (true)
        {
            d = rng.Next(2, 4);
            e = rng.Next(2, 4);
            int power = (int)System.Math.Pow(d, e); // 4, 8, 9, 27
            var divisors = new List<int>();
            for (int i = 2; i <= 9; i++) if (power % i == 0) divisors.Add(i);
            f = divisors[rng.Next(divisors.Count)];
            a = rng.Next(2, 10);
            b = rng.Next(2, 10);
            c = rng.Next(1, 10);
            answer = a * b - c + power / f;
            if (answer >= 0 && answer <= 9) break;
        }

        var digits = new char[4];
        for (int i = 0; i < 4; i++) digits[i] = (char)('0' + rng.Next(10));
        int hidden = rng.Next(4);
        digits[hidden] = (char)('0' + answer);

        var hint = new string[4];
        for (int i = 0; i < 4; i++) hint[i] = i == hidden ? "_" : digits[i].ToString();

        codes[key] = new string(digits);
        codes[key + "_expr"] = "+ - * " + a + " " + b + " " + c + " / pow " + d + " " + e + " " + f;
        codes[key + "_hint"] = string.Join(" ", hint);
    }

    // รหัส = C(n,k) ที่เป็นเลข 4 หลัก (n ไม่เกิน 20, 2 <= k <= n/2 เพื่อไม่ให้ง่ายเกินและไม่ซ้ำกับ C(n,n-k))
    //   key = รหัส เช่น "3432", key_n = "14", key_k = "7"
    void GenerateCombination(System.Random rng, string key)
    {
        var options = new List<Vector2Int>();
        for (int n = 4; n <= 20; n++)
            for (int k = 2; k <= n / 2; k++)
            {
                long c = Combination(n, k);
                if (c >= 1000 && c <= 9999) options.Add(new Vector2Int(n, k));
            }
        var pick = options[rng.Next(options.Count)];
        codes[key] = Combination(pick.x, pick.y).ToString();
        codes[key + "_n"] = pick.x.ToString();
        codes[key + "_k"] = pick.y.ToString();
    }

    static long Combination(int n, int k)
    {
        long result = 1;
        for (int i = 1; i <= k; i++) result = result * (n - k + i) / i;
        return result;
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

    // ตัวสุ่มแยกตามชื่อ (ใช้ fixedSeed เดียวกันถ้าตั้งไว้) สำหรับระบบอื่นที่ต้องสุ่ม เช่น BossManager
    public static System.Random CreateRandom(string salt)
    {
        int hash = 17;
        foreach (char c in salt) hash = hash * 31 + c;
        int seed = Instance != null && Instance.fixedSeed != 0 ? Instance.fixedSeed : System.Environment.TickCount;
        return new System.Random(seed ^ hash);
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
