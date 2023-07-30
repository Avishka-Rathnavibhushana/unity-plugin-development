// contracts/MarketPlace.sol
// SPDX-License-Identifier: MIT
// author: Mintzilla.io

pragma solidity ^0.8.7;

import "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import "@openzeppelin/contracts-upgradeable/token/ERC721/ERC721Upgradeable.sol";
import "@openzeppelin/contracts-upgradeable/utils/CountersUpgradeable.sol";
import "@openzeppelin/contracts-upgradeable/access/OwnableUpgradeable.sol";
import "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import "@openzeppelin/contracts-upgradeable/utils/math/SafeMathUpgradeable.sol";
import "@openzeppelin/contracts-upgradeable/interfaces/IERC2981Upgradeable.sol";
import "@openzeppelin/contracts/token/ERC20/IERC20.sol";

contract Marketplace is Initializable, OwnableUpgradeable, UUPSUpgradeable {
    using SafeMathUpgradeable for uint256;

    address payable public commissionRecipient;
    uint256 public commissionFactor;
    uint256 public commissionMaxLimit;

    /**
      * @dev Set initial state values
      * @param commissionRecipientValue - Account address of the commission recipient
      * @param commissionFactorValue - 100% -> 10000
      */
    function initialize(address payable commissionRecipientValue, uint256 commissionFactorValue) public initializer {
        commissionMaxLimit = 1000;
        require(commissionFactorValue <= commissionMaxLimit, "Cannot charge commision more than 10%");
        __Ownable_init();
        __UUPSUpgradeable_init();
        commissionRecipient = commissionRecipientValue;
        commissionFactor = commissionFactorValue;
    }

    struct MarketItem {
        bytes32 orderId;
        address tokenAddress;
        uint256 tokenId;
        address payable seller;
        address payable eligibleBuyer;
        uint256 price;
        address ERC20TokenAddress;
    }

    mapping(address => mapping(uint256 => MarketItem)) openSales;

    event ItemAdded(
        bytes32 orderId,
        address tokenAddress,
        uint256 tokenId,
        address seller,
        uint256 price,
        address eligibleBuyer,
        address ERC20TokenAddress
    );

    event ItemSold(
        bytes32 orderId,
        address tokenAddress,
        uint256 tokenId,
        address seller,
        address buyer,
        uint256 price,
        address commissionRecipient,
        uint256 commissionAmount,
        address royaltyRecipient,
        uint256 royaltyAmount
    );

    event ItemWithdrawed(
        bytes32 orderId,
        address tokenAddress,
        uint256 tokenId,
        address seller,
        address ERC20TokenAddress
    );

    event ItemUpdated(
        bytes32 orderId,
        address tokenAddress,
        uint256 tokenId,
        address seller,
        uint256 price,
        address eligibleBuyer,
        address ERC20TokenAddress
    );

    modifier OnlyTokenOwner(address tokenAddress, uint256 tokenId) {
        IERC721Upgradeable tokenContract = IERC721Upgradeable(tokenAddress);
        require(tokenContract.ownerOf(tokenId) == msg.sender, "Only token owner can perform this action");
        _;
    }

    modifier HasTransferApproval(address tokenAddress, uint256 tokenId) {
        IERC721Upgradeable tokenContract = IERC721Upgradeable(tokenAddress);
        require((tokenContract.getApproved(tokenId) == address(this)) || tokenContract.isApprovedForAll(tokenContract.ownerOf(tokenId), address(this)), "Token does not have transfer approval");
        _;
    }

    modifier ItemExists(address tokenAddress, uint256 tokenId) {
        require(openSales[tokenAddress][tokenId].tokenId == tokenId, "Could not find item");
        _;
    }

    /**
      * @dev Add new Item to sell
      * @param tokenId - Id of the NFT
      * @param tokenAddress - Address of the NFT contract
      * @param eligibleBuyer - Address of eligible buyer
      */
    function addItem(uint256 tokenId, address tokenAddress, uint256 price, address eligibleBuyer, address ERC20TokenAddress) 
    HasTransferApproval(tokenAddress, tokenId)
    OnlyTokenOwner(tokenAddress, tokenId)
    virtual external {
        require(price > 0, "Price should be greater than zero");

        bytes32 orderId = keccak256(
            abi.encodePacked(
                block.timestamp,
                payable(msg.sender),
                tokenId,
                tokenAddress,
                price,
                ERC20TokenAddress
            )
        );

        MarketItem memory saleItem = MarketItem(
            orderId,
            tokenAddress,
            tokenId,
            payable(msg.sender),
            payable(eligibleBuyer),
            price,
            ERC20TokenAddress
        );

        openSales[tokenAddress][tokenId] = saleItem;

        assert(openSales[tokenAddress][tokenId].tokenId == saleItem.tokenId);
        
        emit ItemAdded(
            orderId,
            tokenAddress,
            tokenId,
            msg.sender,
            price,
            eligibleBuyer,
            ERC20TokenAddress
        );
    }

    /**
      * @dev Buy Item from the Marketplace
      * @param tokenId - Id of the NFT
      * @param tokenAddress - Address of the NFT contract
      */

    function checkItemExistance(uint256 tokenId, address tokenAddress) external view returns (bool) {
       return openSales[tokenAddress][tokenId].orderId!=0;
    }

    function getItem(uint256 tokenId, address tokenAddress) external ItemExists(tokenAddress, tokenId) view returns (uint256,address,address) {
        MarketItem memory saleItem = openSales[tokenAddress][tokenId];
        
        uint256 price = saleItem.price;
        address seller = saleItem.seller;
        address ERC20TokenAddress = saleItem.ERC20TokenAddress;

       return (price,ERC20TokenAddress,seller);
    }

    function buyItem(uint256 tokenId, address tokenAddress, address ERC20TokenAddress, address ERC20TokenOwnerAddress) external ItemExists(tokenAddress, tokenId)
    HasTransferApproval(tokenAddress, tokenId) {
        MarketItem memory saleItem = openSales[tokenAddress][tokenId];
        require(saleItem.eligibleBuyer == payable(address(0)) || msg.sender == saleItem.eligibleBuyer, "Only eligible buyer can buy the item");
        
        // (address royaltyRecipient, uint256 royaltyAmount) = IERC2981Upgradeable(tokenAddress).royaltyInfo(tokenId, saleItem.price);

        // if(royaltyAmount > 0 && royaltyRecipient != address(0)) {
        //     address payable recipient = payable(royaltyRecipient);
        //     recipient.transfer(royaltyAmount);
        // }

        // uint256 commissionAmount = getCommission(saleItem.price);
        // commissionRecipient.transfer(commissionAmount);

        

        //Need to replace with ERC20 Token transfer
        // saleItem.seller.transfer(sellerAmount);
        uint256 sellerAmount = saleItem.price;
        address origin = ERC20TokenOwnerAddress;
        uint256 balanceOfSender = IERC20(ERC20TokenAddress).balanceOf(msg.sender);
        // require(msg.value == saleItem.price, "Value should be equal to the sales Item price");
        
        require(sellerAmount <= balanceOfSender, "Not enough ERC20 Tokens");

        IERC721Upgradeable(tokenAddress).safeTransferFrom(saleItem.seller, msg.sender, saleItem.tokenId);

        // IERC20(ERC20TokenAddress).transferFrom(From, to, price);
        IERC20(ERC20TokenAddress).transferFrom(msg.sender, saleItem.seller, sellerAmount);

        delete openSales[tokenAddress][tokenId];

        emit ItemSold(
            saleItem.orderId,
            saleItem.tokenAddress, 
            saleItem.tokenId, 
            saleItem.seller, 
            msg.sender, 
            saleItem.price,
            commissionRecipient,
            0,
            address(0),
            0
        );
    }

    /**
      * @dev Remove Item that is added to sell
      * @param tokenId - Id of the NFT
      * @param tokenAddress - Address of the NFT contract
      */
    function withdrawItem(uint256 tokenId, address tokenAddress, address ERC20TokenAddress) external ItemExists(tokenAddress, tokenId)
    OnlyTokenOwner(tokenAddress, tokenId) {
        MarketItem memory saleItem = openSales[tokenAddress][tokenId];
        delete openSales[tokenAddress][tokenId];

        emit ItemWithdrawed(
            saleItem.orderId,
            tokenAddress,
            tokenId,
            saleItem.seller,
            ERC20TokenAddress
        );
    }

    /**
      * @dev Update Item that is added to sell
      * @param tokenId - Id of the NFT
      * @param tokenAddress - Address of the NFT contract
      * @param newPrice - New price for the Item
      * @param newEligibleAddress - New eligible address for buyer
      */
    function updateItem(uint256 tokenId, address tokenAddress, uint256 newPrice, address newEligibleAddress, address ERC20TokenAddress) external ItemExists(tokenAddress, tokenId)
    OnlyTokenOwner(tokenAddress, tokenId) {
        MarketItem memory saleItem = openSales[tokenAddress][tokenId];
        saleItem.price = newPrice;
        saleItem.eligibleBuyer = payable(newEligibleAddress);
        openSales[tokenAddress][tokenId] = saleItem;

        emit ItemUpdated(
            saleItem.orderId,
            tokenAddress,
            tokenId,
            saleItem.seller,
            saleItem.price,
            saleItem.eligibleBuyer,
            ERC20TokenAddress
        );
    }

    /**
      * @dev Calculate the commission 
      * @param amount - Selling price of the NFT
      * @return Commission amount
      */
    function getCommission(uint256 amount) internal view returns (uint256) {
       return amount.mul(commissionFactor).div(10000);
    }

    /**
      * @dev Set the commission recipient 
      * @param commissionRecipientValue - Account address of the commission recipient
      */
    function setCommissionRecipient(address payable commissionRecipientValue) external onlyOwner {
        require(commissionRecipientValue != address(0), "fees cannot go to 0 address");
        commissionRecipient = commissionRecipientValue;
    }

    /**
      * @dev Set the commission factor
      * @param commissionFactorValue - 100% -> 10000
      */
    function setCommissionFactor(uint256 commissionFactorValue) external onlyOwner {
        require(commissionFactorValue <= commissionMaxLimit, "Cannot charge commision more than 10%");
        commissionFactor = commissionFactorValue;
    }

    function _authorizeUpgrade(address newImplementation) internal onlyOwner override {}
}
