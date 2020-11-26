using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIPanel : BasePanel {
    // Start is called before the first frame update
    private Button crawlBtn;
    private Button openBtn;
    private InputField tagInput;
    private Button nextBtn;
    private Button exportBtn;
    private Button exitBtn;

    private Image image;

    private int imageIdx = 0;
    List<string> filePaths = new List<string> ();
    public override void OnInit () {
        skinPath = "UIPanel";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow (params object[] para) {
        //find objects
        openBtn = skin.transform.Find ("GetBtn").GetComponent<Button> ();
        nextBtn = skin.transform.Find ("NextBtn").GetComponent<Button> ();
        crawlBtn = skin.transform.Find ("CrawlBtn").GetComponent<Button> ();
        image = skin.transform.Find ("Viewport").GetComponent<Image> ();
        tagInput = skin.transform.Find ("imageTag").GetComponent<InputField> ();
        exportBtn = skin.transform.Find ("ExportBtn").GetComponent<Button> ();
        exitBtn = skin.transform.Find("ExitBtn").GetComponent<Button>();
        //listener
        openBtn.onClick.AddListener (OnGetClick);
        nextBtn.onClick.AddListener (OnNextClick);
        exportBtn.onClick.AddListener (OnExportClick);
        exitBtn.onClick.AddListener (OnExitClick);
        crawlBtn.onClick.AddListener (OnCrawlClick);
    }
    public override void OnClose () {
        //exit handle
    }
    public void OnGetClick () {
        StartCoroutine (GetImage (@"http://mbp.kelo.xyz:15551/api/file", tagInput.text));
    }

    IEnumerator GetImage (string url, string tag) {
        WWWForm form = new WWWForm ();
        form.AddField ("tag", tag);

        UnityWebRequest request = UnityWebRequest.Post (url, form);
        yield return request.SendWebRequest ();

        if (request.isHttpError || request.isNetworkError) {
            PanelManager.Open<TipPanel> (request.error);
        } else {
            string jsonForm = request.downloadHandler.text;

            ImgReceive imgReceive = JsonConvert.DeserializeObject<ImgReceive> (jsonForm);
            PanelManager.Open<TipPanel> (string.Format ("Read {0} Images.", imgReceive.lists.Count));
            filePaths.Clear();//init file paths list
            foreach (ImgUnit img in imgReceive.lists) {
                StartCoroutine (LoadImages (img.file_name));
            }
        }
    }
    public void OnCrawlClick(){
        //StartCoroutine(CrawlImage(@"#########CRWALAPI########",tagInput.tag));//crwal
    }
    IEnumerator CrawlImage(string url, string tag){
        WWWForm form = new WWWForm();
        form.AddField("tag", tag);
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();
        if(request.isHttpError||request.isNetworkError){
            PanelManager.Open<TipPanel>(request.error);
        }else{
            string jsonForm = request.downloadHandler.text;
            //Get text of message
            //PanelManager.Open<TipPanel>(message);
        }
    }
    IEnumerator LoadImages (string file_name) {
        //add image paths into List
        filePaths.Add (string.Format ("http://mbp.kelo.xyz:15551/api/img/" + file_name));
        yield return null;
    }

    public void OnNextClick () {
        if (filePaths.Count != 0) {
            StartCoroutine (Load (filePaths[imageIdx]));
            imageIdx = (imageIdx + 1) % filePaths.Count;
        }
    }

    IEnumerator Load (string path) {

        var request = UnityWebRequestTexture.GetTexture (path);
        yield return request.SendWebRequest ();

        if (request.isHttpError || request.isNetworkError) {
            PanelManager.Open<TipPanel> (request.error);
        } else {
            //Get Texture
            Texture2D texture = DownloadHandlerTexture.GetContent (request);

            //create sprite based on texture
            Sprite sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (5.243927f, 2.809875f));
            //show sprite into image
            image.sprite = sprite;
            //set narive size of picure
            image.SetNativeSize ();

            //resize RectTransform(here is [800]X[800])
            float width;
            float height;
            if (texture.width > texture.height) {
                width = 800.0f;
                height = (width / (float) texture.width) * texture.height;
            } else {
                height = 800.0f;
                width = (height / (float) texture.height) * texture.width;
            }
            image.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
        }
    }
    public void OnExportClick () {
        PanelManager.Open<ExportPanel> (image);
    }
    public void OnExitClick () {
        Application.Quit();
    }
}