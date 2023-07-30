using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class MarketPlace : MonoBehaviour
{
    public RawImage rawImage;
    public GameObject buyButton;
    public GameObject sellButton;
    public GameObject withdrawButton;
    public InputField priceInputField;

    public string transferingERC20TokenContractAddress;
    public string seller;

    private string nftURI;

    public string priceString; // input
    public BigInteger price;
    public Text priceText;

    public Text ownText;

    public string tokenIdString; // input
    public BigInteger tokenId;

    public string marketPlaceContractAddress;
    public string ERC721TokenContractAddress = "0x802dc38921d94b9256b0b799ddcf7e063e96465f";
    public string ERC20TokenContractAddress ; // input

    public WalletConnectClient walletConnectClient;
    public GameManagerERC721 gameManagerERC721;
    public GameManagerERC20 gameManagerERC20;
    public GameManagerMarketPlace GameManagerMarketPlace;


    // Start is called before the first frame update
    async void Start()
    {
        // Initial state of a marketplace item
        // Make buttons and texts invisible
        buyButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        withdrawButton.gameObject.SetActive(false);
        priceInputField.gameObject.SetActive(false);

        ownText.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
    }

    public async void LoadData()
    {
        // "tokenIdString" is used to get token id input from inspector, there for it needs to be convverted to a BigInteger to use it in methods
        /*this.tokenId = BigInteger.Parse(this.tokenIdString);*/

        // Load marketplace contract address 
        // TODO : Need to handle situation where marketplace is not initialized
        LoadMarketPlaceContractData();

        // Load image and display
        this.nftURI = await gameManagerERC721.getTokenURIOfAnNFT(this.tokenId, this.ERC721TokenContractAddress);
        StartCoroutine(GetTexture());

        SetMarketPlaceItemState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(this.nftURI);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.texture = myTexture;
        }
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
            Debug.Log("No File");
            return false;
        }
    }

    private void LoadFromMarketPlaceContractData(MarketPlaceContractData marketPlaceContractData)
    {
        // Set local variables
        marketPlaceContractAddress = marketPlaceContractData.contractAddress;
    }

    private async void SetMarketPlaceItemState()
    {
        // Markeplace item state
        string owner = await gameManagerERC721.getOwnerOfNFT(this.tokenId, this.ERC721TokenContractAddress);
        bool isExist = await GameManagerMarketPlace.CheckItemExistance(this.tokenId, this.ERC721TokenContractAddress, this.marketPlaceContractAddress);
        Debug.Log(this.tokenId);
        Debug.Log(owner);
        Debug.Log(isExist);
        if (isExist)
        {
            GetItemOutputDTO getItemOutputDTO = await GameManagerMarketPlace.getSaleItemDetails(this.tokenId, this.ERC721TokenContractAddress, this.marketPlaceContractAddress);

            this.price = getItemOutputDTO.Price;
            this.transferingERC20TokenContractAddress = getItemOutputDTO.ERC20TokenAddress;
            this.seller = getItemOutputDTO.seller;
            priceText.text = this.price.ToString();

            priceText.gameObject.SetActive(true);

            if (owner == walletConnectClient.accountPublicKey)
            {
                buyButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(false);
                withdrawButton.gameObject.SetActive(true);
                priceInputField.gameObject.SetActive(false);

                ownText.text = "Own";
                ownText.gameObject.SetActive(true);
            }
            else
            {
                buyButton.gameObject.SetActive(true);
                sellButton.gameObject.SetActive(false);
                withdrawButton.gameObject.SetActive(false);
                priceInputField.gameObject.SetActive(false);

                ownText.gameObject.SetActive(false);
            }
        }
        else
        {
            priceText.gameObject.SetActive(false);

            if (owner == walletConnectClient.accountPublicKey)
            {
                buyButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(true);
                withdrawButton.gameObject.SetActive(false);
                priceInputField.gameObject.SetActive(true);

                ownText.text = "Own";
                ownText.gameObject.SetActive(true);
            }
            else
            {
                buyButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(false);
                withdrawButton.gameObject.SetActive(false);
                priceInputField.gameObject.SetActive(false);

                ownText.text = "Can't find in\nmarketplace";
                ownText.gameObject.SetActive(true);
            }
        }
    }

    public async void BuyNFT()
    {
        string spender = this.marketPlaceContractAddress;
        BigInteger amount = this.price; 
        string transferingERC20TokenContractAddress = this.transferingERC20TokenContractAddress;

        // Check whether approved by game player

        BigInteger approvedPrice = await gameManagerERC20.getAllowance(walletConnectClient.accountPublicKey, spender, this.ERC20TokenContractAddress);

        if (approvedPrice<= amount)
        {
            // push meesage to approve erc20 function
            // Approve ERC20 Token to claim
            await gameManagerERC20.PlayerApproveERC20(spender, amount, transferingERC20TokenContractAddress);
            /*Debug.Log("Before delay");
            await Task.Delay(7000);
            Debug.Log("After delay");*/
        }

        // Buy NFT from market place
        await GameManagerMarketPlace.PlayerBuyNFT(this.tokenId, this.ERC721TokenContractAddress, transferingERC20TokenContractAddress,this.seller, walletConnectClient.accountPublicKey, this.marketPlaceContractAddress);

        SetMarketPlaceItemState();
    }

    public async void SellNFT()
    {
        string spender = this.marketPlaceContractAddress;
        this.priceString = this.priceInputField.text;

        // Check whether approved for all by game player

        bool isApprovedForAll = await gameManagerERC721.getIsApprovedForAllFunctionERC721(walletConnectClient.accountPublicKey, spender, this.ERC721TokenContractAddress);

        if (!isApprovedForAll)
        {
            // push meesage to approve all function
            await gameManagerERC721.PlayerApproveAllNFTs(spender, true, walletConnectClient.accountPublicKey, this.ERC721TokenContractAddress);
            Debug.Log("Before delay");
            await Task.Delay(7000);
            Debug.Log("After delay");
        }

        // Add Item to market place
        await GameManagerMarketPlace.PlayerAddNFT(this.tokenId, this.ERC721TokenContractAddress, BigInteger.Parse(this.priceString), this.ERC20TokenContractAddress, walletConnectClient.accountPublicKey, this.marketPlaceContractAddress);

        SetMarketPlaceItemState();
    }

    public async void WithdrawNFT()
    {
        string spender = this.marketPlaceContractAddress;
        string transferingERC20TokenContractAddress = this.transferingERC20TokenContractAddress;

        // Withdraw Item From market place
        await GameManagerMarketPlace.PlayerWithdrawNFT(this.tokenId, this.ERC721TokenContractAddress, this.ERC20TokenContractAddress, walletConnectClient.accountPublicKey, this.marketPlaceContractAddress);

        SetMarketPlaceItemState();
    }

}
