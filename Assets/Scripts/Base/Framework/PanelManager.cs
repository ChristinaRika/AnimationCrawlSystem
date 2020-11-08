using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager 
{
    //Layer
    public enum Layer
    {
        Panel,
        Tip,
    }
    //Layer list
    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
    //Panel list
    public static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
    //Hieraochy
    public static Transform root;
    public static Transform canvas;

    //Init
    public static void Init()
    {
        root = GameObject.Find("MainSystem").transform;
        canvas = root.Find("Canvas");
        Transform panel = canvas.Find("Panel");
        Transform tip = canvas.Find("Tip");
        layers.Add(Layer.Panel, panel);
        layers.Add(Layer.Tip, tip);
    }
    //Open panel
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        //Already open
        string name = typeof(T).ToString();
        if (panels.ContainsKey(name))
        {
            return;
        }
        //Components
        BasePanel panel = root.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();
        //Parent container
        Transform layer = layers[panel.layer];
        panel.skin.transform.SetParent(layer, false);
        //List
        panels.Add(name, panel);
        //On show
        panel.OnShow(para);
    }
    //Close panel
    public static void Close(string name)
    {
        //Not open
        if (!panels.ContainsKey(name))
        {
            return;
        }
        BasePanel panel = panels[name];
        //On close
        panel.OnClose();
        //List
        panels.Remove(name);
        //Distroy
        GameObject.Destroy(panel.skin);
        Component.Destroy(panel);
    }
}
