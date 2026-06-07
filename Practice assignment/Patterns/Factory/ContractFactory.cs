using Practice_assignment.Models;

namespace Practice_assignment.Patterns.Factory
{ 
    public class ContractFactory : IContractFactory
    { // Factory produces a contract with default status based on service level
        public Contract CreateContract(int clientId, DateTime startDate, DateTime endDate, string serviceLevel)
            {
                var status = serviceLevel.Equals("Gold", StringComparison.OrdinalIgnoreCase) // Gold contracts default to active whilst others default to draft
                    ? ContractStatus.Active
                    : ContractStatus.Draft;

                return new Contract   
                {
                    ClientId = clientId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ServiceLevel = serviceLevel,
                    Status = status
                };
            }
        }
    }

