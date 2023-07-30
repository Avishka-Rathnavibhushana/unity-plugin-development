pragma solidity ^0.8.7;
// SPDX-License-Identifier: MIT

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/utils/Strings.sol";

contract FeeCollector{
    uint256 public ratio;
    constructor() {
        ratio = 1;
    }

    function claim(IERC20 token, address to, uint256 amount, address origin) external {
        uint256 senderERC20TokenBalance = IERC20(token).balanceOf(origin);

        uint256 transferingERC20TokenAmount = amount/ratio;

        require(transferingERC20TokenAmount <= senderERC20TokenBalance, "Not enough ERC20 Tokens");

        IERC20(token).transferFrom(origin, to, transferingERC20TokenAmount);
        // emit TransferSent(msg.sender, to, amount);
    }

    // // Need to implement an approve function
    // function approveClaim(IERC20 token, uint256 amount, address tokenAddress) external {
    //     uint256 senderERC20TokenBalance = IERC20(token).balanceOf(msg.sender);

    //     uint256 transferingERC20TokenAmount = amount/ratio;

    //     require(transferingERC20TokenAmount <= senderERC20TokenBalance, "Not enough ERC20 Tokens");

    //     IERC20(token).approve(tokenAddress, transferingERC20TokenAmount);
    //     // emit TransferSent(msg.sender, to, amount);
    // }

    function setRatio(uint256 newRation) external {
        ratio = newRation;
    }
}