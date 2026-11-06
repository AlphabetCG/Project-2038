using UnityEngine;

public class mainmenulogicc : MonoBehaviour
{
    public GameObject loading;
    public AudioSource buttonSound;
    public GameObject mainMenu;    // MainMenu_new
    public GameObject optionsMenu; // หน้า Options

    void Start()
    {
        GameFlow.UnlockCursor(); // กลับมาจากเกมแล้วเมาส์ต้องใช้งานได้
        if (optionsMenu != null) optionsMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    public void changescreen(){
        GameModeMenu.Open(StartSelectedGame);
    }
    void StartSelectedGame(){
        loading.SetActive(true);
        StartCoroutine(GameFlow.LoadSceneAsync(GameFlow.GameScene));
    }
    public void OpenOptions()
    {
        buttonSound.Play();
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void CloseOptions()
    {
        buttonSound.Play();
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void ExitGameButton()
    {
        buttonSound.Play();
        GameFlow.Quit();
    }
}
