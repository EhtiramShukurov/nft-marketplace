using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

namespace Xhibiter.Utilities
{
    public static class AccountManagement
    {
        public static async Task<string> AccountCreationAsync(string password,Web3 web3)
        {
            var newaccount = await web3.Personal.NewAccount.SendRequestAsync(password);
            return newaccount;
        }
        public static async Task<bool> SufficientBalanceAsync(this string address,Web3 web3, decimal value)
        {
            if (address.IsAddressValid())
            {
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            var transferAmount = Web3.Convert.ToWei(value, UnitConversion.EthUnit.Ether);
            var totalCost = web3.TransactionManager.DefaultGasPrice * web3.TransactionManager.DefaultGas + transferAmount;
            if (balance.Value < totalCost)
            {
                return false;
            }
            else
            {
                return true;
            }
            }
            else
            {
                return false;
            }
        }
        public static bool IsAddressValid(this string address)
        {
            if (address.IsValidEthereumAddressLength() && address.IsValidEthereumAddressHexFormat())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static async Task<decimal> AccountBalanceAsync(this string address,Web3 web3)
        {
            if (address.IsAddressValid())
            {
                var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
                decimal etherAmount = Web3.Convert.FromWei(balance.Value);
                return Math.Round(etherAmount,1);
            }
            else
            {
                return 0;
            }
        }
        public static async Task<string> TransferAsync(this string address,string password,decimal value,string recipientAddress)
        {
            try
            {
                if (address.IsAddressValid())
                {
                    var account = new ManagedAccount(address, password);
                    var web3 = new Web3(account);
                    var isUnlocked = await web3.Personal.UnlockAccount.SendRequestAsync(account.Address, password, 120);
                    var transferAmount = Web3.Convert.ToWei(value, UnitConversion.EthUnit.Ether);
                    bool sufficient = await address.SufficientBalanceAsync(web3, value);
                    if (sufficient)
                    {
                        var transaction = web3.Eth.GetEtherTransferService()
                            .TransferEtherAndWaitForReceiptAsync(recipientAddress, value);
                        var receipt = await transaction;
                        if (receipt.Status.Value == 1)
                        {
                            return "Transaction was successful.";
                        }
                        else
                        {
                            return "Transaction failed.";
                        }
                    }
                    else
                    {
                        return "The address does not exist or doesn't have enough balance.";
                    }
                }
                else
                {
                    return "Address is not valid";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("could not decrypt key with given password"))
                {
                    return "wrong password";
                }
                else
                {
                    return "An error occurred: " + ex.Message;
                }
            }
        }
    }
}
