using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashlightAdvanced : MonoBehaviour
{
    public Light light;
    public TMP_Text text;

    public TMP_Text batteryText;

    public float lifetime = 100;

    public float batteries = 0;

    public AudioSource flashON;
    public AudioSource flashOFF;

    private bool on;

    // ใช้เช็คว่าค่าเปลี่ยนก่อนอัปเดตข้อความ
    private int shownLifetime = -1;
    private float shownBatteries = -1;


    void Start()
    {
        light = GetComponent<Light>();

        on = false;
        light.enabled = false;

    }


    void Update()
    {
        // ไม่รับ input ตอน pause / เปิด UI / เกมจบ
        if (GameFlow.IsInGameplay())
        {
            if (Input.GetButtonDown("flashlight"))
            {
                if (on)
                {
                    flashOFF.Play();
                    SetOn(false);
                }
                else if (lifetime > 0)
                {
                    flashON.Play();
                    SetOn(true);
                }
            }

            if (Input.GetButtonDown("reload") && batteries >= 1)
            {
                batteries -= 1;
                lifetime = Mathf.Min(lifetime + 50, 100);
            }
        }

        if (on)
        {
            lifetime -= 1 * Time.deltaTime;
            if (lifetime <= 0)
            {
                lifetime = 0;
                SetOn(false);
            }
        }

        UpdateText();
    }

    void SetOn(bool value)
    {
        on = value;
        light.enabled = value;
    }

    void UpdateText()
    {
        int roundedLifetime = Mathf.RoundToInt(lifetime);
        if (roundedLifetime != shownLifetime)
        {
            shownLifetime = roundedLifetime;
            text.text = roundedLifetime + "%";
        }
        if (batteries != shownBatteries)
        {
            shownBatteries = batteries;
            batteryText.text = batteries.ToString();
        }
    }
}
