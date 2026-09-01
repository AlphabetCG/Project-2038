using UnityEngine;
using UnityEngine.SceneManagement;

public class mainmenulogicc : MonoBehaviour
{
    public GameObject loading;
    public AudioSource buttonSound;
    public void changescreen(){
        loading.SetActive(true);
        SceneManager.LoadScene("SampleScene");
    }
    public void ExitGameButton()
    {
        buttonSound.Play();
        Application.Quit();
        Debug.Log("App Has Exited");
    }
}
