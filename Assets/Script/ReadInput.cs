using UnityEngine;

public class ReadInput : MonoBehaviour
{
    private string input;
    public GameObject player;
    public GameObject hud;
    public GameObject inv;
    public GameObject login_UI;
    public GameObject main_UI;
    public GameObject check_list;
    public GameObject line;
    public GameObject Login;

    public GameObject NotePad_UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(input == "701310"){
            Debug.Log("jeffTureeeeeee");
            line.SetActive(true);
            input = "";
            login_UI.SetActive(false);
            main_UI.SetActive(true);
        }
    }
    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log(input);
    }
    public void DoNotOpen(){
        NotePad_UI.SetActive(true);
    }
    public void NotePad(){
        NotePad_UI.SetActive(false);
    }
    public void ExitButton_login()
    {

        login_UI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // ล็อคเคอร์เซอร์ไว้กลางจอ
    }
    public void ExitButton_main()
    {   
        Login.SetActive(false);
        line.SetActive(false);
        check_list.SetActive(true);
        main_UI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // ล็อคเคอร์เซอร์ไว้กลางจอ
    }
}
