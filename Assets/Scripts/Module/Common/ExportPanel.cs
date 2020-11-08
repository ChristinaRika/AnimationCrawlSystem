using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ExportPanel : BasePanel {
    private Button exportBtn;
    private Button choosePathBtn;
    private InputField pathInput;
    private Texture2D texture;
    private Button closeBtn;
    private Text pathShow;
    private Image image;
    private string folderPath = Application.streamingAssetsPath;
    private string imgName = "default.png";
    public override void OnInit () {
        skinPath = "ExportPanel";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow (params object[] para) {
        exportBtn = skin.transform.Find ("ExportBtn").GetComponent<Button> ();
        choosePathBtn = skin.transform.Find ("ChoosePathBtn").GetComponent<Button> ();
        pathInput = skin.transform.Find ("ImgNameInput").GetComponent<InputField> ();
        closeBtn = skin.transform.Find ("CloseBtn").GetComponent<Button> ();
        pathShow = skin.transform.Find ("Path").GetComponent<Text> ();
        image = (Image) para[0];
        if (pathInput.text != "") {
            imgName = pathInput.text;
            if (!(imgName.Contains (".png") || imgName.Contains (".jpg") || imgName.Contains (".gif"))) {
                imgName += ".png";
            }
        }

        exportBtn.onClick.AddListener (OnExportClick);
        choosePathBtn.onClick.AddListener (OnChoosePathClick);
        closeBtn.onClick.AddListener (OnCloseClick);
    }
    public override void OnClose () {
        //exit handle
    }
    public void OnCloseClick () {
        Close ();
    }
    public void OnChoosePathClick () {
        folderPath = string.Format (@"{0}\{1}", OpenFile (), imgName);
        pathShow.text = folderPath;
    }
    public void OnExportClick () {
        texture = GetTexture ();

        SaveTextureToFile (texture, folderPath);
        PanelManager.Open<TipPanel> ("Export Succ.");
    }
    private string OpenFile () {
        OpenDialogDir ofn2 = new OpenDialogDir ();    
        ofn2.pszDisplayName = new string (new char[2000]);//buffer to store  
            
        ofn2.lpszTitle = "Choose Path"; // title
        
        const int BIF_EDITBOX = 0x00000010;
        
        const int BIF_NEWDIALOGSTYLE = 0x00000040;
        ofn2.ulFlags = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);    
        ofn2.hwndOwner = DllOpenFileDialog.GetForegroundWindow ();

        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder (ofn2);

            
        char[] charArray = new char[2000];   
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';
        charArray = Application.streamingAssetsPath.ToCharArray ();    
        DllOpenFileDialog.SHGetPathFromIDList (pidlPtr, charArray);    
        string fullDirPath = new String (charArray);    
        fullDirPath = fullDirPath.Substring (0, fullDirPath.IndexOf ('\0'));

            
        return fullDirPath;
    }
    //render image texture into tex and export.
    private Texture2D GetTexture () {
        return DuplicateTexture (image.sprite.texture); //Make texture2D readable.
    }
    private void SaveTextureToFile (Texture2D tex, string path) {
        byte[] img = tex.EncodeToPNG ();
        System.IO.File.WriteAllBytes (path, img);

    }

    private Texture2D DuplicateTexture (Texture2D source) {
        RenderTexture renderTex = RenderTexture.GetTemporary (
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit (source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D (source.width, source.height);
        readableText.ReadPixels (new Rect (0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply ();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary (renderTex);
        return readableText;
    }
}