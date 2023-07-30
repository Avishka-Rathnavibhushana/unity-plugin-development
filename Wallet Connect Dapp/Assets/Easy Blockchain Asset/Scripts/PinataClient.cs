using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using UnityEngine.Networking;

[System.Serializable]
public class PinataRsponse
{
    public string IpfsHash;
    public int PinSize;
    public string Timestamp;
    public bool isDuplicate;
}

public class PinataClient : MonoBehaviour
{

    public string filePath;
    // Pinata API endpoint
    private static readonly string nftStorageApiUrl = "https://api.pinata.cloud/pinning/pinFileToIPFS";

    // HTTP client to communicate with Pinata
    private static readonly HttpClient pinataHttpClient = new HttpClient();

    // Pinata API key
    public string PINATA_API_KEY;

    // Pinata Secret API key
    public string PINATA_SECRET_API_KEY;

    void Start()
    {
       
    }

    /**
    <summary>Upload a local file using "nft.storage" HTTP API</summary>
    <param name="path">The full path of local file to be uploaded</param>
    <returns>A "Task" which result is a "NFTStorageUploadResponse" object, obtained by parsing JSON from "nft.storage" API response (POST /upload endpoint)</returns>
    */

    public async Task<string> UploadDataFromFile(string path)
    {
        Debug.Log($"Uploading a text file [{path}].");
        if (string.IsNullOrWhiteSpace(path))
            Debug.Log("Argumwnt null Exception");

        if (!File.Exists(path))
            Debug.Log("File not found");

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(path));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

        form.Add(fileContent, "file", Path.GetFileName(path));

        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("pinata_api_key", PINATA_API_KEY);
        httpClient.DefaultRequestHeaders.Add("pinata_secret_api_key", PINATA_SECRET_API_KEY);

        var response = await httpClient.PostAsync(nftStorageApiUrl, form);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        Debug.Log("Uploading is complete.");
        Debug.Log($"API Response: {result}\n\n");

        PinataRsponse pinataResponse = JsonUtility.FromJson<PinataRsponse>(result);
        string IpfsHash = pinataResponse.IpfsHash;
        Debug.Log("IpfsHash : " + IpfsHash);

        string accessLink = "https://gateway.pinata.cloud/ipfs/" + IpfsHash;

        return accessLink;

    }

    public async void UploadFile()
    {
        string path = filePath;
        string link = await UploadDataFromFile(path);
        Debug.Log(link);
    }
}
