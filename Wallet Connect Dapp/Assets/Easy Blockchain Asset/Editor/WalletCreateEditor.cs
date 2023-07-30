using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Here we import the Netherum.JsonRpc methods and classes.

using NBitcoin;

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.HdWallet;

public class WalletCreateEditor : EditorWindow
{
    string mnemonicPhrase = "";
    string privateKey = "";
    string publicKey = "";

    bool loadedFromJson = false;

    [MenuItem("Window/Kaiju/Wallet Create")]
	public static void ShowWindow()
	{
		GetWindow<WalletCreateEditor>("Wallet Create");
	}
    Rect buttonRect;
    void OnGUI()
	{
        loadedFromJson = LoadJsonData();

        if (!loadedFromJson)
        {
            if (GUILayout.Button("Create Wallet", GUILayout.Width(130)))
            {
                CreateAccount();
            }
        }
        else
        {

            if (GUILayout.Button("Delete Wallet", GUILayout.Width(130)))
            {
                DeleteAccount();
            }
        }

        
    }

    public async void CreateAccount()
    {
        Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

        string mnemonicPhraseLocal = mnemonic.ToString();

        string password = "";   // By default this password is set as empty string in wallets like metamask

        // Create / recover wallet from memonic phrase
        var wallet = new Wallet(mnemonicPhraseLocal, password);

        // Create / recover account 0 of the wallet
        var account = wallet.GetAccount(0);

        // Public key of the account
        string publicKeyLocal = account.Address;

        // Private key of the account
        Debug.Log(account.PrivateKey);
        string privateKeyLocal = account.PrivateKey.Substring(2);
        Debug.Log(privateKeyLocal);

        bool saved = SaveJsonData(mnemonicPhraseLocal, publicKeyLocal, privateKeyLocal);

        if (saved)
        {
            LoadJsonData();
        }

        /*// Get account balance
        getBalance(address); */
    }

    public async void DeleteAccount()
    {
        if (FileManager.DeleteFile("Wallet Acccount Data.dat"))
        {
            GUILayout.Label("Not yet created a wallet.\nCreate a new wallet.", EditorStyles.boldLabel);
            Debug.Log("Successfuly deleted");
        }
        else
        {
            Debug.Log("Deleting Failed");
        }

        /*// Get account balance
        getBalance(address); */
    }

    private bool SaveJsonData(string mnemonicPhraseLocal, string publicKeyLocal, string privateKeyLocal)
    {
        WalletAcccountData walletAcccountData = new WalletAcccountData();
        PopulateWalletAcccountData(walletAcccountData, mnemonicPhraseLocal, publicKeyLocal, privateKeyLocal);

        if (FileManager.WriteToFile("Wallet Acccount Data.dat", walletAcccountData.ToJson()))
        {
            Debug.Log("Successfuly saved");
            return true;
        }
        else
        {
            Debug.Log("Saving Failed");
            return false;
        }
    }

    public void PopulateWalletAcccountData(WalletAcccountData walletAcccountData, string mnemonicPhraseLocal, string publicKeyLocal, string privateKeyLocal)
    {
        walletAcccountData.mnemonicPhrase = mnemonicPhraseLocal;
        walletAcccountData.publicKey = publicKeyLocal.ToLower();
        walletAcccountData.privateKey = privateKeyLocal.ToLower();
    }
    
    private bool LoadJsonData()
    {
        if (FileManager.LoadFromFile("Wallet Acccount Data.dat", out var json))
        {
            WalletAcccountData walletAcccountData = new WalletAcccountData();
            walletAcccountData.LoadFromJson(json);

            LoadFromWalletAcccountData(walletAcccountData);
            return true;
        }
        else
        {
            displayEmptyText();
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromWalletAcccountData(WalletAcccountData walletAcccountData)
    {
        // Set local variables
        mnemonicPhrase = walletAcccountData.mnemonicPhrase;
        privateKey = walletAcccountData.privateKey;
        publicKey = walletAcccountData.publicKey;

        // Set UI Text Fields
        GUILayout.BeginHorizontal();
        GUILayout.Label("Mnemonic Phrase", EditorStyles.boldLabel, GUILayout.Width(130));
        EditorGUILayout.SelectableLabel(mnemonicPhrase, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Private Key", EditorStyles.boldLabel, GUILayout.Width(130));
        EditorGUILayout.SelectableLabel(privateKey, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Public Key", EditorStyles.boldLabel, GUILayout.Width(130));
        EditorGUILayout.SelectableLabel(publicKey, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        GUILayout.EndHorizontal();
    }

    public void displayEmptyText()
    {
        GUILayout.Label("Not yet created a wallet.\nCreate a new wallet.", EditorStyles.boldLabel);
    }
}