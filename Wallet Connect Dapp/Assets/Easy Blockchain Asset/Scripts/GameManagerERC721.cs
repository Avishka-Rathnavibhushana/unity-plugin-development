using System;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

using UnityEngine;

using Nethereum.Web3;
using Nethereum.Util;
using Nethereum.ABI;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI.FunctionEncoding.Attributes;

public class StandardTokenDeploymentERC721 : ContractDeploymentMessage
{
    public static string BYTECODE =
        "608060405260006008553480156200001657600080fd5b506040518060400160405280600881526020017f47616d654974656d0000000000000000000000000000000000000000000000008152506040518060400160405280600381526020017f49544d000000000000000000000000000000000000000000000000000000000081525081600090805190602001906200009b929190620000bd565b508060019080519060200190620000b4929190620000bd565b505050620001d2565b828054620000cb906200016d565b90600052602060002090601f016020900481019282620000ef57600085556200013b565b82601f106200010a57805160ff19168380011785556200013b565b828001600101855582156200013b579182015b828111156200013a5782518255916020019190600101906200011d565b5b5090506200014a91906200014e565b5090565b5b80821115620001695760008160009055506001016200014f565b5090565b600060028204905060018216806200018657607f821691505b602082108114156200019d576200019c620001a3565b5b50919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b612c1480620001e26000396000f3fe608060405234801561001057600080fd5b50600436106100f55760003560e01c806370a0823111610097578063b88d4fde11610066578063b88d4fde14610284578063c87b56dd146102a0578063cf378343146102d0578063e985e9c514610300576100f5565b806370a08231146101fc578063764912ef1461022c57806395d89b411461024a578063a22cb46514610268576100f5565b8063095ea7b3116100d3578063095ea7b31461017857806323b872dd1461019457806342842e0e146101b05780636352211e146101cc576100f5565b806301ffc9a7146100fa57806306fdde031461012a578063081812fc14610148575b600080fd5b610114600480360381019061010f9190611c72565b610330565b6040516101219190612084565b60405180910390f35b610132610412565b60405161013f919061209f565b60405180910390f35b610162600480360381019061015d9190611ccc565b6104a4565b60405161016f919061201d565b60405180910390f35b610192600480360381019061018d9190611c32565b610529565b005b6101ae60048036038101906101a99190611ac0565b610641565b005b6101ca60048036038101906101c59190611ac0565b6106a1565b005b6101e660048036038101906101e19190611ccc565b6106c1565b6040516101f3919061201d565b60405180910390f35b61021660048036038101906102119190611a53565b610773565b60405161022391906122c1565b60405180910390f35b61023461082b565b60405161024191906122c1565b60405180910390f35b610252610831565b60405161025f919061209f565b60405180910390f35b610282600480360381019061027d9190611b96565b6108c3565b005b61029e60048036038101906102999190611b13565b6108d9565b005b6102ba60048036038101906102b59190611ccc565b61093b565b6040516102c7919061209f565b60405180910390f35b6102ea60048036038101906102e59190611bd6565b610a8d565b6040516102f791906122c1565b60405180910390f35b61031a60048036038101906103159190611a80565b610ad9565b6040516103279190612084565b60405180910390f35b60007f80ac58cd000000000000000000000000000000000000000000000000000000007bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916827bffffffffffffffffffffffffffffffffffffffffffffffffffffffff191614806103fb57507f5b5e139f000000000000000000000000000000000000000000000000000000007bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916827bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916145b8061040b575061040a82610b6d565b5b9050919050565b60606000805461042190612517565b80601f016020809104026020016040519081016040528092919081815260200182805461044d90612517565b801561049a5780601f1061046f5761010080835404028352916020019161049a565b820191906000526020600020905b81548152906001019060200180831161047d57829003601f168201915b5050505050905090565b60006104af82610bd7565b6104ee576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016104e590612241565b60405180910390fd5b6004600083815260200190815260200160002060009054906101000a900473ffffffffffffffffffffffffffffffffffffffff169050919050565b6000610534826106c1565b90508073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff1614156105a5576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161059c90612281565b60405180910390fd5b8073ffffffffffffffffffffffffffffffffffffffff166105c4610c43565b73ffffffffffffffffffffffffffffffffffffffff1614806105f357506105f2816105ed610c43565b610ad9565b5b610632576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161062990612181565b60405180910390fd5b61063c8383610c4b565b505050565b61065261064c610c43565b82610d04565b610691576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610688906122a1565b60405180910390fd5b61069c838383610de2565b505050565b6106bc838383604051806020016040528060008152506108d9565b505050565b6000806002600084815260200190815260200160002060009054906101000a900473ffffffffffffffffffffffffffffffffffffffff169050600073ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff16141561076a576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610761906121c1565b60405180910390fd5b80915050919050565b60008073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1614156107e4576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016107db906121a1565b60405180910390fd5b600360008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050919050565b60085481565b60606001805461084090612517565b80601f016020809104026020016040519081016040528092919081815260200182805461086c90612517565b80156108b95780601f1061088e576101008083540402835291602001916108b9565b820191906000526020600020905b81548152906001019060200180831161089c57829003601f168201915b5050505050905090565b6108d56108ce610c43565b8383611049565b5050565b6108ea6108e4610c43565b83610d04565b610929576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610920906122a1565b60405180910390fd5b610935848484846111b6565b50505050565b606061094682610bd7565b610985576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161097c90612221565b60405180910390fd5b60006006600084815260200190815260200160002080546109a590612517565b80601f01602080910402602001604051908101604052809291908181526020018280546109d190612517565b8015610a1e5780601f106109f357610100808354040283529160200191610a1e565b820191906000526020600020905b815481529060010190602001808311610a0157829003601f168201915b505050505090506000610a2f611212565b9050600081511415610a45578192505050610a88565b600082511115610a7a578082604051602001610a62929190611ff9565b60405160208183030381529060405292505050610a88565b610a8384611229565b925050505b919050565b600080610a9a60076112d0565b9050610aa684826112de565b610ab081846114b8565b610aba600761152c565b6001600854610ac991906123a6565b6008819055508091505092915050565b6000600560008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060009054906101000a900460ff16905092915050565b60007f01ffc9a7000000000000000000000000000000000000000000000000000000007bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916827bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916149050919050565b60008073ffffffffffffffffffffffffffffffffffffffff166002600084815260200190815260200160002060009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1614159050919050565b600033905090565b816004600083815260200190815260200160002060006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550808273ffffffffffffffffffffffffffffffffffffffff16610cbe836106c1565b73ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b92560405160405180910390a45050565b6000610d0f82610bd7565b610d4e576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610d4590612161565b60405180910390fd5b6000610d59836106c1565b90508073ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff161480610d9b5750610d9a8185610ad9565b5b80610dd957508373ffffffffffffffffffffffffffffffffffffffff16610dc1846104a4565b73ffffffffffffffffffffffffffffffffffffffff16145b91505092915050565b8273ffffffffffffffffffffffffffffffffffffffff16610e02826106c1565b73ffffffffffffffffffffffffffffffffffffffff1614610e58576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610e4f906120e1565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff161415610ec8576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610ebf90612121565b60405180910390fd5b610ed3838383611542565b610ede600082610c4b565b6001600360008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828254610f2e919061242d565b925050819055506001600360008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828254610f8591906123a6565b92505081905550816002600083815260200190815260200160002060006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550808273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef60405160405180910390a4611044838383611547565b505050565b8173ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff1614156110b8576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016110af90612141565b60405180910390fd5b80600560008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060006101000a81548160ff0219169083151502179055508173ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff167f17307eab39ab6107e8899845ad3d59bd9653f200f220920489ca2b5937696c31836040516111a99190612084565b60405180910390a3505050565b6111c1848484610de2565b6111cd8484848461154c565b61120c576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401611203906120c1565b60405180910390fd5b50505050565b606060405180602001604052806000815250905090565b606061123482610bd7565b611273576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161126a90612261565b60405180910390fd5b600061127d611212565b9050600081511161129d57604051806020016040528060008152506112c8565b806112a7846116e3565b6040516020016112b8929190611ff9565b6040516020818303038152906040525b915050919050565b600081600001549050919050565b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff16141561134e576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161134590612201565b60405180910390fd5b61135781610bd7565b15611397576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161138e90612101565b60405180910390fd5b6113a360008383611542565b6001600360008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282546113f391906123a6565b92505081905550816002600083815260200190815260200160002060006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550808273ffffffffffffffffffffffffffffffffffffffff16600073ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef60405160405180910390a46114b460008383611547565b5050565b6114c182610bd7565b611500576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016114f7906121e1565b60405180910390fd5b80600660008481526020019081526020016000209080519060200190611527929190611867565b505050565b6001816000016000828254019250508190555050565b505050565b505050565b600061156d8473ffffffffffffffffffffffffffffffffffffffff16611844565b156116d6578373ffffffffffffffffffffffffffffffffffffffff1663150b7a02611596610c43565b8786866040518563ffffffff1660e01b81526004016115b89493929190612038565b602060405180830381600087803b1580156115d257600080fd5b505af192505050801561160357506040513d601f19601f820116820180604052508101906116009190611c9f565b60015b611686573d8060008114611633576040519150601f19603f3d011682016040523d82523d6000602084013e611638565b606091505b5060008151141561167e576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401611675906120c1565b60405180910390fd5b805181602001fd5b63150b7a0260e01b7bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916817bffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916149150506116db565b600190505b949350505050565b6060600082141561172b576040518060400160405280600181526020017f3000000000000000000000000000000000000000000000000000000000000000815250905061183f565b600082905060005b6000821461175d5780806117469061257a565b915050600a8261175691906123fc565b9150611733565b60008167ffffffffffffffff811115611779576117786126b0565b5b6040519080825280601f01601f1916602001820160405280156117ab5781602001600182028036833780820191505090505b5090505b60008514611838576001826117c4919061242d565b9150600a856117d391906125c3565b60306117df91906123a6565b60f81b8183815181106117f5576117f4612681565b5b60200101907effffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff1916908160001a905350600a8561183191906123fc565b94506117af565b8093505050505b919050565b6000808273ffffffffffffffffffffffffffffffffffffffff163b119050919050565b82805461187390612517565b90600052602060002090601f01602090048101928261189557600085556118dc565b82601f106118ae57805160ff19168380011785556118dc565b828001600101855582156118dc579182015b828111156118db5782518255916020019190600101906118c0565b5b5090506118e991906118ed565b5090565b5b808211156119065760008160009055506001016118ee565b5090565b600061191d61191884612301565b6122dc565b905082815260208101848484011115611939576119386126e4565b5b6119448482856124d5565b509392505050565b600061195f61195a84612332565b6122dc565b90508281526020810184848401111561197b5761197a6126e4565b5b6119868482856124d5565b509392505050565b60008135905061199d81612b82565b92915050565b6000813590506119b281612b99565b92915050565b6000813590506119c781612bb0565b92915050565b6000815190506119dc81612bb0565b92915050565b600082601f8301126119f7576119f66126df565b5b8135611a0784826020860161190a565b91505092915050565b600082601f830112611a2557611a246126df565b5b8135611a3584826020860161194c565b91505092915050565b600081359050611a4d81612bc7565b92915050565b600060208284031215611a6957611a686126ee565b5b6000611a778482850161198e565b91505092915050565b60008060408385031215611a9757611a966126ee565b5b6000611aa58582860161198e565b9250506020611ab68582860161198e565b9150509250929050565b600080600060608486031215611ad957611ad86126ee565b5b6000611ae78682870161198e565b9350506020611af88682870161198e565b9250506040611b0986828701611a3e565b9150509250925092565b60008060008060808587031215611b2d57611b2c6126ee565b5b6000611b3b8782880161198e565b9450506020611b4c8782880161198e565b9350506040611b5d87828801611a3e565b925050606085013567ffffffffffffffff811115611b7e57611b7d6126e9565b5b611b8a878288016119e2565b91505092959194509250565b60008060408385031215611bad57611bac6126ee565b5b6000611bbb8582860161198e565b9250506020611bcc858286016119a3565b9150509250929050565b60008060408385031215611bed57611bec6126ee565b5b6000611bfb8582860161198e565b925050602083013567ffffffffffffffff811115611c1c57611c1b6126e9565b5b611c2885828601611a10565b9150509250929050565b60008060408385031215611c4957611c486126ee565b5b6000611c578582860161198e565b9250506020611c6885828601611a3e565b9150509250929050565b600060208284031215611c8857611c876126ee565b5b6000611c96848285016119b8565b91505092915050565b600060208284031215611cb557611cb46126ee565b5b6000611cc3848285016119cd565b91505092915050565b600060208284031215611ce257611ce16126ee565b5b6000611cf084828501611a3e565b91505092915050565b611d0281612461565b82525050565b611d1181612473565b82525050565b6000611d2282612363565b611d2c8185612379565b9350611d3c8185602086016124e4565b611d45816126f3565b840191505092915050565b6000611d5b8261236e565b611d65818561238a565b9350611d758185602086016124e4565b611d7e816126f3565b840191505092915050565b6000611d948261236e565b611d9e818561239b565b9350611dae8185602086016124e4565b80840191505092915050565b6000611dc760328361238a565b9150611dd282612704565b604082019050919050565b6000611dea60258361238a565b9150611df582612753565b604082019050919050565b6000611e0d601c8361238a565b9150611e18826127a2565b602082019050919050565b6000611e3060248361238a565b9150611e3b826127cb565b604082019050919050565b6000611e5360198361238a565b9150611e5e8261281a565b602082019050919050565b6000611e76602c8361238a565b9150611e8182612843565b604082019050919050565b6000611e9960388361238a565b9150611ea482612892565b604082019050919050565b6000611ebc602a8361238a565b9150611ec7826128e1565b604082019050919050565b6000611edf60298361238a565b9150611eea82612930565b604082019050919050565b6000611f02602e8361238a565b9150611f0d8261297f565b604082019050919050565b6000611f2560208361238a565b9150611f30826129ce565b602082019050919050565b6000611f4860318361238a565b9150611f53826129f7565b604082019050919050565b6000611f6b602c8361238a565b9150611f7682612a46565b604082019050919050565b6000611f8e602f8361238a565b9150611f9982612a95565b604082019050919050565b6000611fb160218361238a565b9150611fbc82612ae4565b604082019050919050565b6000611fd460318361238a565b9150611fdf82612b33565b604082019050919050565b611ff3816124cb565b82525050565b60006120058285611d89565b91506120118284611d89565b91508190509392505050565b60006020820190506120326000830184611cf9565b92915050565b600060808201905061204d6000830187611cf9565b61205a6020830186611cf9565b6120676040830185611fea565b81810360608301526120798184611d17565b905095945050505050565b60006020820190506120996000830184611d08565b92915050565b600060208201905081810360008301526120b98184611d50565b905092915050565b600060208201905081810360008301526120da81611dba565b9050919050565b600060208201905081810360008301526120fa81611ddd565b9050919050565b6000602082019050818103600083015261211a81611e00565b9050919050565b6000602082019050818103600083015261213a81611e23565b9050919050565b6000602082019050818103600083015261215a81611e46565b9050919050565b6000602082019050818103600083015261217a81611e69565b9050919050565b6000602082019050818103600083015261219a81611e8c565b9050919050565b600060208201905081810360008301526121ba81611eaf565b9050919050565b600060208201905081810360008301526121da81611ed2565b9050919050565b600060208201905081810360008301526121fa81611ef5565b9050919050565b6000602082019050818103600083015261221a81611f18565b9050919050565b6000602082019050818103600083015261223a81611f3b565b9050919050565b6000602082019050818103600083015261225a81611f5e565b9050919050565b6000602082019050818103600083015261227a81611f81565b9050919050565b6000602082019050818103600083015261229a81611fa4565b9050919050565b600060208201905081810360008301526122ba81611fc7565b9050919050565b60006020820190506122d66000830184611fea565b92915050565b60006122e66122f7565b90506122f28282612549565b919050565b6000604051905090565b600067ffffffffffffffff82111561231c5761231b6126b0565b5b612325826126f3565b9050602081019050919050565b600067ffffffffffffffff82111561234d5761234c6126b0565b5b612356826126f3565b9050602081019050919050565b600081519050919050565b600081519050919050565b600082825260208201905092915050565b600082825260208201905092915050565b600081905092915050565b60006123b1826124cb565b91506123bc836124cb565b9250827fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff038211156123f1576123f06125f4565b5b828201905092915050565b6000612407826124cb565b9150612412836124cb565b92508261242257612421612623565b5b828204905092915050565b6000612438826124cb565b9150612443836124cb565b925082821015612456576124556125f4565b5b828203905092915050565b600061246c826124ab565b9050919050565b60008115159050919050565b60007fffffffff0000000000000000000000000000000000000000000000000000000082169050919050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000819050919050565b82818337600083830152505050565b60005b838110156125025780820151818401526020810190506124e7565b83811115612511576000848401525b50505050565b6000600282049050600182168061252f57607f821691505b6020821081141561254357612542612652565b5b50919050565b612552826126f3565b810181811067ffffffffffffffff82111715612571576125706126b0565b5b80604052505050565b6000612585826124cb565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8214156125b8576125b76125f4565b5b600182019050919050565b60006125ce826124cb565b91506125d9836124cb565b9250826125e9576125e8612623565b5b828206905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b600080fd5b600080fd5b600080fd5b600080fd5b6000601f19601f8301169050919050565b7f4552433732313a207472616e7366657220746f206e6f6e20455243373231526560008201527f63656976657220696d706c656d656e7465720000000000000000000000000000602082015250565b7f4552433732313a207472616e736665722066726f6d20696e636f72726563742060008201527f6f776e6572000000000000000000000000000000000000000000000000000000602082015250565b7f4552433732313a20746f6b656e20616c7265616479206d696e74656400000000600082015250565b7f4552433732313a207472616e7366657220746f20746865207a65726f2061646460008201527f7265737300000000000000000000000000000000000000000000000000000000602082015250565b7f4552433732313a20617070726f766520746f2063616c6c657200000000000000600082015250565b7f4552433732313a206f70657261746f7220717565727920666f72206e6f6e657860008201527f697374656e7420746f6b656e0000000000000000000000000000000000000000602082015250565b7f4552433732313a20617070726f76652063616c6c6572206973206e6f74206f7760008201527f6e6572206e6f7220617070726f76656420666f7220616c6c0000000000000000602082015250565b7f4552433732313a2062616c616e636520717565727920666f7220746865207a6560008201527f726f206164647265737300000000000000000000000000000000000000000000602082015250565b7f4552433732313a206f776e657220717565727920666f72206e6f6e657869737460008201527f656e7420746f6b656e0000000000000000000000000000000000000000000000602082015250565b7f45524337323155524953746f726167653a2055524920736574206f66206e6f6e60008201527f6578697374656e7420746f6b656e000000000000000000000000000000000000602082015250565b7f4552433732313a206d696e7420746f20746865207a65726f2061646472657373600082015250565b7f45524337323155524953746f726167653a2055524920717565727920666f722060008201527f6e6f6e6578697374656e7420746f6b656e000000000000000000000000000000602082015250565b7f4552433732313a20617070726f76656420717565727920666f72206e6f6e657860008201527f697374656e7420746f6b656e0000000000000000000000000000000000000000602082015250565b7f4552433732314d657461646174613a2055524920717565727920666f72206e6f60008201527f6e6578697374656e7420746f6b656e0000000000000000000000000000000000602082015250565b7f4552433732313a20617070726f76616c20746f2063757272656e74206f776e6560008201527f7200000000000000000000000000000000000000000000000000000000000000602082015250565b7f4552433732313a207472616e736665722063616c6c6572206973206e6f74206f60008201527f776e6572206e6f7220617070726f766564000000000000000000000000000000602082015250565b612b8b81612461565b8114612b9657600080fd5b50565b612ba281612473565b8114612bad57600080fd5b50565b612bb98161247f565b8114612bc457600080fd5b50565b612bd0816124cb565b8114612bdb57600080fd5b5056fea26469706673582212204cb165ef63292cc798ef498ba6e36f85298a1ad8b7a10e2bde5c402c413c982e64736f6c63430008070033";

    public StandardTokenDeploymentERC721() : base(BYTECODE)
    {
    }
}

[Function("balanceOf", "uint256")]
public class BalanceOfFunctionERC721 : FunctionMessage
{
    [Parameter("address", "owner", 0)]
    public string Owner { get; set; }
}

[Function("name", "string")]
public class NameFunctionERC721 : FunctionMessage
{

}

[Function("totalTokenCount", "uint256")]
public class TotalTokenCountFunctionERC721 : FunctionMessage
{

}

[Function("symbol", "string")]
public class SymbolFunctionERC721 : FunctionMessage
{

}

[Function("ownerOf", "address")]
public class OwnerFunctionERC721 : FunctionMessage
{
    [Parameter("uint256", "tokenId", 0)]
    public BigInteger TokenId { get; set; }
}

[Function("tokenURI", "string")]
public class TokenURIFunctionERC721 : FunctionMessage
{
    [Parameter("uint256", "tokenId", 0)]
    public BigInteger TokenId { get; set; }
}

[Function("getApproved", "address")]
public class GetApprovedFunctionERC721 : FunctionMessage
{
    [Parameter("uint256", "tokenId", 0)]
    public BigInteger TokenId { get; set; }
}

[Function("awardItem", "uint256")]
public class MintFunctionERC721 : FunctionMessage
{
    [Parameter("address", "player", 1)]
    public string player { get; set; }

    [Parameter("string", "tokenURI", 2)]
    public string tokenURI { get; set; }
}

[Function("approve", "bool")]
public class ApproveFunctionERC721 : FunctionMessage
{
    [Parameter("address", "to", 0)]
    public string to { get; set; }

    [Parameter("uint256", "tokenId", 1)]
    public BigInteger tokenId { get; set; }
}

[Function("transfer", "bool")]
public class TransferFunctionERC721 : FunctionMessage
{
    [Parameter("address", "_to", 0)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 1)]
    public BigInteger TokenAmount { get; set; }
}

[Function("setApprovalForAll", "bool")]
public class SetApprovalForAllFunctionERC721 : FunctionMessage
{
    [Parameter("address", "operator", 0)]
    public string spender { get; set; }

    [Parameter("bool", "approved", 1)]
    public bool approved { get; set; }
}

[Function("isApprovedForAll", "bool")]
public class IsApprovedForAllFunctionERC721 : FunctionMessage
{
    [Parameter("address", "owner", 0)]
    public string owner { get; set; }

    [Parameter("address", "operator", 1)]
    public string spender { get; set; }
}

public class GameManagerERC721 : MonoBehaviour
{
    private string gameDeveloperPrivateKey = "";

    private int chainId;
    private string rpcUrl;

    public WalletConnectClient walletConnectClient;

    // Start is called before the first frame update
    void Start()
    {
        this.chainId = Constants.AVALANCHE_TESTNET_CHAIN_ID;
        this.rpcUrl = Constants.AVALANCHE_TESTNET_RPC_URL;

        LoadPrivateKey();
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
            Debug.Log("No File");
            return false;
        }
    }

    public void LoadFromWalletAcccountData(WalletAcccountData walletAcccountData)
    {
        // Set local variables
        this.gameDeveloperPrivateKey = walletAcccountData.privateKey;
    }

    public async Task<string> getTokenURIOfAnNFT(BigInteger tokenId, string contractAddress)
    {
        var account = new Account(this.gameDeveloperPrivateKey, this.chainId);
        var web3 = new Web3(account, this.rpcUrl);

        var uriOfFunctionMessage = new TokenURIFunctionERC721()
        {
            TokenId = tokenId,
        };

        var queryHandler = web3.Eth.GetContractQueryHandler<TokenURIFunctionERC721>();
        var uriLocal = await queryHandler
            .QueryAsync<string>(contractAddress, uriOfFunctionMessage)
            .ConfigureAwait(false);

        return uriLocal.ToString();
    }

    public async Task<string> getOwnerOfNFT(BigInteger tokenId, string contractAddress)
    {
        var web3 = new Web3(this.rpcUrl);

        var functionMessage = new OwnerFunctionERC721()
        {
            TokenId = tokenId
        };
        var handler = web3.Eth.GetContractQueryHandler<OwnerFunctionERC721>();
        var result = await handler
            .QueryAsync<string>(contractAddress, functionMessage)
            .ConfigureAwait(false);

        return result;
    }

    public async Task<bool> getIsApprovedForAllFunctionERC721(string owner, string spender, string contractAddress)
    {
        var web3 = new Web3(this.rpcUrl);

        var functionMessage = new IsApprovedForAllFunctionERC721()
        {
            owner = owner,
            spender = spender
        };

        var queryHandler = web3.Eth.GetContractQueryHandler<IsApprovedForAllFunctionERC721>();
        var result = await queryHandler
            .QueryAsync<bool>(contractAddress, functionMessage)
            .ConfigureAwait(false);

        return result;
    }

    public async Task<bool> PlayerApproveAllNFTs(string spender, bool approved, string gameDeveloperPublicKey, string contractAddress)
    {
        string functionSignature = "setApprovalForAll(address,bool)";
        var functionMessage = new SetApprovalForAllFunctionERC721()
        {
            spender = spender,
            approved = approved
        };

        byte[] value = Encoding.ASCII.GetBytes(functionSignature);
        var hash = new Sha3Keccack().CalculateHash(value);
        var methodSignature = hash.ToHex();
        var methodSignature4bytes = methodSignature.Substring(0, 8);

        var abiEncode = new ABIEncode();
        var parameterEncoding = abiEncode.GetABIParamsEncoded(functionMessage).ToHex();

        var completeDataHex = "0x" + methodSignature4bytes + parameterEncoding;

        await walletConnectClient.SendTransactionCustom(completeDataHex, gameDeveloperPublicKey, contractAddress, this.chainId);

        Debug.Log("Approve All Successful");
        return true;
    }

    public async Task<BigInteger> getTotalTokenCount(string contractAddress)
    {
        var web3 = new Web3(this.rpcUrl);

        var functionMessage = new TotalTokenCountFunctionERC721()
        {
        };

        var handler = web3.Eth.GetContractQueryHandler<TotalTokenCountFunctionERC721>();
        var result = await handler.QueryAsync<BigInteger>(contractAddress, functionMessage);

        return result;
    }
}
