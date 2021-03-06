﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginPanel : BasePanel {
    //ID input
    private InputField idInput;
    //Password input
    private InputField pwInput;
    //Login button
    private Button loginButton;
    //Register button
    private Button registerButton;
    private Button exitBtn;

    //Init
    public override void OnInit () {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }
    //Show
    public override void OnShow (params object[] para) {
        //Find component
        idInput = skin.transform.Find ("IdInput").GetComponent<InputField> ();
        pwInput = skin.transform.Find ("PwInput").GetComponent<InputField> ();
        loginButton = skin.transform.Find ("LoginBtn").GetComponent<Button> ();
        registerButton = skin.transform.Find ("RegisterBtn").GetComponent<Button> ();
        exitBtn = skin.transform.Find ("ExitBtn").GetComponent<Button> ();
        //Listen
        loginButton.onClick.AddListener (OnLoginClick);
        registerButton.onClick.AddListener (OnRegisterClick);
        exitBtn.onClick.AddListener (OnExitClick);
    }
    //Close
    public override void OnClose () {

    }
    public void OnExitClick () {
        Application.Quit ();
    }

    //On login click 
    public void OnLoginClick () {
        //id and password are null
        if (idInput.text == "" || pwInput.text == "") {
            PanelManager.Open<TipPanel> ("User id and password can't be empty!");
            return;
        }
        //login succ...
        StartCoroutine (PostUserLoginMessage (idInput.text, pwInput.text, @"http://mbp.unalian.ga:18231/api/login"));
        //HandleUserID();

    }
    //On register click
    public void OnRegisterClick () {
        PanelManager.Open<RegisterPanel> ();
    }
    IEnumerator PostUserLoginMessage (string id, string psw, string url) {

        WWWForm form = new WWWForm ();
        form.AddField ("username", id);
        form.AddField ("password", psw);

        UnityWebRequest request = UnityWebRequest.Post (url, form);
        yield return request.SendWebRequest ();

        if (request.isHttpError || request.isNetworkError)
            PanelManager.Open<TipPanel> (request.error);
        else {
            string jsonForm = request.downloadHandler.text;
            //Debug.Log(jsonForm);

            LoginMsg loginMsg = JsonConvert.DeserializeObject<LoginMsg> (jsonForm);

            if (loginMsg.code == 1) {
                PanelManager.Open<UIPanel> ();
                //Debug.Log(loginMsg.msg);
                Close ();
            } else {
                PanelManager.Open<TipPanel> (loginMsg.msg);
            }
        }
    }

}