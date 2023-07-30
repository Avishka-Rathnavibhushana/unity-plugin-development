// contracts/GLDToken.sol
// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";

contract GLDToken is ERC20 {
    constructor(uint256 initialSupply) ERC20("Gold", "GLD") {
        _mint(msg.sender, initialSupply);
    }

    function claim(address to, uint256 amount) public returns (bool) { 
        // Minimum amount needed to claim reward is 10
        require(amount >= 10, "balance is low");

        // each 10 of amount = 1 GLD token
        uint256 rewardingTokens = amount/10;

        transfer(to, rewardingTokens);

        return true;
    }
}