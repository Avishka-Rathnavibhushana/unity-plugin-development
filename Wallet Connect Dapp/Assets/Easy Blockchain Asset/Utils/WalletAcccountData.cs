    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WalletAcccountData : MonoBehaviour
{
    public string mnemonicPhrase;
    public string privateKey;
    public string publicKey;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile, this);
    }
}

/*public interface ISaveableWalletAcccountData
{
    void PopulateWalletAcccountData(WalletAcccountData walletAcccountData);
    void LoadFromWalletAcccountData(WalletAcccountData walletAcccountData);
}*/
