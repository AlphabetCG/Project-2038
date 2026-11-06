using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Keypad พื้นฐาน: Keypad_1 และ BombPad สืบทอดจากคลาสนี้
public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypadOB;
    public GameObject hud;
    public GameObject inv;

    public Animator ANI;

    public TextMeshProUGUI textOB;
    public string answer = "12345";
    public string codeKey;      // ถ้าใส่ไว้ คำตอบจะมาจาก PuzzleCodes (สุ่มทุกเกม) แทน answer

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;

    public float wrongDisplayTime = 1f; // เวลาที่แสดงคำว่า "Wrong"

    private bool isOpen;
    private bool showingWrong;

    protected virtual void Start()
    {
        keypadOB.SetActive(false);
    }

    // อ่านตอนกด OK เพราะ Keypad อาจถูกปิดไว้ตั้งแต่เริ่มฉาก (Start ยังไม่ทำงาน)
    string ExpectedAnswer => string.IsNullOrEmpty(codeKey) ? answer : PuzzleCodes.Get(codeKey);

    public void Number(int number)
    {
        if (showingWrong) return;
        if (textOB.text.Length < 5) // Example: Maximum 5 digits
        {
            textOB.text += number.ToString();
            button.Play();
        }
    }

    public void Execute()
    {
        if (showingWrong) return;
        if (textOB.text == ExpectedAnswer)
        {
            correct.Play();
            textOB.text = "Right";
            OnCorrect();
        }
        else
        {
            wrong.Play();
            StartCoroutine(ShowWrong());
        }
    }

    IEnumerator ShowWrong()
    {
        showingWrong = true;
        textOB.text = "Wrong";
        yield return new WaitForSecondsRealtime(wrongDisplayTime);
        textOB.text = "";
        showingWrong = false;
    }

    // ให้คลาสลูกเพิ่มสิ่งที่เกิดขึ้นตอนใส่รหัสถูก
    protected virtual void OnCorrect()
    {
    }

    public void Clear()
    {
        if (showingWrong) return;
        textOB.text = "";
        button.Play();
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        CloseUI();
    }

    protected void CloseUI()
    {
        isOpen = false;
        PlayerUI.Close(player, hud, inv);
    }

    public virtual void Update()
    {
        if (textOB.text == "Right" && animate)
        {
            ANI.SetTrigger("Open"); // ใช้ Trigger แทน Bool
            animate = false; // ป้องกันการรันซ้ำ
        }

        // keypadOB ถูกเปิดจาก OpenKeyPad / Bomb ให้หยุดผู้เล่นแค่ครั้งเดียวตอนเปิด
        if (keypadOB.activeInHierarchy && !isOpen)
        {
            isOpen = true;
            PlayerUI.Open(player, hud, inv);
        }
        else if (!keypadOB.activeInHierarchy && isOpen)
        {
            isOpen = false;
        }
    }
}
