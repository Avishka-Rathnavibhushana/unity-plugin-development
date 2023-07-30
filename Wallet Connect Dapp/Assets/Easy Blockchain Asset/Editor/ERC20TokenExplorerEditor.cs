using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using Nethereum.Web3;
using Nethereum.Util;
using System.Numerics;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.ABI.FunctionEncoding.Attributes;

public class ERC20TokenExplorerEditor : EditorWindow
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

    string balance;
    string name;
    string symbol;
    string totalSupply;

    bool loadedFromPlayToEarnContractData = false;
    public string playToEarnContractAddress = "";
    string allowance;

    [MenuItem("Window/Kaiju/ERC20 Tokens/ERC20 Token Explorer")]
	public static void ShowWindow()
	{
		GetWindow<ERC20TokenExplorerEditor>("ERC20 Token Explorer");
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
            LoadERC20TokensJsonData();
            if (this.contractAddresses.Count >= 1)
            {

                selected = EditorGUILayout.Popup("Contracts", selected, contractAddresses.ToArray());
                contractAddress = contractAddresses[selected];

                if (selected != selectedLast)
                {
                    UpdateTokenData();
                    selectedLast = selectedLast;
                }
                

                GUILayout.BeginHorizontal();
                GUILayout.Label("Token Name", EditorStyles.boldLabel, GUILayout.Width(130));
                EditorGUILayout.SelectableLabel(name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Token Symbol", EditorStyles.boldLabel, GUILayout.Width(130));
                EditorGUILayout.SelectableLabel(symbol, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Total Supply", EditorStyles.boldLabel, GUILayout.Width(130));
                EditorGUILayout.SelectableLabel(totalSupply, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Owner Balance", EditorStyles.boldLabel, GUILayout.Width(130));
                EditorGUILayout.SelectableLabel(balance, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Selected Contract", EditorStyles.boldLabel, GUILayout.Width(130));
                EditorGUILayout.SelectableLabel(contractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();

                GUILayout.Label("Approve all tokens to  Play To Earn Contract", EditorStyles.boldLabel);
                GUILayout.Label("============================================", EditorStyles.boldLabel);

                loadedFromPlayToEarnContractData = LoadPlayToEarnContractData();

                if (loadedFromPlayToEarnContractData)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Allowance", EditorStyles.boldLabel, GUILayout.Width(130));
                    EditorGUILayout.SelectableLabel(allowance, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    GUILayout.EndHorizontal();

                    if (allowance != balance)
                    {
                        if (GUILayout.Button("Approve All", GUILayout.Width(180)))
                        {
                            ApproveAll();
                        }
                    }
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

    private bool SaveERC20TokensJsonData(string contractAddress)
    {
        ERC20TokensData eRC20TokensData = new ERC20TokensData();

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
            displayEmptyERC20Text();
            Debug.Log("No File");
            return false;
        }
    }

    public void displayEmptyERC20Text()
    {
        GUILayout.Label("Not yet deployed any ERC20 tokens.\nDeploy an ERC20 token first.", EditorStyles.boldLabel);
    }

    public void LoadFromERC20TokensData(ERC20TokensData eRC20TokensData)
    {
        this.contractAddresses = eRC20TokensData.eRC20TokenContractList;
        if (contractAddresses.Count >= 1)
        {
            contractAddress = contractAddresses[selected];

        }
        
    }

    public void UpdateTokenData()
    {
        getBalance();
        getName();
        getSymbol();
        getTotalSupply();
        getAllowance();
    }

    public async void getBalance()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var balanceOfFunctionMessage = new BalanceOfFunction()
        {
            Owner = publicKey,
        };

        var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
        var balanceLocal = await balanceHandler.QueryAsync<BigInteger>(contractAddress, balanceOfFunctionMessage);

        balance = balanceLocal.ToString();
    }

    public async void getName()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var nameOfFunctionMessage = new NameOfFunction()
        {

        };

        var nameHandler = web3.Eth.GetContractQueryHandler<NameOfFunction>();
        var nameLocal = await nameHandler.QueryAsync<string>(contractAddress, nameOfFunctionMessage);

        name = nameLocal;
    }

    public async void getSymbol()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var symbolOfFunctionMessage = new SymbolOfFunction()
        {

        };

        var symbolHandler = web3.Eth.GetContractQueryHandler<SymbolOfFunction>();
        var symbolLocal = await symbolHandler.QueryAsync<string>(contractAddress, symbolOfFunctionMessage);

        symbol = symbolLocal;
    }

    public async void getTotalSupply()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var totalSupplyOfFunctionMessage = new TotalSupplyOfFunction()
        {

        };

        var totalSupplyHandler = web3.Eth.GetContractQueryHandler<TotalSupplyOfFunction>();
        var totalSupplyLocal = await totalSupplyHandler.QueryAsync<BigInteger>(contractAddress, totalSupplyOfFunctionMessage);

        totalSupply = totalSupplyLocal.ToString();
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

    public async void getAllowance()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var functionMessage = new AllowanceFunctionERC20()
        {
            owner = publicKey,
            spender = playToEarnContractAddress
        };

        var handler = web3.Eth.GetContractQueryHandler<AllowanceFunctionERC20>();
        var result = await handler.QueryAsync<BigInteger>(contractAddress, functionMessage);

        allowance = result.ToString();
    }

    public void LoadFromPlayToEarnContractData(PlayToEarnContractData playToEarnContractData)
    {
        // Set local variables
        playToEarnContractAddress = playToEarnContractData.contractAddress;

        // Set UI Text Fields
        GUILayout.BeginHorizontal();
        GUILayout.Label("Play to Earn Contract Address", EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.SelectableLabel(playToEarnContractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        GUILayout.EndHorizontal();
    }

    public void displayEmptyTextPlayToEarnContractData()
    {
        GUILayout.Label("Not yet created a Play To Earn Contract.\nCreate a new Play To Earn Contract First.", EditorStyles.boldLabel);
    }

    public async void ApproveAll()
    {
        string spender = playToEarnContractAddress;
        BigInteger amount = BigInteger.Parse(balance);
        string transferingERC20TokenContractAddress = contractAddress;

        var account = new Account(this.privateKey, this.chainId);
        var web3 = new Web3(account, this.rpcUrl);

        var functionMessage = new ApproveFunctionERC20()
        {
            spender = spender,
            amount = amount
        };

        var handler = web3.Eth.GetContractTransactionHandler<ApproveFunctionERC20>();
        var receipt = await handler.SendRequestAndWaitForReceiptAsync(transferingERC20TokenContractAddress, functionMessage);

        Debug.Log("Approve Successful");
    }
}