using System;
using System.Numerics;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

public class MarketPlaceContractEditor : EditorWindow

{ 
    string privateKey = "";
    string publicKey = "";
    int chainId = Constants.AVALANCHE_TESTNET_CHAIN_ID;
    string rpcUrl = Constants.AVALANCHE_TESTNET_RPC_URL;

    bool loadedFromJson = false;
    bool loadedFromMarketPlaceContractData = false;

    public string marketPlaceContractAddress = "";

    int selected = 0;
    int selectedLast = -1;
    public List<string> contractAddresses = new List<string>();
    string contractAddress;

    string totalTokenCount;
    string name;
    string symbol;
    bool approvedForAll = false;

    string nftApprovingMarket;

    int selectedNFT = 0;
    int selectedNFTLast = -1;
    public List<string> nftList = new List<string>();
    string nft;

    string getApproved;
    bool alreadyInMarketPlace;
    string nftOwner;
    string tokenPrice;
    string erc20TokenAddress;

    [MenuItem("Window/Kaiju/Market Place")]
	public static void ShowWindow()
	{
		GetWindow<MarketPlaceContractEditor>("Market Place");
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
            loadedFromMarketPlaceContractData = LoadMarketPlaceContractData();

            if (!loadedFromMarketPlaceContractData)
            {
                if (GUILayout.Button("Create Market Place Contract", GUILayout.Width(180)))
                {
                    DeployMarketPlaceContract();
                }
            }
            else
            {

                if (GUILayout.Button("Delete Market Place Contract", GUILayout.Width(180)))
                {
                    DeleteMarketPlaceContract();
                }

                LoadERC721TokensJsonData();
                if (this.contractAddresses.Count >= 1)
                {

                    selected = EditorGUILayout.Popup("Contracts", selected, contractAddresses.ToArray());
                    contractAddress = contractAddresses[selected];

                    if (selected != selectedLast)
                    {
                        UpdateTokenData();
                        this.nftList = new List<string>();
                        selectedNFT = 0;
                        selectedNFTLast = -1;
                        Debug.Log("on update " + totalTokenCount);
                    }

                    GUILayout.Label("Selected Collection Details", EditorStyles.boldLabel);
                    GUILayout.Label("=========================", EditorStyles.boldLabel);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Collection name", EditorStyles.boldLabel, GUILayout.Width(130));
                    EditorGUILayout.SelectableLabel(name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Collection Symbol", EditorStyles.boldLabel, GUILayout.Width(130));
                    EditorGUILayout.SelectableLabel(symbol, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Selected Contract", EditorStyles.boldLabel, GUILayout.Width(130));
                    EditorGUILayout.SelectableLabel(contractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Approve All NFTs to Market Place", EditorStyles.boldLabel);
                    GUILayout.Label("---------------------------------", EditorStyles.boldLabel);

                    if (!approvedForAll)
                    {
                        if (GUILayout.Button("Approve All", GUILayout.Width(180)))
                        {
                            ApproveAll(true);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Already approved all NFTs to the market place" , EditorStyles.boldLabel);
                        if (GUILayout.Button("Remove Approve All", GUILayout.Width(200)))
                        {
                            ApproveAll(false);
                        }
                    }

                    GUILayout.Label("Selected Collection NFT Details", EditorStyles.boldLabel);
                    GUILayout.Label("===============================", EditorStyles.boldLabel);

                    if (totalTokenCount == "0")
                    {
                        displayEmptyNFTsText();
                    }

                    if (this.nftList.Count >= 1)
                    {
                        selectedNFT = EditorGUILayout.Popup("NFT", selectedNFT, nftList.ToArray());
                        nft = nftList[selectedNFT];

                        if (selectedNFT != selectedNFTLast)
                        {
                            UpdateNFTData();
                            selectedNFTLast = selectedNFT;
                        }

                        GUILayout.Label("Selected NFT Details", EditorStyles.boldLabel);
                        GUILayout.Label("-------------------------", EditorStyles.boldLabel);

                        if (totalTokenCount == "0")
                        {
                            Debug.Log(totalTokenCount);
                            displayEmptyNFTsText();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("NFT index", EditorStyles.boldLabel, GUILayout.Width(130));
                            EditorGUILayout.SelectableLabel(nft, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("NFT Approved For", EditorStyles.boldLabel, GUILayout.Width(130));
                            EditorGUILayout.SelectableLabel(getApproved, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            GUILayout.EndHorizontal();

                            /*if (!approvedForAll)
                            {
                                GUILayout.Label("Change approving market place contract", EditorStyles.boldLabel);
                                GUILayout.Label("--------------------------------------", EditorStyles.boldLabel);
                            }*/
                            
                            if (publicKey == nftOwner)
                            {
                                /*if (!approvedForAll)
                                {   
                                    if (GUILayout.Button("Approve NFT", GUILayout.Width(150)))
                                    {
                                        approveNFT();
                                    }

                                    nftApprovingMarket =
                                EditorGUILayout.TextField("NFT approving for ", nftApprovingMarket, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                }*/

                                

                                if (marketPlaceContractAddress == getApproved)
                                {

                                    GUILayout.Label("Add NFT to market place", EditorStyles.boldLabel);
                                    GUILayout.Label("-------------------------", EditorStyles.boldLabel);

                                    if (alreadyInMarketPlace)
                                    {
                                        GUILayout.Label("Already in the market place of " + marketPlaceContractAddress, EditorStyles.boldLabel);

                                        if (GUILayout.Button("Remove NFT from Market Place", GUILayout.Width(190)))
                                        {
                                            withdrawNFT();
                                        }
                                    }
                                    else
                                    {
                                        tokenPrice =
                            EditorGUILayout.TextField("Token Price", tokenPrice, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                        erc20TokenAddress =
                                    EditorGUILayout.TextField("ERC20 address", erc20TokenAddress, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Selected ERC721", EditorStyles.boldLabel, GUILayout.Width(130));
                                        EditorGUILayout.SelectableLabel(contractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Market Place Contract Address", EditorStyles.boldLabel, GUILayout.Width(190));
                                        EditorGUILayout.SelectableLabel(marketPlaceContractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                        GUILayout.EndHorizontal();

                                        if (GUILayout.Button("Add NFT to Market Place", GUILayout.Width(170)))
                                        {
                                            addNFT();
                                        }
                                    }

                                }
                                else if (getApproved == "None")
                                {
                                    GUILayout.Label("Add NFT to market place", EditorStyles.boldLabel);
                                    GUILayout.Label("-------------------------", EditorStyles.boldLabel);
                                    GUILayout.Label("This NFT is not yet approved to any market place", EditorStyles.boldLabel);

                                }
                                else
                                {
                                    GUILayout.Label("Add NFT to market place", EditorStyles.boldLabel);
                                    GUILayout.Label("-------------------------", EditorStyles.boldLabel);
                                    GUILayout.Label("This NFT is approved to another market place", EditorStyles.boldLabel);

                                }
                            }
                            else
                            {
                                GUILayout.Label("Add NFT to market place", EditorStyles.boldLabel);
                                GUILayout.Label("-------------------------", EditorStyles.boldLabel);

                                GUILayout.Label("Game Developer is not the owner of this token", EditorStyles.boldLabel);
                            }
                        }
                    }
                }
            }


        }


    }

    public void updateNFTList()
    {
        for (int i = 0; i < Convert.ToInt32(totalTokenCount); i++)
        {
            if (this.nftList.Count == Convert.ToInt32(totalTokenCount))
            {
                break;
            }
            else
            {
                nftList.Add(i.ToString());
            }

        }
        selectedLast = selected;
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

    public async void DeployMarketPlaceContract()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        Debug.Log(account.Address);

        var deploymentMessage = new StandardTokenDeploymenMarketPlace { };

        var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeploymenMarketPlace>();
        var transactionReceiptDeployment = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

        SaveMarketPlaceContractData(transactionReceiptDeployment.ContractAddress);
    }

    public async void DeleteMarketPlaceContract()
    {
        if (FileManager.DeleteFile("Market Place Contract Data.dat"))
        {
            GUILayout.Label("Not yet created a Market Place.\nCreate a new Market Place First.", EditorStyles.boldLabel);
            Debug.Log("Successfuly deleted");
        }
        else
        {
            Debug.Log("Deleting Failed");
        }
    }

    private bool SaveMarketPlaceContractData(string contractAddressLocal)
    {
        MarketPlaceContractData marketPlaceContractData = new MarketPlaceContractData();
        PopulateMarketPlaceContractData(marketPlaceContractData, contractAddressLocal);

        if (FileManager.WriteToFile("Market Place Contract Data.dat", marketPlaceContractData.ToJson()))
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

    public void PopulateMarketPlaceContractData(MarketPlaceContractData marketPlaceContractData, string contractAddressLocal)
    {
        marketPlaceContractData.contractAddress = contractAddressLocal;
    }

    private bool LoadMarketPlaceContractData()
    {
        if (FileManager.LoadFromFile("Market Place Contract Data.dat", out var json))
        {
            MarketPlaceContractData marketPlaceContractData = new MarketPlaceContractData();
            marketPlaceContractData.LoadFromJson(json);

            LoadFromMarketPlaceContractData(marketPlaceContractData);
            return true;
        }
        else
        {
            displayEmptyTextMarketPlaceContractData();
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromMarketPlaceContractData(MarketPlaceContractData marketPlaceContractData)
    {
        // Set local variables
        marketPlaceContractAddress = marketPlaceContractData.contractAddress;

        // Set UI Text Fields
        GUILayout.BeginHorizontal();
        GUILayout.Label("Contract Address", EditorStyles.boldLabel, GUILayout.Width(130));
        EditorGUILayout.SelectableLabel(marketPlaceContractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        GUILayout.EndHorizontal();
    }

    public void displayEmptyTextMarketPlaceContractData()
    {
        GUILayout.Label("Not yet created a Market Place.\nCreate a new Market Place First.", EditorStyles.boldLabel);
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
            displayEmptyERC721Text();
            Debug.Log("No File");
            return false;
        }
    }

    public void displayEmptyERC721Text()
    {
        GUILayout.Label("Not yet deployed any ERC721 tokens.\nDeploy an ERC721 token first.", EditorStyles.boldLabel);
    }

    public void LoadFromERC721TokensData(ERC721TokensData eRC721TokensData)
    {
        this.contractAddresses = eRC721TokensData.eRC721TokenContractList;
        if (contractAddresses.Count >= 1)
        {
            contractAddress = contractAddresses[selected];

        }

    }

    public void UpdateTokenData()
    {
        getTotalTokenCount();
        getName();
        getSymbol();
        CheckItemExistance();
        getIsApprovedForAllFunctionERC721();
    }

    public async void getTotalTokenCount()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var functionMessage = new TotalTokenCountFunctionERC721()
        {
        };

        var handler = web3.Eth.GetContractQueryHandler<TotalTokenCountFunctionERC721>();
        var totalTokenCountLocal = await handler.QueryAsync<BigInteger>(contractAddress, functionMessage);

        totalTokenCount = totalTokenCountLocal.ToString();

        updateNFTList();
    }

    public async void getName()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var nameOfFunctionMessage = new NameFunctionERC721()
        {

        };

        var nameHandler = web3.Eth.GetContractQueryHandler<NameFunctionERC721>();
        var nameLocal = await nameHandler.QueryAsync<string>(contractAddress, nameOfFunctionMessage);

        name = nameLocal;
    }

    public async void getSymbol()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var symbolOfFunctionMessage = new SymbolFunctionERC721()
        {

        };

        var symbolHandler = web3.Eth.GetContractQueryHandler<SymbolFunctionERC721>();
        var symbolLocal = await symbolHandler.QueryAsync<string>(contractAddress, symbolOfFunctionMessage);

        symbol = symbolLocal;
    }

    public void UpdateNFTData()
    {
        if (totalTokenCount == "0")
        {
            Debug.Log(totalTokenCount);
            displayEmptyNFTsText();
        }
        else
        {
            CheckItemExistance();
            getGetApproved();
            getOwner();
            getIsApprovedForAllFunctionERC721();
        }
        
    }

    public void displayEmptyNFTsText()
    {
        GUILayout.Label("Not yet created any NFTs.\nMint an NFT First.", EditorStyles.boldLabel);
    }


    public async void getGetApproved()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var getApprovedFunctionMessage = new GetApprovedFunctionERC721()
        {
            TokenId = selectedNFT,
        };

        var queryHandler = web3.Eth.GetContractQueryHandler<GetApprovedFunctionERC721>();
        var getApprovedLocal = await queryHandler
            .QueryAsync<string>(contractAddress, getApprovedFunctionMessage)
            .ConfigureAwait(false);

        if (getApprovedLocal.ToString() != "0x0000000000000000000000000000000000000000")
        {
            getApproved = getApprovedLocal.ToString();
        }
        else
        {
            getApproved = "None";
        }
    }

    public async void getOwner()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var ownerOfFunctionMessage = new OwnerFunctionERC721()
        {
            TokenId = selectedNFT,
        };

        var ownerHandler = web3.Eth.GetContractQueryHandler<OwnerFunctionERC721>();
        var ownerLocal = await ownerHandler.QueryAsync<string>(contractAddress, ownerOfFunctionMessage);

        nftOwner = ownerLocal.ToString();
    }

    public async void CheckItemExistance()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var functionMessage = new CheckItemExistanceFunction()
        {
            tokenId = selectedNFT,
            tokenAddress = contractAddress,
        };

        var handler = web3.Eth.GetContractQueryHandler<CheckItemExistanceFunction>();

        var alreadyInMarketPlaceLocal = await handler.QueryAsync<bool>(marketPlaceContractAddress, functionMessage);

        alreadyInMarketPlace = alreadyInMarketPlaceLocal;
    }

    public async void approveNFT()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        string marketPlaceContractAddress = nftApprovingMarket;
        int sellingTokenId = selectedNFT;

        var approvetFunctionMessage = new ApproveFunctionERC721()
        {
            to = marketPlaceContractAddress,
            tokenId = sellingTokenId,
        };

        var approveHandler = web3.Eth.GetContractTransactionHandler<ApproveFunctionERC721>();
        var approved = await approveHandler.SendRequestAndWaitForReceiptAsync(contractAddress, approvetFunctionMessage);

        getGetApproved();
    }

    public async void addNFT()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var functionMessage = new AddItemFunction()
        {
            tokenId = selectedNFT,
            tokenAddress = contractAddress,
            price = Convert.ToUInt64(tokenPrice),
            eligibleBuyer = "0x0000000000000000000000000000000000000000",
            ERC20TokenAddress = erc20TokenAddress,
        };

        var handler = web3.Eth.GetContractTransactionHandler<AddItemFunction>();

        await handler.SendRequestAndWaitForReceiptAsync(marketPlaceContractAddress, functionMessage);

        Debug.Log("Succeeded");

        CheckItemExistance();
    }

    public async void withdrawNFT()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var functionMessage = new WithdrawItemFunction()
        {
            tokenId = selectedNFT,
            tokenAddress = contractAddress,
        };

        var handler = web3.Eth.GetContractTransactionHandler<WithdrawItemFunction>();

        await handler.SendRequestAndWaitForReceiptAsync(marketPlaceContractAddress, functionMessage);

        Debug.Log("Succeeded");

        CheckItemExistance();
    }

    public async void ApproveAll(bool approved)
    {
        var account = new Account(this.privateKey, this.chainId);
        var web3 = new Web3(account, this.rpcUrl);

        var functionMessage = new SetApprovalForAllFunctionERC721()
        {
            spender = marketPlaceContractAddress,
            approved = approved
        };

        var handler = web3.Eth.GetContractTransactionHandler<SetApprovalForAllFunctionERC721>();
        var receipt = await handler.SendRequestAndWaitForReceiptAsync(contractAddress, functionMessage);

        Debug.Log("Approve Successful");
        getIsApprovedForAllFunctionERC721();
    }

    public async void getIsApprovedForAllFunctionERC721()
    {
        var web3 = new Web3(rpcUrl);

        var functionMessage = new IsApprovedForAllFunctionERC721()
        {
            owner = publicKey,
            spender = marketPlaceContractAddress
        };

        var queryHandler = web3.Eth.GetContractQueryHandler<IsApprovedForAllFunctionERC721>();
        var result = await queryHandler
            .QueryAsync<bool>(contractAddress, functionMessage)
            .ConfigureAwait(false);

        approvedForAll = result;

        if (result)
        {
            getApproved = marketPlaceContractAddress;
        }
    }
}