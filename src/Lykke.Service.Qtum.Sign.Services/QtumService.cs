﻿using Lykke.Service.Qtum.Sign.Core.Services;
using NBitcoin;
using NBitcoin.Qtum;
using NBitcoin.Policy;
using System;
using System.Linq;

namespace Lykke.Service.Qtum.Sign.Services
{
    public class QtumService : IQtumService
    {
        private readonly Network _network;

        public QtumService(string network)
        {
            QtumNetworks.Register();

            _network = Network.GetNetwork(network);
        }

        public bool IsValidPrivateKey(string privateKey)
        {
            try
            {
                Key.Parse(privateKey, _network);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string GetPrivateKey()
        {
            var key = new Key();

            return key.GetWif(_network).ToString();
        }

        public string GetPublicAddress(string privateKey)
        {
            var wallet = new BitcoinSecret(privateKey);

            return wallet.PubKey.GetAddress(_network).ToString();
        }

        public string SignTransaction(Transaction tx, ICoin[] coins, Key[] keys)
        {
            var builder = new TransactionBuilder().AddCoins(coins).AddKeys(keys);

            var signed = builder.SignTransaction(tx);

            if (!builder
                .SetTransactionPolicy(new StandardTransactionPolicy { CheckFee = false })
                .Verify(signed, out var errors))
            {
                throw new InvalidOperationException($"Invalid transaction sign: {string.Join("; ", errors.Select(e => e.ToString()))}");
            }

            return signed.ToHex();
        }
    }
}
