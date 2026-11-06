using UnityEngine;

public class Readinput_Jomo : MonoBehaviour
{
    private string input;
    public GameObject player;
    public GameObject hud;
    public GameObject inv;
    public GameObject login_UI;
    public GameObject main_UI;

    public GameObject folder_UI;
    public GameObject keytxt_UI;
    public GameObject keyfbx_UI;
    public GameObject Login_text;
    public GameObject flashdrive_txt;
    public GameObject line;
    private string password;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        password = PuzzleCodes.Get("jomo_pass");
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(password) && input == password){
            line.SetActive(true);
            input = "";
            login_UI.SetActive(false);
            main_UI.SetActive(true);
        }
    }
    public void ReadStringInput(string s)
    {
        input = s;
    }
    public void DoNotOpen(){
        folder_UI.SetActive(true);
    }
    public void NotePad(){
        folder_UI.SetActive(false);
    }
    public void keytxt(){
        keytxt_UI.SetActive(true);
    }
    public void keytext_close(){
        keytxt_UI.SetActive(false);
    }
    public void keyfbx(){
        keyfbx_UI.SetActive(true);
    }
    public void keyfbx_close(){
        keyfbx_UI.SetActive(false);
    }
    public void ExitButton_login()
    {

        login_UI.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
    public void ExitButton_main()
    {
        Login_text.SetActive(false);
        flashdrive_txt.SetActive(true);
        main_UI.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
}
