using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_assignment.Models
{
    public enum ContractStatus // Enum for contract status with default values
    {
        Draft,
        Active,
        Expired,
        OnHold
    }
    public class Contract
    {
            public int Id { get; set; }

            [Required]
            public int ClientId { get; set; } // Foreign key to Client

            [Required(ErrorMessage = "Start date is required.")] // Validation for required start date
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "End date is required.")] // Validation for required end date
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }

            [Required]
            public ContractStatus Status { get; set; } = ContractStatus.Draft;

            [Required(ErrorMessage = "Service level is required.")]
            [StringLength(200)]
            public string ServiceLevel { get; set; } = string.Empty;

            // File handling path stored in DB, file on disk
            public string? SignedAgreementPath { get; set; }
            public string? SignedAgreementFileName { get; set; }

            // Navigation
            [ForeignKey("ClientId")]
            public Client? Client { get; set; }
            public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        }
    }

