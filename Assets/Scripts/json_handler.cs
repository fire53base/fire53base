using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class json_handler : MonoBehaviour
{


    public TMPro.TextMeshProUGUI text_Display;
    public RawImage frame_Display_image;
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Placement
    {
        public Position position { get; set; }
    }

    public class Operation
    {
        public string name { get; set; }
        public string argument { get; set; }

        public string input_text { get; set; }
    }

    public class Layer
    {
        public string type { get; set; }
        public string path { get; set; }



        public List<Placement> placement { get; set; }
        public List<Operation> operations { get; set; }
    }

    public class Root
    {
        public List<Layer> layers { get; set; }
    }


    // Start is called before the first frame update

   public string url = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_color.json";
    void Start()
    {
        StartCoroutine(GetRequest_forJSON(url));

    }

    IEnumerator GetRequest_forJSON(string url = "")
    {

        var uwr = new UnityWebRequest(url, "GET");
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);


        }
        else
        {


            String results = uwr.downloadHandler.text;



            Debug.Log("JSON loaded in coroutine: " + results);

            //To Process Json
            process_JSON(results);

        }
    }

    IEnumerator get_Image(string image_url, string col = "#FFFFFF")
    {
        WWW www = new WWW(image_url);
        yield return www;
        frame_Display_image.gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

        www.LoadImageIntoTexture(frame_Display_image.mainTexture
            as Texture2D);

        Color newCol;

        if (ColorUtility.TryParseHtmlString(col, out newCol))
        {
            frame_Display_image.color = newCol;
        }
    }

    public void process_JSON(string json_data)
    {
        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json_data);



        foreach (Layer ls in myDeserializedClass.layers)
        {
            //***********Text Layer Part***************//
            if (ls.type == "text")
            {
                if (ls.placement != null)
                    foreach (Placement pl in ls.placement)
                    {
                        RectTransform rt = text_Display.gameObject.GetComponent<RectTransform>();

                        rt.anchoredPosition = new Vector2(pl.position.x, pl.position.y);
                        rt.sizeDelta = new Vector2(pl.position.width, pl.position.height);


                    }

                if (ls.operations != null)
                    foreach (Operation op in ls.operations)
                    {
                        if (op.name == "color")
                        {
                            Color newCol;

                            if (ColorUtility.TryParseHtmlString(op.argument, out newCol))
                            {
                                text_Display.color = newCol;
                            }
                        }
                        //op.input_text = "Test!";
                        if(!String.IsNullOrEmpty( op.input_text))
                        {
                            text_Display.text = op.input_text;
                        }
                        else
                        {
                            text_Display.text = "";
                        }
                    }
            }
            //**************Frame Layer Part******************//
            else if (ls.type == "frame")
            {
                if (ls.placement != null)
                    foreach (Placement pl in ls.placement)
                    {
                        RectTransform rt = frame_Display_image.gameObject.GetComponent<RectTransform>();

                        rt.anchoredPosition = new Vector2(pl.position.x, pl.position.y);
                        rt.sizeDelta = new Vector2(pl.position.width, pl.position.height);


                    }




                string hex_col_code = "#FFFFFF";
                if (ls.operations != null)
                    foreach (Operation op in ls.operations)
                    {
                        if (op.name == "color")
                        {
                            hex_col_code = op.argument;

                        }
                    }

                if (!String.IsNullOrEmpty(ls.path))
                {
                    try
                    {


                        StartCoroutine(get_Image(ls.path, hex_col_code));
                    }
                    catch (Exception ex)
                    {

                        print("Image Display : " + ex);

                        //**********Display Image is invalid ************//
                        text_Display.text = "Invalid Image !";
                        frame_Display_image.color = Color.clear;
                    }
                }
            }



        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
