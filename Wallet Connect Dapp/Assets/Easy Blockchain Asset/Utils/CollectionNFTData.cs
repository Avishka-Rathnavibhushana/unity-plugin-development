using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionNFTData : MonoBehaviour
{
    public SerializableDictionary<string, string> collectionNFTList;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

public interface ISaveableCollectionNFTData
{
    void PopulatCollectionNFTData(CollectionNFTData collectionNFTData);
    void LoadFromCollectionNFTData(CollectionNFTData collectionNFTData);
}

/*[System.Serializable]
public class CollectionData : MonoBehaviour
{
    public string contractAddress;
    public List<NFTData> nftList;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

[System.Serializable]
public class NFTData : MonoBehaviour
{
    public string id;
    public string prefabTag;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}*/