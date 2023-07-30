using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

public class PopulateGrid : MonoBehaviour
{
    public GameObject prefab;
    public BigInteger numberToCreate;

    public string ERC721TokenContractAddress = "0x45032617d18a86780b9465fb829166e13b319463";
    public string ERC20TokenContractAddress = "0x79cbd3b2a2fefdd8ffd91bafaa2be8793936621f0";

    public WalletConnectClient walletConnectClient;
    public GameManagerERC721 gameManagerERC721;
    public GameManagerERC20 gameManagerERC20;
    public GameManagerMarketPlace GameManagerMarketPlace;

    // Start is called before the first frame update
    async void Start()
    {
        numberToCreate = await this.gameManagerERC721.getTotalTokenCount(this.ERC721TokenContractAddress);
        Populate();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Populate()
    {
        GameObject newObj;

        for (BigInteger i = 0; i < numberToCreate; i++)
        {
            newObj = (GameObject)Instantiate(prefab, transform);

            var marketPlace = newObj.GetComponent<MarketPlace>();
            marketPlace.walletConnectClient = walletConnectClient;
            marketPlace.gameManagerERC721 = gameManagerERC721;
            marketPlace.gameManagerERC20 = gameManagerERC20;
            marketPlace.GameManagerMarketPlace = GameManagerMarketPlace;
            marketPlace.tokenId = i;
            marketPlace.LoadData();
        }
    }
}
