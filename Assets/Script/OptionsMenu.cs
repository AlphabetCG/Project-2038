using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ใส่ในหน้า Options แล้วผูก:
//   volumeSlider.onValueChanged      -> OptionsMenu.SetVolume
//   sensitivitySlider.onValueChanged -> OptionsMenu.SetSensitivity
//   fullscreenToggle.onValueChanged  -> OptionsMenu.SetFullscreen
//   qualityDropdown.onValueChanged   -> OptionsMenu.SetQuality
// ค่าถูกบันทึกใน PlayerPrefs และนำไปใช้กับผู้เล่นตอนเริ่มเกม (ผ่าน StartingScreen)
public class OptionsMenu : MonoBehaviour
{
    const string VolumeKey = "MasterVolume";
    const string SensitivityKey = "MouseSensitivity";
    const string QualityKey = "QualityLevel";
    public const float DefaultSensitivity = 2f;

    public Slider volumeSlider;      // ค่า 0 - 1
    public Slider sensitivitySlider; // แนะนำค่า 0.5 - 5
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;

    public static float Volume => PlayerPrefs.GetFloat(VolumeKey, 1f);
    public static float Sensitivity => PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void ApplySavedSettings()
    {
        AudioListener.volume = Volume;
        if (PlayerPrefs.HasKey(QualityKey)) QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(QualityKey), true);
    }

    void Start()
    {
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(Volume);
        if (sensitivitySlider != null) sensitivitySlider.SetValueWithoutNotify(Sensitivity);
        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt(QualityKey, index);
    }

    public static void ApplyToPlayer(GameObject player)
    {
        if (player == null) return;
        FirstPersonController fpc = player.GetComponent<FirstPersonController>();
        if (fpc != null) fpc.mouseSensitivity = Sensitivity;
    }
}
