using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class RegisterPanel : BasePanel
{
    //ID input
    private InputField idInput;
    //Password input
    private InputField pwInput;
    //Repeat input
    private InputField repInput;
    //Register button
    private Button registerBtn;
    //Close button
    private Button closeBtn;

    //Init
    public override void OnInit()
    {
        skinPath = "RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }
    //Show
    public override void OnShow(params object[] para)
    {
        //Find component
        idInput = skin.transform.Find("IdInput").GetComponent<InputField>();
        pwInput = skin.transform.Find("PwInput").GetComponent<InputField>();
        repInput = skin.transform.Find("RepInput").GetComponent<InputField>();
        registerBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //Listen
        registerBtn.onClick.AddListener(OnRegisterClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        
    }
    //Close
    public override void OnClose()
    {

    }
    //On register button click
    public void OnRegisterClick()
    {
        //User id and password are empty
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("User id and password can't be empty!");
            return;
        }
        //Passwords are different
        if (repInput.text != pwInput.text)
        {
            PanelManager.Open<TipPanel>("The passwords entered twice are different!");
            return;
        }
        //Send to server
        StartCoroutine(PostUserRegisterData(@"http://mbp.kelo.xyz:15551/api/register", idInput.text, pwInput.text));
        
    }
    //On close button click
    public void OnCloseClick()
    {
        Close();
    }
    IEnumerator PostUserRegisterData(string url, string id, string psw) {
        WWWForm form = new WWWForm();
        form.AddField("username", id);
        form.AddField("password", psw);
        

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.isHttpError||request.isNetworkError)
            PanelManager.Open<TipPanel>(request.error);
        else
        {
            string jsonForm = request.downloadHandler.text;

            RegisterMsg registerMsg = JsonConvert.DeserializeObject<RegisterMsg>(jsonForm);

            if(registerMsg.code == 1){
                PanelManager.Open<TipPanel> ();
            }else{
                PanelManager.Open<TipPanel>(registerMsg.msg);
            }
        }
    }
   
}
