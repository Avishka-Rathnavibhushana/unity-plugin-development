using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;

public class ImagePicker : MonoBehaviour
{
    public PinataClient pinataClient;
    public RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    string path;

    public void openFileExplorer()
    {
        path = EditorUtility.OpenFilePanel("Show all images","","jpg");

        StartCoroutine(GetTexture());
    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///"+path);
        
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.texture = myTexture;

            pinataClient.filePath = path;
            Debug.Log(path);
        }
    }
}
