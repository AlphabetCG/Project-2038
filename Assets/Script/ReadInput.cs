using System.Collections;
using UnityEngine;
using TMPro;

// หน้า login ของคอม TaVChai: รหัสมาจาก PuzzleCodes (สุ่มทุกเกม)
public class ReadInput : MonoBehaviour
{
    public GameObject player;
    public GameObject hud;
    public GameObject inv;
    public GameObject login_UI;
    public GameObject main_UI;
    public GameObject check_list;
    public GameObject line;
    public GameObject Login;

    public GameObject NotePad_UI;

    public string passwordKey = "tavchai_pass"; // ชื่อรหัสใน PuzzleCodes
    public TMP_InputField passwordField;
    public GameObject wrongText;                // ข้อความ "Wrong password" ไม่บังคับ
    public float wrongDisplayTime = 1f;

    void OnEnable()
    {
        if (wrongText != null) wrongText.SetActive(false);
    }

    // ผูกกับ onEndEdit ของช่องรหัส (กด Enter)
    public void ReadStringInput(string s)
    {
        s = s.Trim();
        if (s.Length == 0) return;

        if (s == PuzzleCodes.Get(passwordKey))
        {
            ChecklistProgress.Complete(ChecklistTask.LoginTaVChai);
            line.SetActive(true);
            login_UI.SetActive(false);
            main_UI.SetActive(true);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowWrong());
        }
    }

    IEnumerator ShowWrong()
    {
        if (passwordField != null)
        {
            passwordField.text = "";
            passwordField.ActivateInputField();
        }
        if (wrongText == null) yield break;
        wrongText.SetActive(true);
        yield return new WaitForSecondsRealtime(wrongDisplayTime);
        wrongText.SetActive(false);
    }

    public void DoNotOpen(){
        NotePad_UI.SetActive(true);
    }
    public void NotePad(){
        NotePad_UI.SetActive(false);
    }
    public void ExitButton_login()
    {
        login_UI.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
    public void ExitButton_main()
    {
        Login.SetActive(false);
        line.SetActive(false);
        check_list.SetActive(true);
        main_UI.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
}
