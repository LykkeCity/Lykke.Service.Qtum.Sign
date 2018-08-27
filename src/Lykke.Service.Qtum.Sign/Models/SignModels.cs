﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;
using Lykke.Service.Qtum.Sign.Core.Services;

namespace Lykke.Service.Qtum.Sign.Models
{
    [DataContract]
    public class SignTransactionRequest : IValidatableObject
    {
        [DataMember]
        [Required]
        public string[] PrivateKeys { get; set; }

        [DataMember]
        [Required]
        public string TransactionContext { get; set; }

        public Transaction Tx { get; private set; }

        public ICoin[] Coins { get; private set; }

        public Key[] Keys { get; private set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            var qtumService = (IQtumService)validationContext.GetService(typeof(IQtumService));

            if (PrivateKeys == null || !PrivateKeys.Any())
            {
                result.Add(new ValidationResult(
                    $"{nameof(PrivateKeys)} array can not be empty",
                    new[] { nameof(PrivateKeys) }));
            }

            var num = 0;
            foreach (var key in PrivateKeys)
            {
                if (!qtumService.IsValidPrivateKey(key))
                {
                    result.Add(new ValidationResult(
                        $"{nameof(PrivateKeys)}.[{num}] is not a valid", 
                        new[] { nameof(PrivateKeys) }));
                }

                num++;
            }

            if (Tx == null && Coins == null)
            {
                try
                {
                    (Tx, Coins) = Serializer.ToObject<(Transaction, ICoin[])>(TransactionContext);
                }
                catch
                {
                    (Tx, Coins) = (null, null);
                }
            }

            if (Keys == null)
            {
                try
                {
                    Keys = PrivateKeys.Select(k => Key.Parse(k)).ToArray();
                }
                catch
                {
                    Keys = null;
                }
            }

            if (Tx == null || Coins == null || Coins.Length == 0)
            {
                result.Add(new ValidationResult(
                    $"{nameof(TransactionContext)} is not valid", 
                    new[] { nameof(TransactionContext) }));
            }

            return result;
        }
    }

    public class SignResponse
    {
        public string SignedTransaction { get; set; }
    }
}
