using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ERC721TokensData : MonoBehaviour
{
    public List<string> eRC721TokenContractList;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

public interface ISaveableERC721TokensData
{
    void PopulateERC721TokensData(ERC721TokensData eRC721TokensData);
    void LoadFromERC721TokensData(ERC721TokensData eRC721TokensData);
}

/*[System.Serializable]
public class ERC721TokenData : MonoBehaviour
{
    public string tokenOwnerPublicKey;
    public string contractAddress;
*//*    public string tokenName;
    public string tokenSymbol;
    public int totalSupply;*//*

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

public interface ISaveableERC721TokenData
{
    void PopulateERC721TokenData(ERC721TokenData eRC721TokenData);
    void LoadFromERC721TokenData(ERC721TokenData eRC721TokenData);
}*/