using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    //Tip text
    private Text text;
    //OK button
    private Button okBtn;

    //init
    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }
    //show
    public override void OnShow(params object[] args)
    {
        //Find component
        text = skin.transform.Find("Text").GetComponent<Text>();
        okBtn = skin.transform.Find("OkButton").GetComponent<Button>();
        //Listen
        okBtn.onClick.AddListener(OnOkClick);
        //Notice
        if (args.Length == 1)
        {
            text.text = (string)args[0];
        }
    }
    //close
    public override void OnClose()
    {
        
    }
    //On ok button click
    public void OnOkClick()
    {
        Close();
    }
}
