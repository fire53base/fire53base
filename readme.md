
Quick Start:

*The URL for JSON can be added as string in Editor Mode throught the Script "json_handler.cs" attached to "JSON_handler".
*On clicking play button in Editor Mode , the JSON file is fetched and Layers are rendered on the screen.

Example Use :

 public string url = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_only.json";
    void Start()
    {
        StartCoroutine(GetRequest_forJSON(url));

    }
