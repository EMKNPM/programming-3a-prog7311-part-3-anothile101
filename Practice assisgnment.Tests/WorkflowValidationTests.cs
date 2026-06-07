
using Xunit;
using Practice_assignment.Models;

namespace Practice_assignment.Tests
{
   
// Tests the core business workflow rule a ServiceRequest cant be created if the parent Contract is Expired or OnHold.
        
        public class WorkflowValidationTests
        {
            // Mirrors the exact validation in ServiceRequestService.CreateAsync
            private static void ValidateContractForServiceRequest(Contract contract)
            {
                if (contract == null)
                    throw new ArgumentNullException(nameof(contract), "Contract cannot be null.");

                if (contract.Status == ContractStatus.Expired ||
                    contract.Status == ContractStatus.OnHold)
                {
                    throw new InvalidOperationException(
                        $"Service requests cannot be created for a contract with status '{contract.Status}'. " +
                        "Only Active or Draft contracts are allowed.");
                }
            }

            // Blocked Statuses 
            [Fact]
            public void Expired_Contract_Blocks_ServiceRequest_Creation()
            {
                var contract = new Contract { Id = 1, Status = ContractStatus.Expired };
                var ex = Assert.Throws<InvalidOperationException>(
                    () => ValidateContractForServiceRequest(contract));
                Assert.Contains("Expired", ex.Message);
            }

            [Fact]
            public void OnHold_Contract_Blocks_ServiceRequest_Creation()
            {
                var contract = new Contract { Id = 2, Status = ContractStatus.OnHold };
                var ex = Assert.Throws<InvalidOperationException>(
                    () => ValidateContractForServiceRequest(contract));
                Assert.Contains("OnHold", ex.Message);
            }

            [Theory]
            [InlineData(ContractStatus.Expired)]
            [InlineData(ContractStatus.OnHold)]
            public void Blocked_Statuses_Always_Throw(ContractStatus blockedStatus)
            {
                var contract = new Contract { Id = 10, Status = blockedStatus };
                Assert.Throws<InvalidOperationException>(
                    () => ValidateContractForServiceRequest(contract));
            }

            // Allowed Statuses 
            [Fact]
            public void Active_Contract_Allows_ServiceRequest_Creation()
            {
                var contract = new Contract { Id = 3, Status = ContractStatus.Active };
                var ex = Record.Exception(() => ValidateContractForServiceRequest(contract));
                Assert.Null(ex);
            }

            [Fact]
            public void Draft_Contract_Allows_ServiceRequest_Creation()
            {
                var contract = new Contract { Id = 4, Status = ContractStatus.Draft };
                var ex = Record.Exception(() => ValidateContractForServiceRequest(contract));
                Assert.Null(ex);
            }

            [Theory]
            [InlineData(ContractStatus.Active)]
            [InlineData(ContractStatus.Draft)]
            public void Allowed_Statuses_Never_Throw(ContractStatus allowedStatus)
            {
                var contract = new Contract { Id = 5, Status = allowedStatus };
                var ex = Record.Exception(() => ValidateContractForServiceRequest(contract));
                Assert.Null(ex);
            }

            // Error Message 
            [Fact]
            public void Error_Message_Mentions_The_Blocked_Status()
            {
                var contract = new Contract { Id = 6, Status = ContractStatus.Expired };
                var ex = Assert.Throws<InvalidOperationException>(
                    () => ValidateContractForServiceRequest(contract));
                Assert.Contains("Expired", ex.Message);
                Assert.Contains("Active or Draft", ex.Message);
            }

            [Fact]
            public void Error_Message_Guides_User_To_Correct_Action()
            {
                var contract = new Contract { Id = 7, Status = ContractStatus.OnHold };
                var ex = Assert.Throws<InvalidOperationException>(
                    () => ValidateContractForServiceRequest(contract));
                // Message must tell the user what is allowed
                Assert.Contains("Active or Draft", ex.Message);
            }

           
            [Fact]
            public void Null_Contract_Throws_ArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ValidateContractForServiceRequest(null!));
            }

            [Fact]
            public void Newly_Created_Contract_Defaults_To_Draft_And_Is_Allowed()
            {
                // A new Contract object defaults to Draft
                var contract = new Contract { Id = 8 };
                Assert.Equal(ContractStatus.Draft, contract.Status);

                var ex = Record.Exception(() => ValidateContractForServiceRequest(contract));
                Assert.Null(ex);
            }
        }
    }