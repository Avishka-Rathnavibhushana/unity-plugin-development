    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MarketPlaceContractData : MonoBehaviour
{
    public string contractAddress;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

/*public interface ISaveableMarketPlaceContractData
{
    void PopulateMarketPlaceContractData(MarketPlaceContractData marketPlaceContractData);
    void LoadFromMarketPlaceContractData(MarketPlaceContractData marketPlaceContractData);
}*/
