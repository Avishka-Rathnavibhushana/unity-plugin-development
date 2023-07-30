using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

public class ERC721TokenExplorerEditor : EditorWindow
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

    string totalTokenCount;
    string balance;
    string name;
    string symbol;

    string mintingTokenImage;
    /*string nftApprovingMarket;*/
    GameObject mappingObject;

    int selectedNFT = 0;
    int selectedNFTLast = -1;
    public List<string> nftList = new List<string>();
    string nft;

    string nftOwner;
    string nftUri;
    string getApproved;


    /*string marketPlaceContractAddress;
    string tokenPrice;
    string erc20TokenAddress;*/

    public PinataClient pinataClient;

    [MenuItem("Window/Kaiju/ERC721 Token/ERC721 Token Explorer")]
    public static void ShowWindow()
    {
        GetWindow<ERC721TokenExplorerEditor>("ERC721 Token Explorer");
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
            LoadERC721TokensJsonData();
            if (this.contractAddresses.Count >= 1)
            {

                selected = EditorGUILayout.Popup("Contracts", selected, contractAddresses.ToArray());
                contractAddress = contractAddresses[selected];

                if (selected!=selectedLast)
                {
                    UpdateTokenData();
                    this.nftList = new List<string>();
                    selectedNFT = 0;
                    selectedNFTLast = -1;
                }

                GUILayout.Label("Selected Contract Details", EditorStyles.boldLabel);
                GUILayout.Label("=========================", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Collection total NFT count", EditorStyles.boldLabel, GUILayout.Width(200));
                EditorGUILayout.SelectableLabel(totalTokenCount, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Collection Owner's NFT balance", EditorStyles.boldLabel, GUILayout.Width(200));
                EditorGUILayout.SelectableLabel(balance, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
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

                GUILayout.Label("Token Minting", EditorStyles.boldLabel);
                GUILayout.Label("-------------", EditorStyles.boldLabel);

                mintingTokenImage =
                EditorGUILayout.TextField("Minting Token Image ", mintingTokenImage, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Add jpg Image", GUILayout.Width(170)))
                {
                    openFileExplorerERC721();
                }

                GUILayout.Label("Select an image before minting", EditorStyles.boldLabel);

                if (GUILayout.Button("Mint Token", GUILayout.Width(150)))
                {
                    mintNFT();
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
                        displayEmptyNFTsText();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("NFT index", EditorStyles.boldLabel, GUILayout.Width(130));
                        EditorGUILayout.SelectableLabel(nft, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("NFT Owner", EditorStyles.boldLabel, GUILayout.Width(130));
                        EditorGUILayout.SelectableLabel(nftOwner, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("NFT URI", EditorStyles.boldLabel, GUILayout.Width(130));
                        EditorGUILayout.SelectableLabel(nftUri, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("NFT Approved For", EditorStyles.boldLabel, GUILayout.Width(130));
                        EditorGUILayout.SelectableLabel(getApproved, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Adding a prefab to nft", EditorStyles.boldLabel);
                        GUILayout.Label("-------------------------", EditorStyles.boldLabel);

                        GameObject pObj = (GameObject)EditorGUILayout.ObjectField("Prefab", mappingObject, typeof(GameObject), false,GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        if (pObj != null && PrefabUtility.GetPrefabType(pObj) == PrefabType.Prefab)
                        {
                            mappingObject = pObj;
 }
                        else
                        {
                            mappingObject = null;
                        }

                        if (GUILayout.Button("Save Prefab", GUILayout.Width(170)))
                        {
                            onPrefabSave();
                        }

                        if (mappingObject != null)
                        {
                            if (GUILayout.Button("Remove Prefab", GUILayout.Width(170)))
                            {
                                onPrefabRemove();
                            }
                        }
                        

                        /*GUILayout.Label("Change approving market place contract", EditorStyles.boldLabel);
                        GUILayout.Label("--------------------------------------", EditorStyles.boldLabel);*/

                        /* bool marketPlaceExist = LoadMarketPlaceContractData();

                         if (marketPlaceExist)
                         {


                             if (GUILayout.Button("Approve NFT", GUILayout.Width(150)))
                             {
                                 approveNFT();
                             }

                             nftApprovingMarket =
                         EditorGUILayout.TextField("NFT approving for ", nftApprovingMarket, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                             if(marketPlaceContractAddress== getApproved)
                             {
                                 GUILayout.Label("Add NFT to market place", EditorStyles.boldLabel);
                                 GUILayout.Label("-------------------------", EditorStyles.boldLabel);

                                 tokenPrice =
                         EditorGUILayout.TextField("Token Price", tokenPrice, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                 erc20TokenAddress =
                             EditorGUILayout.TextField("ERC20 address", erc20TokenAddress, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                                 GUILayout.BeginHorizontal();
                                 GUILayout.Label("Selected ERC721", EditorStyles.boldLabel, GUILayout.Width(130));
                                 EditorGUILayout.SelectableLabel(contractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                 GUILayout.EndHorizontal();

                                 // Set UI Text Fields
                                 GUILayout.BeginHorizontal();
                                 GUILayout.Label("Market Place Contract Address", EditorStyles.boldLabel, GUILayout.Width(190));
                                 EditorGUILayout.SelectableLabel(marketPlaceContractAddress, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                 GUILayout.EndHorizontal();

                                 if (GUILayout.Button("Add NFT to Market Place", GUILayout.Width(170)))
                                 {
                                     addNFT();
                                 }
                             }
                             else
                             {
                                 GUILayout.Label("Add NFT to market place", EditorStyles.boldLabel);
                                 GUILayout.Label("-------------------------", EditorStyles.boldLabel);
                                 GUILayout.Label("This NFT is approved to another market place", EditorStyles.boldLabel);

                             }

                         }*/
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
        getBalance();
        getName();
        getSymbol();
    }

    public async void getBalance()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var balanceOfFunctionMessage = new BalanceOfFunctionERC721()
        {
            Owner = publicKey,
        };

        var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunctionERC721>();
        var balanceLocal = await balanceHandler.QueryAsync<BigInteger>(contractAddress, balanceOfFunctionMessage);

        balance = balanceLocal.ToString();
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
        getBalance();
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

    
    public async void mintNFT()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        string tokenImage = mintingTokenImage;
            
        var mintFunctionMessage = new MintFunctionERC721()
        {
            player = publicKey,
            tokenURI = tokenImage,
        };

        var mintHandler = web3.Eth.GetContractTransactionHandler<MintFunctionERC721>();
        var minted = await mintHandler.SendRequestAndWaitForReceiptAsync(contractAddress, mintFunctionMessage);

        getTotalTokenCount();
    }

    public void UpdateNFTData()
    {
        if (totalTokenCount == "0")
        {
            displayEmptyNFTsText();
        }
        else
        {
            getOwner();
            getTokenURI();
            getGetApproved();
            LoadCollectionNFTData();
        }
        
    }

    public void displayEmptyNFTsText()
    {
        GUILayout.Label("Not yet created any NFTs.\nMint an NFT First.", EditorStyles.boldLabel);
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

    public async void getTokenURI()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var uriOfFunctionMessage = new TokenURIFunctionERC721()
        {
            TokenId = selectedNFT,
        };

        var queryHandler = web3.Eth.GetContractQueryHandler<TokenURIFunctionERC721>();
        var uriLocal = await queryHandler
            .QueryAsync<string>(contractAddress, uriOfFunctionMessage)
            .ConfigureAwait(false);

        nftUri = uriLocal.ToString();
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

    /*public async void approveNFT()
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
    }*/

    public async void openFileExplorerERC721()
    {
        string path = EditorUtility.OpenFilePanel("Show all images", "", "jpg");

        PinataClient pinataClient = new PinataClient();
        pinataClient.PINATA_API_KEY = "d3d103dec208c5e0217e";
        pinataClient.PINATA_SECRET_API_KEY = "089e6a41b9494d6211c8d45bbd2e47adb7bfa45271164551432342024fb3187c";
        string accessLink = await pinataClient.UploadDataFromFile(path);

        mintingTokenImage = accessLink;
    }

    /*private bool LoadMarketPlaceContractData()
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
    }

    public void displayEmptyTextMarketPlaceContractData()
    {
        GUILayout.Label("Not yet created a Market Place.\nCreate a new Market Place First.", EditorStyles.boldLabel);
    }*/

    /*public async void addNFT()
    {
        var account = new Account(privateKey, chainId);
        var web3 = new Web3(account, rpcUrl);

        var addNftFunctionMessage = new AddItemFunction()
        {
            tokenId = selectedNFT,
            tokenAddress = contractAddress,
            price = Convert.ToUInt64(tokenPrice),
            eligibleBuyer = "0x0000000000000000000000000000000000000000",
            ERC20TokenAddress = erc20TokenAddress,
        };

        var addNftHandler = web3.Eth.GetContractTransactionHandler<AddItemFunction>();

        await addNftHandler.SendRequestAndWaitForReceiptAsync(marketPlaceContractAddress, addNftFunctionMessage);

        Debug.Log("Succeeded");
    }*/

   

    public void onPrefabSave()
    {
        string tag = contractAddress + "#" + nft;
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            bool found = false;
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            int removedTagIndex = -1;
            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue=="" && removedTagIndex == -1)
                {
                    removedTagIndex = i;
                }
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    found = true; 
                    break;
                }
            }
 
            if (!found)
            {
                int updateingIndex = removedTagIndex;
                if (removedTagIndex == -1)
                {
                    updateingIndex = tags.arraySize;
                    tags.InsertArrayElementAtIndex(updateingIndex);
                }
                tags.GetArrayElementAtIndex(updateingIndex).stringValue = tag;
                so.ApplyModifiedProperties();
                so.Update();
            }
            mappingObject.tag = tag;
            SaveCollectionNFTData(contractAddress, nft, tag);
        }
    }

    public void onPrefabRemove()
    {
        mappingObject.tag = "Untagged";

        string tag = contractAddress + "#" + nft;
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            bool found = false;
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    tags.GetArrayElementAtIndex(i).stringValue = "";
                    so.ApplyModifiedProperties();
                    so.Update();
                    break;
                }
            }
        }

        tag = "";

        SaveCollectionNFTData(contractAddress, nft, tag);

        mappingObject = null;

    }

    private bool SaveCollectionNFTData(string contractAddress, string id, string prefabTag)
    {
        CollectionNFTData collectionNFTData = new CollectionNFTData();

        PopulatCollectionNFTData(collectionNFTData, contractAddress, id, prefabTag);

        if (FileManager.WriteToFile("Collection NFT Data.dat", collectionNFTData.ToJson()))
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

    public void PopulatCollectionNFTData(CollectionNFTData collectionNFTData, string contractAddress, string id,string prefabTag)
    {
        SerializableDictionary<string, string> collectionNFTList;

        CollectionNFTData savedCollectionNFTData = ReturnCollectionNFTData();

        string key = contractAddress + "#" + id;
        if (savedCollectionNFTData.collectionNFTList == null)
        {
            collectionNFTList = new SerializableDictionary<string, string>();
        }
        else
        {
            collectionNFTList = savedCollectionNFTData.collectionNFTList;
            if (prefabTag == "")
            {
                if (collectionNFTList.ContainsKey(key))
                {
                    collectionNFTList.Remove(collectionNFTList[key]);
                }
            }
            else
            {
                if (collectionNFTList.ContainsKey(key))
                {
                    collectionNFTList[key] = prefabTag;
                }
                else
                {
                    collectionNFTList.Add(key, prefabTag);
                }
            }
            
        }

        collectionNFTData.collectionNFTList = collectionNFTList;
    }

    private CollectionNFTData ReturnCollectionNFTData()
    {
        if (FileManager.LoadFromFile("Collection NFT Data.dat", out var json))
        {
            CollectionNFTData collectionNFTData = new CollectionNFTData();
            collectionNFTData.LoadFromJson(json);

            return collectionNFTData;
        }
        else
        {
            Debug.Log("No File");
            return null;
        }
    }

    private bool LoadCollectionNFTData()
    {
        if (FileManager.LoadFromFile("Collection NFT Data.dat", out var json))
        {
            CollectionNFTData collectionNFTData = new CollectionNFTData();
            collectionNFTData.LoadFromJson(json);

            LoadFromCollectionNFTData(collectionNFTData);
            return true;
        }
        else
        {
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromCollectionNFTData(CollectionNFTData collectionNFTData)
    {
        string key = contractAddress + "#" + nft;
        if (collectionNFTData.collectionNFTList.ContainsKey(key))
        {
            try
            {
                var taggedObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g => g.tag == collectionNFTData.collectionNFTList[key]).ToList();
                if (taggedObjects.Count >= 1)
                {
                    GameObject pObj = taggedObjects[taggedObjects.Count - 1];
                    if (pObj != null && PrefabUtility.GetPrefabType(pObj) == PrefabType.Prefab)
                    {
                        mappingObject = pObj;
                    }
                    else
                    {
                        mappingObject = null;
                    }
                }
                else
                { 
                    mappingObject = null;
                }
                
            }
            catch (UnityException ex)
            {
                mappingObject = null;
                Debug.Log(ex);
            }
        }
        else
        {
            mappingObject = null;
        }
    }

}