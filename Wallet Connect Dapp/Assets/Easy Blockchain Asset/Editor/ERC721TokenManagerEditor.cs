using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

public class ERC721TokenManagerEditor : EditorWindow
{
    string privateKey = "";
    string publicKey = "";
    int chainId = Constants.AVALANCHE_TESTNET_CHAIN_ID;
    string rpcUrl = Constants.AVALANCHE_TESTNET_RPC_URL;

    bool loadedFromJson = false;

    int selected = 0;
    int selectedLast = -1;
    public List<string> contractAddresses = new List<string>();
    string contractAddress;

    [MenuItem("Window/Kaiju/ERC721 Token/ERC721 Token Manager")]
    public static void ShowWindow()
    {
        GetWindow<ERC721TokenManagerEditor>("ERC721 Token Manager");
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

        }
        else
        {
            if (GUILayout.Button("Deploy ERC721 Token", GUILayout.Width(150)))
            {
                DeployERC721();
            }
            LoadERC721TokensJsonData();
            if (this.contractAddresses.Count >= 1)
            {

                selected = EditorGUILayout.Popup("Contracts", selected, contractAddresses.ToArray());
                contractAddress = contractAddresses[selected];

                GUILayout.BeginHorizontal();
                GUILayout.Label("Selected Contract", EditorStyles.boldLabel, GUILayout.Width(130));
                EditorGUILayout.SelectableLabel(contractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                   
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

    private bool SaveERC721TokensJsonData(string contractAddress)
    {
        ERC721TokensData eRC721TokensData = new ERC721TokensData();

        PopulateERC721TokenData(eRC721TokensData, contractAddress);

        if (FileManager.WriteToFile("ERC721 Token Data.dat", eRC721TokensData.ToJson()))
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

    public void PopulateERC721TokenData(ERC721TokensData eRC721TokensData, string contractAddress)
    {
        this.contractAddresses.Add(contractAddress);
        eRC721TokensData.eRC721TokenContractList = this.contractAddresses;
    }

    private bool LoadERC721TokensJsonData()
    {
        if (FileManager.LoadFromFile("ERC721 Token Data.dat", out var json))
        {
            ERC721TokensData eRC721TokensData = new ERC721TokensData();
            eRC721TokensData.LoadFromJson(json);

            LoadFromERC721TokensData(eRC721TokensData);
            return true;
        }
        else
        {
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromERC721TokensData(ERC721TokensData eRC721TokensData)
    {
        this.contractAddresses = eRC721TokensData.eRC721TokenContractList;
        if (contractAddresses.Count >= 1)
        {
            contractAddress = contractAddresses[selected];

        }

    }

    public async void DeployERC721()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        Debug.Log(account.Address);

        var deploymentMessage = new StandardTokenDeploymentERC721 { };

        var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeploymentERC721>();
        var transactionReceiptDeployment = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);
        Debug.Log(transactionReceiptDeployment.ContractAddress);
        SaveERC721TokensJsonData(transactionReceiptDeployment.ContractAddress);
    }

}