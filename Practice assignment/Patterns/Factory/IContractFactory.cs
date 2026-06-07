using Practice_assignment.Models;

namespace Practice_assignment.Patterns.Factory
{
    /* The factory pattern abstracts the creation of Contract objects based on the service level
     This decouples the controller from the specifics of how each contract type is initialized */
    public interface IContractFactory
    {
            Contract CreateContract(int clientId, DateTime startDate, DateTime endDate, string serviceLevel);
        }
    }

