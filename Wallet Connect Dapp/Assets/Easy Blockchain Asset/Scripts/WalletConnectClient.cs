using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using WalletConnectSharp.Unity;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Models.Ethereum;

public class WalletConnectClient : WalletConnectActions
{

    public Text publicKeyText;
    public Text connectingText;

    public GameObject connectButton;
    public GameObject disconnectButton;

    public GameObject QRImage;

    public WalletConnect WalletConnectInstance;

    public string accountPublicKey;

    private int chainId;
    private string rpcUrl;

    // Start is called before the first frame update
    void Start()
    {
        this.chainId = Constants.AVALANCHE_TESTNET_CHAIN_ID;
        this.rpcUrl = Constants.AVALANCHE_TESTNET_RPC_URL;
    }

    public async void ConnectWallet()
    {
        var deviceType = SystemInfo.deviceType;
        Debug.Log(deviceType);
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            MobileWalletConnectButtonTrigger();
        }
        else
        {
            DesktopWalletConnectButtonTrigger();
        }
    }

    public async void MobileWalletConnectButtonTrigger()
    {
        Debug.Log("Wallet Connect Mobile Button Pressed");

        WalletConnectInstance.connectOnStart = true;
        WalletConnectInstance.gameObject.SetActive(true);

        publicKeyText.text = "Connecting...";

        WalletConnectInstance.OpenDeepLink(WalletConnectInstance.SelectedWallet);
    }

    public async void DesktopWalletConnectButtonTrigger()
    {
        Debug.Log("Wallet Connect Desktop Button Pressed");

        publicKeyText.gameObject.SetActive(false);
        connectButton.SetActive(false);

        connectingText.gameObject.SetActive(true);
        connectingText.text = "Scan QR to Connect wallet";

        WalletConnectInstance.gameObject.SetActive(true);
        QRImage.gameObject.SetActive(true);

        await WalletConnectInstance.Connect();
    }

    public async void WalletConnectHandler(WCSessionData data)
    {

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            MobileWalletConnectHandler(data);
        }
        else
        {
            DesktopWalletConnectHandler(data);
        }
    }

    public async void MobileWalletConnectHandler(WCSessionData data)
    {
        Debug.Log("Mobile Wallet connection Established");

        connectButton.SetActive(false);

        disconnectButton.SetActive(true);

        // Extract wallet address from the Wallet Connect Session data object.
        this.accountPublicKey = data.accounts[0].ToLower();

        publicKeyText.text = this.accountPublicKey.Substring(0,10) + "...";

        Debug.Log(this.accountPublicKey);
    }

    public async void DesktopWalletConnectHandler(WCSessionData data)
    {
        Debug.Log("Desktop Wallet connection Established");

        QRImage.gameObject.SetActive(false);

        connectingText.gameObject.SetActive(false);

        disconnectButton.SetActive(true);

        // Extract wallet address from the Wallet Connect Session data object.
        this.accountPublicKey = data.accounts[0].ToLower();

        publicKeyText.text = this.accountPublicKey.Substring(0, 10) + "...";
        publicKeyText.gameObject.SetActive(true);

        Debug.Log(this.accountPublicKey);
    }

    public async void WalletDisconnectHandler()
    {
        Debug.Log("Wallet connection disconnected");

        connectButton.SetActive(true);
        disconnectButton.SetActive(false);

        publicKeyText.text = "";
    }

    public async void DisconnectWallet()
    {

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            MobileDisconnectWallet();
        }
        else
        {
            DesktopDisconnectWallet();
        }
    }

    public async void MobileDisconnectWallet()
    {
        Debug.Log("Mobile Wallet connection disconnected");

        // Disconnect wallet subscription.
        publicKeyText.text = "Disconnecting...";

        /*await WalletConnectInstance.SessionDisconnect();*/
        // CLear out the session so it is re-establish on sign-in.
        WalletConnectInstance.CLearSession();
        WalletConnectInstance.CloseSession(false);

        publicKeyText.text = "";

        connectButton.SetActive(true);
        disconnectButton.SetActive(false);

        // Close out the application.
        /*Application.Quit();*/
        WalletConnectInstance.gameObject.SetActive(false);
    }

    public async void DesktopDisconnectWallet()
    {
        Debug.Log("Desktop Wallet connection disconnected");

        // Disconnect wallet subscription.
        publicKeyText.text = "Disconnecting...";

        /*await WalletConnectInstance.SessionDisconnect();*/
        // CLear out the session so it is re-establish on sign-in.
        WalletConnectInstance.CLearSession();
        WalletConnectInstance.CloseSession(false);


        publicKeyText.text = "";

        connectButton.SetActive(true);
        disconnectButton.SetActive(false);

        // Close out the application.
        /*Application.Quit();*/
        /*WalletConnectInstance.gameObject.SetActive(false);*/
    }

    public async Task<bool> SendTransactionCustom(string hexData, string fromAddress, string toAddress, int chainId)
    {
        /*  ********************************************************************
         *  0xa9059cbb000000000000000000000000dcc85a30cc5ea5dc96a01aa52f3dd48710b8ba6c0000000000000000000000000000000000000000000000000000000000989680
         *  taken from metamask transfer function hex data
         *  completed
         *  *******************************************************************
         *  
         *  approve function erc20
         *  0x095ea7b30000000000000000000000008d65e9cc07f26f826d1de429bd335f9ea9ccfdc2000000000000000000000000000000000000000000000000000000e8d4a51000
         *  
         *  buy item market place
         *  0x206b2cfc000000000000000000000000000000000000000000000000000000000000000000000000000000000000000023675b38b8e2d12061a99f8d6e6b22bdbbc3ba510000000000000000000000000177e32ba872c1a5efa7847da8b612c8bfb55ef0000000000000000000000000704a620b99f619911cacbb39c39f0e14e80bd4e5
         * */
        var transaction = new TransactionData()
        {
            data = hexData,
            from = fromAddress,
            to = toAddress,
            chainId = chainId
        };

        var results = await SendTransaction(transaction);
        Debug.Log(results);
        return true;
    }

    /*public async void TransferErc20()
    {
        string receiverPublicKey = "dcc85a30cc5ea5dc96a01aa52f3dd48710b8ba6c";
        int receivingTokenAmount = 10000000;


        string erc20ContractAddress = "0x1d9254b8d6A7c7Bc10Ba42Ef57A8593C5392DaB3";
        int chainId = this.chainId;
        string functionSignature = "transfer(address,uint256)";
        var functionMessage = new TransferFunction()
        {
            To = receiverPublicKey,
            TokenAmount = receivingTokenAmount
        };

        byte[] value = Encoding.ASCII.GetBytes(functionSignature);
        var hash = new Sha3Keccack().CalculateHash(value);
        var methodSignature = hash.ToHex();
        var methodSignature4bytes = methodSignature.Substring(0, 8);

        var abiEncode = new ABIEncode();
        var parameterEncoding = abiEncode.GetABIParamsEncoded(functionMessage).ToHex();

        var completeDataHex = "0x" + methodSignature4bytes + parameterEncoding;

        await SendTransactionCustom(completeDataHex, accountPublicKey, erc20ContractAddress, chainId);
    }*/

    /*public async void BuyNFT()
    {
        // Approve ERC20 Token to claim
        // Using ERC20 Token deployer account - Developer
        await ApproveERC20ToBuy();
        // Claim ERC20
        await BuyApprovedNFT();
    }

    public async Task<bool> ApproveERC20ToBuy()
    {
        *//*0x095ea7b30000000000000000000000008d65e9cc07f26f826d1de429bd335f9ea9ccfdc20000000000000000000000000000000000000000000000000000000000002710
         *0x095ea7b30000000000000000000000008d65e9cc07f26f826d1de429bd335f9ea9ccfdc20000000000000000000000000000000000000000000000000000000000002710
         *//*
        string spender = "0x8D65E9cC07f26f826D1De429Bd335F9eA9ccfDc2";
        var amount = 10000;

        string erc20ContractAddress = "0x27c245eFbB4ed4441457704cE571291B3FA101e3";
        int chainId = this.chainId;
        string functionSignature = "approve(address,uint256)";
        var functionMessage = new ApproveFunction()
        {
            spender = spender,
            amount = amount
        };
 
        byte[] value = Encoding.ASCII.GetBytes(functionSignature);
        var hash = new Sha3Keccack().CalculateHash(value);
        var methodSignature = hash.ToHex();
        var methodSignature4bytes = methodSignature.Substring(0, 8);

        var abiEncode = new ABIEncode();
        var parameterEncoding = abiEncode.GetABIParamsEncoded(functionMessage).ToHex();

        var completeDataHex = "0x" + methodSignature4bytes + parameterEncoding;
        Debug.Log(completeDataHex);
        await SendTransactionCustom(completeDataHex, accountPublicKey, erc20ContractAddress, chainId);
        Debug.Log("Approve Successful");
        return true;
    }
    private string gameDeveloperPrivateKey = "";
    public async Task<bool> BuyApprovedNFT()
    {
        *//*0x206b2cfc000000000000000000000000000000000000000000000000000000000000000000000000000000000000000023675b38b8e2d12061a99f8d6e6b22bdbbc3ba5100000000000000000000000027c245efbb4ed4441457704ce571291b3fa101e3000000000000000000000000dcc85a30cc5ea5dc96a01aa52f3dd48710b8ba6c
         *0x206b2cfc000000000000000000000000000000000000000000000000000000000000000000000000000000000000000023675b38b8e2d12061a99f8d6e6b22bdbbc3ba5100000000000000000000000027c245efbb4ed4441457704ce571291b3fa101e3000000000000000000000000dcc85a30cc5ea5dc96a01aa52f3dd48710b8ba6c
         *//*
        Debug.Log("started");
        var account = new Account(this.gameDeveloperPrivateKey, this.chainId);

        var transferingTokenID = 0;
        string ERC721TokenContractAddress = "0x23675B38B8E2D12061a99f8d6e6b22BdBBC3bA51";
        string ERC20TokenContractAddress = "0x27c245eFbB4ed4441457704cE571291B3FA101e3";
        string gameDeveloperPublicKey = account.Address;

        string marketPlaceContractAddress = "0x8D65E9cC07f26f826D1De429Bd335F9eA9ccfDc2";
        int chainId = this.chainId;
        string functionSignature = "buyItem(uint256,address,address,address)";
        var functionMessage = new BuyItemFunctionERC721()
        {
            tokenId = transferingTokenID,
            tokenAddress = ERC721TokenContractAddress,
            ERC20TokenAddress = ERC20TokenContractAddress,
            ERC20TokenOwnerAddress = gameDeveloperPublicKey
        };

        byte[] value = Encoding.ASCII.GetBytes(functionSignature);
        var hash = new Sha3Keccack().CalculateHash(value);
        var methodSignature = hash.ToHex();
        var methodSignature4bytes = methodSignature.Substring(0, 8);

        var abiEncode = new ABIEncode();
        var parameterEncoding = abiEncode.GetABIParamsEncoded(functionMessage).ToHex();

        var completeDataHex = "0x" + methodSignature4bytes + parameterEncoding;
        Debug.Log(completeDataHex);
        await SendTransactionCustom(completeDataHex, accountPublicKey, marketPlaceContractAddress, chainId);
        Debug.Log("Buy Successful");
        return true;
    }*/
}

