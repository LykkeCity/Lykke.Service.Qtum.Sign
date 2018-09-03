using Common.Log;
using Lykke.Service.Qtum.Sign.Services;
using NBitcoin;
using NBitcoin.Qtum;
using NBitcoin.Policy;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;
using System.Linq;
using Xunit;
using Lykke.Service.Qtum.Sign.Models;
using System.ComponentModel.DataAnnotations;
using System;
using Moq;
using Lykke.Service.Qtum.Sign.Core.Services;

namespace Lykke.Service.Qtum.Sign.Tests
{
    public class QtumServiceTests
    {
        public Network network = QtumNetworks.Testnet;
        public string from = "qSsbGREwMFvV4dtjgid3tMgvz5Hw2YVcbp";
        public string fromPrivateKey = "cSJ8DmmYHuMCTPz9HUU5xwSKubcfiY6moa1LcUgBMj8FRvG1TgtW";
        public BitcoinAddress fromAddress;
        public Key fromKey;
        public string to = "qKEtj1SUuvGRYLzXto9R2hjVryfQavPyJ1";
        public BitcoinAddress toAddress;
        public Transaction prevTx = Transaction.Parse("0200000001ab6b46efb237046e1e609544d1ec250724b853eb8e87e41741e2484ebcc935ac010000006a4730440220447b115517b6ef94b5168c126f1028ae4de9622ed65b88f66d015ecdd1404f39022002a4b2dfc855a21d5a256fe4aa59e77d097a017e02a9beeda566c92522aa8ee8012102988f67b90c13b967d537c2473e05a0c6aaf6ed4fa1272c982a8ed6523b0b2893feffffff020050d6dc010000001976a914662aba77ea81b5d9f3f61c040097564f8c33efe688ac4e2e9151560000001976a91411be0b5052608566cd35e44599ff7d166c5c105088acd4160300");
        public TransactionBuilder txBuilder = new TransactionBuilder();
        public Transaction tx;
        public ICoin[] spentCoins;
        public QtumService service;
        public Mock<IServiceProvider> serviceProvider;

        public QtumServiceTests()
        {
            new LogToMemory();

            fromAddress = new BitcoinPubKeyAddress(from);
            fromKey = Key.Parse(fromPrivateKey);
            toAddress = new BitcoinPubKeyAddress(to);
            tx = txBuilder
                .AddCoins(prevTx.Outputs.AsCoins().Where(c => c.ScriptPubKey.GetDestinationAddress(network).ToString() == from).ToArray())
                .Send(toAddress, Money.Coins(1))
                .SetChange(fromAddress)
                .SubtractFees()
                .SendFees(txBuilder.EstimateFees(new FeeRate(Money.Satoshis(1024))))
                .BuildTransaction(false);
            spentCoins = txBuilder.FindSpentCoins(tx);
            service = new QtumService("qtum-testnet");

            serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(provider => provider.GetService(typeof(IQtumService)))
                .Returns(service);
        }

        [Fact]
        public void GetPrivateKeyShouldReturnData()
        {
            // Act
            var key = service.GetPrivateKey();

            // Assert
            Assert.Equal(52, key.Length);
        }

        [Fact]
        public void GetPublicAddressShouldReturnData()
        {
            // Arrange
            var key = service.GetPrivateKey();

            // Act
            var address = service.GetPublicAddress(key);

            // Assert
            Assert.Equal(34, address.Length);
        }

        [Fact]
        public void ShouldSignTransaction()
        {
            // Act
            var signedTransactionHex = service.SignTransaction(tx, spentCoins, new[] { fromKey });
            var signedTx = Transaction.Parse(signedTransactionHex);

            // Assert
            Assert.True(new TransactionBuilder()
                .AddCoins(spentCoins)
                .SetTransactionPolicy(new StandardTransactionPolicy { CheckFee = false })
                .Verify(signedTx, out var errors));
        }

        [Fact]
        public void ShouldSerializeDeserializeData()
        {
            // Arrange
            var body = JsonConvert.SerializeObject(new
            {
                PrivateKeys = new[] { this.fromPrivateKey },
                TransactionContext = Serializer.ToString((this.tx, this.spentCoins))
            });

            // Act
            var request = JsonConvert.DeserializeObject<SignTransactionRequest>(body);
            var validationResult = request.Validate(new ValidationContext(request, serviceProvider.Object, null));

            // Assert;
            Assert.Empty(validationResult);
        }

        [Fact]
        public void ShouldNotValidate_IfTxIsNull()
        {
            // Arrange
            var body = JsonConvert.SerializeObject(new
            {
                PrivateKeys = new[] { this.fromPrivateKey },
                TransactionContext = Serializer.ToString(((Transaction)null, this.spentCoins))
            });

            // Act
            var request = JsonConvert.DeserializeObject<SignTransactionRequest>(body);
            var validationResult = request.Validate(new ValidationContext(request, serviceProvider.Object, null));

            // Assert
            Assert.NotEmpty(validationResult);
            Assert.Contains(nameof(SignTransactionRequest.TransactionContext), validationResult.First().MemberNames);
        }

        [Fact]
        public void ShouldNotValidate_IfPrivateKeysArrayIsNull()
        {
            // Arrange
            var body = JsonConvert.SerializeObject(new
            {
                PrivateKeys = Array.Empty<string>(),
                TransactionHex = Serializer.ToString((this.tx, (ICoin[])null))
            });

            // Act
            var request = JsonConvert.DeserializeObject<SignTransactionRequest>(body);
            var validationResult = request.Validate(new ValidationContext(request, serviceProvider.Object, null));

            // Assert
            Assert.NotEmpty(validationResult);
            Assert.Contains(nameof(SignTransactionRequest.PrivateKeys), validationResult.First().MemberNames);
        }

        [Fact]
        public void ShouldNotValidate_IfKeyIsInvalid()
        {
            // Arrange
            var body = JsonConvert.SerializeObject(new
            {
                PrivateKeys = new[] { "invalid" },
                TransactionContext = Serializer.ToString((this.tx, this.spentCoins))
            });

            // Act
            var request = JsonConvert.DeserializeObject<SignTransactionRequest>(body);
            var validationResult = request.Validate(new ValidationContext(request, serviceProvider.Object, null));

            // Assert
            Assert.NotEmpty(validationResult);
            Assert.Contains(nameof(SignTransactionRequest.PrivateKeys), validationResult.First().MemberNames);
        }
    }
}
