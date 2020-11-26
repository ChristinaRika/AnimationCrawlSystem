using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{

    //Skin path
    public string skinPath;
    //Skin
    
    public GameObject skin;
    //Layer
    public PanelManager.Layer layer = PanelManager.Layer.Panel;

    //Init
    public void Init()
    {
        //Skin
        GameObject skinPrefab = ResManager.LoadPrefab(skinPath);
        skin = (GameObject)Instantiate(skinPrefab);
    }
    //Close
    public void Close()
    {
        string name = this.GetType().ToString();
        PanelManager.Close(name);
    }

    //On init
    public virtual void OnInit()
    {

    }
    //On show
    public virtual void OnShow(params object[] para)
    {

    }
    //On close
    public virtual void OnClose()
    {

    }
}
