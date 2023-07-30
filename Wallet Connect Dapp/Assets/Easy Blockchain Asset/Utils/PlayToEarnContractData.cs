using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayToEarnContractData : MonoBehaviour
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

/*public interface ISaveablePlayToEarnContractData
{
    void PopulatePlayToEarnContractData(PlayToEarnContractData playToEarnContractData);
    void LoadFromPlayToEarnContractData(PlayToEarnContractData playToEarnContractData);
}*/
