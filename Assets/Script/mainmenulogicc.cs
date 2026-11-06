using UnityEngine;

public class mainmenulogicc : MonoBehaviour
{
    public GameObject loading;
    public AudioSource buttonSound;
    public void changescreen(){
        loading.SetActive(true);
        StartCoroutine(GameFlow.LoadSceneAsync(GameFlow.GameScene));
    }
    public void ExitGameButton()
    {
        buttonSound.Play();
        GameFlow.Quit();
    }
}
