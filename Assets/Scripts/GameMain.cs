using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static string id = "";

    // Start is called before the first frame update
    void Start()
    {
        //Init
        PanelManager.Init();
        //Open login panel
        PanelManager.Open<LoginPanel>();
    }
    
}
