using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ERC20TokensData : MonoBehaviour
{
    public List<string> eRC20TokenContractList;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

public interface ISaveableERC20TokensData
{
    void PopulateERC20TokensData(ERC20TokensData eRC20TokensData);
    void LoadFromERC20TokensData(ERC20TokensData eRC20TokensData);
}

/*[System.Serializable]
public class ERC20TokenData : MonoBehaviour
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

public interface ISaveableERC20TokenData
{
    void PopulateERC20TokenData(ERC20TokenData eRC20TokenData);
    void LoadFromERC20TokenData(ERC20TokenData eRC20TokenData);
}*/