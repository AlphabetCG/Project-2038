using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// A private runtime volume leaves authored lighting and shared profiles intact.
public class DisplayBrightness : MonoBehaviour
{
    public const float Minimum = 0.5f;
    public const float Maximum = 2f;
    const string PreferenceKey = "DisplayBrightness";
    static DisplayBrightness instance;
    VolumeProfile profile;
    ColorAdjustments adjustments;

    public static float Value => Mathf.Clamp(PlayerPrefs.GetFloat(PreferenceKey, 1f), Minimum, Maximum);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance() => instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (instance != null) return;
        var root = new GameObject("Display Brightness");
        DontDestroyOnLoad(root);
        instance = root.AddComponent<DisplayBrightness>();
    }

    void Awake()
    {
        profile = ScriptableObject.CreateInstance<VolumeProfile>();
        adjustments = profile.Add<ColorAdjustments>();
        var volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 1000;
        volume.sharedProfile = profile;
        Apply();
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Camera.main != null)
            Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        Apply();
    }

    public static void Set(float value)
    {
        PlayerPrefs.SetFloat(PreferenceKey, Mathf.Clamp(value, Minimum, Maximum));
        if (instance != null) instance.Apply();
    }

    void Apply() => adjustments.postExposure.Override(Mathf.Log(Value, 2f));

    void OnDestroy()
    {
        if (instance == this) instance = null;
        if (profile == null) return;
        foreach (var component in profile.components) Destroy(component);
        Destroy(profile);
    }
}
