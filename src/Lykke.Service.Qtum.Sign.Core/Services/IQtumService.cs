﻿using NBitcoin;

namespace Lykke.Service.Qtum.Sign.Core.Services
{
    public interface IQtumService
    {
        bool IsValidPrivateKey(string privateKey);
        string GetPrivateKey();
        string GetPublicAddress(string privateKey);
        string SignTransaction(Transaction tx, ICoin[] coins, Key[] keys);
    }
}
