
using Xunit;
using Practice_assignment.Models;
using Practice_assignment.Patterns.Factory;

namespace Practice_assignment.Tests
{
        // Tests the ContractFactory design pattern implementation.

        public class ContractFactoryTests
        {
            private readonly IContractFactory _factory = new ContractFactory();

            // Factory Output Tests 

            [Fact]
            public void Factory_Creates_Contract_With_Correct_ClientId()
            {
                var contract = _factory.CreateContract(
                    clientId: 42,
                    startDate: DateTime.Today,
                    endDate: DateTime.Today.AddYears(1),
                    serviceLevel: "Bronze");

                Assert.Equal(42, contract.ClientId);
            }

            [Fact]
            public void Factory_Creates_Contract_With_Correct_Dates()
            {
                var start = new DateTime(2026, 1, 1);
                var end = new DateTime(2026, 12, 31);

                var contract = _factory.CreateContract(1, start, end, "Silver");

                Assert.Equal(start, contract.StartDate);
                Assert.Equal(end, contract.EndDate);
            }

            [Fact]
            public void Factory_Creates_Contract_With_Correct_ServiceLevel()
            {
                var contract = _factory.CreateContract(1, DateTime.Today,
                    DateTime.Today.AddYears(1), "Silver");

                Assert.Equal("Silver", contract.ServiceLevel);
            }

            // Status logic 
            [Fact]
            public void Gold_Contract_Defaults_To_Active_Status()
            {
                // Gold clients are immediately Active
                var contract = _factory.CreateContract(1, DateTime.Today,
                    DateTime.Today.AddYears(1), "Gold");

                Assert.Equal(ContractStatus.Active, contract.Status);
            }

            [Fact]
            public void Silver_Contract_Defaults_To_Draft_Status()
            {
                var contract = _factory.CreateContract(1, DateTime.Today,
                    DateTime.Today.AddYears(1), "Silver");

                Assert.Equal(ContractStatus.Draft, contract.Status);
            }

            [Fact]
            public void Bronze_Contract_Defaults_To_Draft_Status()
            {
                var contract = _factory.CreateContract(1, DateTime.Today,
                    DateTime.Today.AddYears(1), "Bronze");

                Assert.Equal(ContractStatus.Draft, contract.Status);
            }

            [Theory]
            [InlineData("Silver", ContractStatus.Draft)]
            [InlineData("Bronze", ContractStatus.Draft)]
            [InlineData("Gold", ContractStatus.Active)]
            public void Factory_Sets_Correct_Status_Per_ServiceLevel(
                string serviceLevel, ContractStatus expectedStatus)
            {
                var contract = _factory.CreateContract(1, DateTime.Today,
                    DateTime.Today.AddYears(1), serviceLevel);

                Assert.Equal(expectedStatus, contract.Status);
            }

            [Fact]
            public void Factory_Returns_New_Instance_Each_Call()
            {
                // Factory must return a new object every time
                var c1 = _factory.CreateContract(1, DateTime.Today, DateTime.Today.AddYears(1), "Gold");
                var c2 = _factory.CreateContract(2, DateTime.Today, DateTime.Today.AddYears(1), "Gold");

                Assert.NotSame(c1, c2);
                Assert.Equal(1, c1.ClientId);
                Assert.Equal(2, c2.ClientId);
            }
        }
    }
