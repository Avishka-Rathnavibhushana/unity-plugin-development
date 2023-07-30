using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

public class ERC20TokenManagerEditor : EditorWindow
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

    string totalSupply = "1000000000000000000";

    [MenuItem("Window/Kaiju/ERC20 Tokens/ERC20 Token Manager")]
	public static void ShowWindow()
	{
		GetWindow<ERC20TokenManagerEditor>("ERC20 Token Manager");
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
            totalSupply =
                EditorGUILayout.TextField("Total Supply", totalSupply, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Deploy ERC20 Token", GUILayout.Width(150)))
            {
                
                DeployERC20();
            }
            LoadERC20TokensJsonData();
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

    private bool SaveERC20TokensJsonData(string contractAddress)
    {
        ERC20TokensData eRC20TokensData = new ERC20TokensData();
        Debug.Log(contractAddress);
        PopulateERC20TokenData(eRC20TokensData, contractAddress);

        if (FileManager.WriteToFile("ERC20 Token Data.dat", eRC20TokensData.ToJson()))
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

    public void PopulateERC20TokenData(ERC20TokensData eRC20TokensData, string contractAddress)
    {
        this.contractAddresses.Add(contractAddress);
        eRC20TokensData.eRC20TokenContractList = this.contractAddresses;
    }

    private bool LoadERC20TokensJsonData()
    {
        if (FileManager.LoadFromFile("ERC20 Token Data.dat", out var json))
        {
            ERC20TokensData eRC20TokensData = new ERC20TokensData();
            eRC20TokensData.LoadFromJson(json);

            LoadFromERC20TokensData(eRC20TokensData);
            return true;
        }
        else
        {
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromERC20TokensData(ERC20TokensData eRC20TokensData)
    {
        this.contractAddresses = eRC20TokensData.eRC20TokenContractList;
        if (contractAddresses.Count >= 1)
        {
            contractAddress = contractAddresses[selected];

        }
        
    }

    public async void DeployERC20()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var deploymentMessage = new StandardERC20TokenDeployment
        {
            TotalSupply = UInt64.Parse(totalSupply)
        };

        var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardERC20TokenDeployment>();
        var transactionReceiptDeployment = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

        SaveERC20TokensJsonData(transactionReceiptDeployment.ContractAddress);
    }
}