using UnityEngine;

public class quitGame : MonoBehaviour
{
    public void Quit()
    {
        GameFlow.Quit(); // ใน Editor จะหยุด Play mode, ตอน Build จะปิดเกม
    }

    public void Restart()
    {
        GameFlow.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        GameFlow.LoadScene(GameFlow.MainMenuScene);
    }
}
