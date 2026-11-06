using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ChecklistTask
{
    ReadNote, LoginTaVChai, ReachHardware, LoginJoMo, CollectFlashdrive,
    PrintKey, CollectKey, FollowFootprints, DefuseBomb, SolveSensors,
    CollectBossKey, ReachBossRoom, CollectFinalDrive, DefeatBoss
}

// Progress belongs to the current scene, independently of which UI objects are visible.
public class ChecklistProgress : MonoBehaviour
{
    public static ChecklistProgress Instance { get; private set; }
    public event Action Changed;
    readonly bool[] completed = new bool[14];
    public int Count { get; private set; }
    public int CurrentIndex
    {
        get
        {
            for (int i = 0; i < completed.Length; i++) if (!completed[i]) return i;
            return completed.Length - 1;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    static void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameFlow.GameScene || Instance != null) return;
        var root = new GameObject("Checklist System");
        root.AddComponent<ChecklistProgress>();
        root.AddComponent<ChecklistUI>();
    }

    void Awake() => Instance = this;
    void OnDestroy() { if (Instance == this) Instance = null; }
    public bool IsComplete(ChecklistTask task) => completed[(int)task];

    public static void Complete(ChecklistTask task)
    {
        var progress = Instance;
        if (progress == null || GameFlow.GameEnded || progress.completed[(int)task]) return;
        progress.completed[(int)task] = true;
        progress.Count++;
        progress.Changed?.Invoke();
    }

    public static readonly string[] Titles =
    {
        "Read the note", "Log in to TaVChai", "Reach Hardware House", "Log in to JoMo",
        "Collect the flashdrive", "Print a key", "Collect the printed key", "Follow the footprints",
        "Defuse the bomb", "Solve the sensor puzzle", "Collect the boss key", "Find the boss room",
        "Collect the final flashdrive", "Find the anomalies"
    };

    public static readonly string[] Descriptions =
    {
        "Read the note in the starting room.", "Log in to TaVChai's computer.",
        "Find a route to Hardware House.", "Log in to JoMo's computer in Hardware House.",
        "Pick up the flashdrive.", "Use the flashdrive at the printer to produce a key.",
        "Pick up the key from the printer.", "Follow the footprints to their destination.",
        "Enter the correct code to defuse the bomb.", "Complete the sensor puzzle to reveal the keycard.",
        "Pick up the keycard.", "Reach the entrance to the boss room.",
        "Pick up the final flashdrive to begin the boss encounter.", "Find all five anomalies in the boss room."
    };

    // Conceptual help only: never read or expose the generated puzzle answers.
    public static readonly string[] Hints =
    {
        "Look for a paper note near the start. Move close and press E to read it.",
        "The note uses hexadecimal (base 16). A to F stand for 10 to 15. Convert the value to base 10 for the password.",
        "Search the rooms for door-code clues. For the prefix expression, each operator comes before its two operands. Work out the inner operations, then fill in the missing digit.",
        "Study KONIGSMONGKUTT'S MAP. An open Euler trail uses every edge once and has different start and end vertices. Count each vertex's edges: exactly two vertices must have odd degree. Use the code beneath the matching graph.",
        "Open the folder on JoMo's computer and select the key model. Wait for the save to finish, then move close to the flashdrive and press E to collect it.",
        "Bring the flashdrive to the resin printer. Interact with the printer while carrying it.",
        "After printing, look near the printer for the physical key and pick it up.",
        "Look down for footprints on the floor. Follow the trail rather than choosing a corridor at random.",
        "The clue asks for a combination: choose k items from n, with order ignored. Use C(n,k) = n! / (k! (n-k)!). Read n and k from this run's clue.",
        "Read the sensor instructions before switching anything. Work through the required states carefully; an incorrect final configuration can end the game.",
        "Once the sensor puzzle is solved, look for the keycard it reveals and collect it.",
        "Use the keycard to continue toward the boss room. Check the reference sketch outside before going in.",
        "Find the flashdrive in the final area. Collecting it starts the boss encounter.",
        "Compare the room with the reference sketch outside. Look for moved, rotated, resized, missing or extra objects. Face a nearby object and press E to inspect it. Three mistakes end the run."
    };
}
