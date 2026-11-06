using UnityEngine;

// Keypad ของระเบิด: ใส่รหัสถูกแล้วปิด keypad, เปิดประตูห้องน้ำ และเปลี่ยนข้อความภารกิจ
public class BombPad : Keypad
{
    public GameObject Oldtext;
    public GameObject newtext;

    public AudioSource audioSource;

    public GameObject doortoilet;
    public GameObject budisiam;

    protected override void OnCorrect()
    {
        ChecklistProgress.Complete(ChecklistTask.DefuseBomb);
        budisiam.SetActive(true);
        doortoilet.SetActive(true);
        keypadOB.SetActive(false);
        CloseUI();
        audioSource.Stop();
        Oldtext.SetActive(false);
        newtext.SetActive(true);
    }
}
