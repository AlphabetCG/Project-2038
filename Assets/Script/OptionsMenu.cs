using UnityEngine;
using UnityEngine.UI;

// ใส่ใน OptionsCanvas แล้วผูก Slider:
//   volumeSlider.onValueChanged      -> OptionsMenu.SetVolume
//   sensitivitySlider.onValueChanged -> OptionsMenu.SetSensitivity
// ค่าถูกบันทึกใน PlayerPrefs และนำไปใช้กับผู้เล่นตอนเริ่มเกม (ผ่าน StartingScreen)
public class OptionsMenu : MonoBehaviour
{
    const string VolumeKey = "MasterVolume";
    const string SensitivityKey = "MouseSensitivity";
    public const float DefaultSensitivity = 2f;

    public Slider volumeSlider;      // ค่า 0 - 1
    public Slider sensitivitySlider; // แนะนำค่า 0.5 - 5

    public static float Volume => PlayerPrefs.GetFloat(VolumeKey, 1f);
    public static float Sensitivity => PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void ApplySavedVolume()
    {
        AudioListener.volume = Volume;
    }

    void Start()
    {
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(Volume);
        if (sensitivitySlider != null) sensitivitySlider.SetValueWithoutNotify(Sensitivity);
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

    public static void ApplyToPlayer(GameObject player)
    {
        if (player == null) return;
        FirstPersonController fpc = player.GetComponent<FirstPersonController>();
        if (fpc != null) fpc.mouseSensitivity = Sensitivity;
    }
}
