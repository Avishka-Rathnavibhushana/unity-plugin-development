using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

public class PlayToEarnContractEditor : EditorWindow
{ 
    string privateKey = "";
    string publicKey = "";

    bool loadedFromJson = false;
    bool loadedFromPlayToEarnContractData = false;

    public string playToEarnContractAddress = "";

    int chainId = Constants.AVALANCHE_TESTNET_CHAIN_ID;
    string rpcUrl = Constants.AVALANCHE_TESTNET_RPC_URL;

    [MenuItem("Window/Kaiju/Play To Earn")]
	public static void ShowWindow()
	{
		GetWindow<PlayToEarnContractEditor>("Play To Earn");
	}

    Rect buttonRect;
    void OnGUI()
    {
        loadedFromJson = LoadPrivateKey();

        if (!loadedFromJson)
        {
            /*if (GUILayout.Button("Create Wallet", GUILayout.Width(130)))
            {
                CreateAccount();
            }*/
            GUILayout.Label("First create an wallet with Kaiju/Wallet Create", EditorStyles.boldLabel);
        }
        else
        {
            loadedFromPlayToEarnContractData = LoadPlayToEarnContractData();

            if (!loadedFromPlayToEarnContractData)
            {
                if (GUILayout.Button("Create Play To Earn Contract", GUILayout.Width(180)))
                {
                    DeployPlayToEarnContract();
                }
            }
            else
            {

                if (GUILayout.Button("Delete Play To Earn Contract", GUILayout.Width(180)))
                {
                    DeletePlayToEarnContract();
                }
            }


        }


    }

    private bool LoadPrivateKey()
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
        privateKey = walletAcccountData.privateKey;
        publicKey = walletAcccountData.publicKey;
    }

    public void displayEmptyText()
    {
        GUILayout.Label("Not yet created a wallet.\nCreate a new wallet First.", EditorStyles.boldLabel);
    }

    public async void DeployPlayToEarnContract()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        Debug.Log(account.Address);

        var deploymentMessage = new StandardPlayToEarnContractDeployment { };

        var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardPlayToEarnContractDeployment>();
        var transactionReceiptDeployment = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

        SavePlayToEarnContractData(transactionReceiptDeployment.ContractAddress);
    }

    public async void DeletePlayToEarnContract()
    {
        if (FileManager.DeleteFile("Play To Earn Contract Data.dat"))
        {
            GUILayout.Label("Not yet created a Play To Earn Contract.\nCreate a new Play To Earn Contract First.", EditorStyles.boldLabel);
            Debug.Log("Successfuly deleted");
        }
        else
        {
            Debug.Log("Deleting Failed");
        }
    }

    private bool SavePlayToEarnContractData(string contractAddressLocal)
    {
        PlayToEarnContractData playToEarnContractData = new PlayToEarnContractData();
        PopulatePlayToEarnContractData(playToEarnContractData, contractAddressLocal);

        if (FileManager.WriteToFile("Play To Earn Contract Data.dat", playToEarnContractData.ToJson()))
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

    public void PopulatePlayToEarnContractData(PlayToEarnContractData playToEarnContractData, string contractAddressLocal)
    {
        playToEarnContractData.contractAddress = contractAddressLocal;
    }

    private bool LoadPlayToEarnContractData()
    {
        if (FileManager.LoadFromFile("Play To Earn Contract Data.dat", out var json))
        {
            PlayToEarnContractData playToEarnContractData = new PlayToEarnContractData();
            playToEarnContractData.LoadFromJson(json);

            LoadFromPlayToEarnContractData(playToEarnContractData);
            return true;
        }
        else
        {
            displayEmptyTextPlayToEarnContractData();
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromPlayToEarnContractData(PlayToEarnContractData playToEarnContractData)
    {
        // Set local variables
        playToEarnContractAddress = playToEarnContractData.contractAddress;

        // Set UI Text Fields
        GUILayout.BeginHorizontal();
        GUILayout.Label("Contract Address", EditorStyles.boldLabel, GUILayout.Width(130));
        EditorGUILayout.SelectableLabel(playToEarnContractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        GUILayout.EndHorizontal();
    }

    public void displayEmptyTextPlayToEarnContractData()
    {
        GUILayout.Label("Not yet created a Play To Earn Contract.\nCreate a new Play To Earn Contract First.", EditorStyles.boldLabel);
    }

}