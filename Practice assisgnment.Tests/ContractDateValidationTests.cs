
using Xunit;
using Practice_assignment.Models;

namespace Practice_assisgnment.Tests
{
   
        // Tests the contract date validation EndDate must always be after StartDate.
        
        public class ContractDateValidationTests
        {
            // Mirrors the validation added to ContractService.CreateContractAsync
            private static void ValidateContractDates(DateTime startDate, DateTime endDate)
            {
                if (endDate <= startDate)
                    throw new InvalidOperationException(
                        "End date must be after start date.");
            }

           

            [Fact]
            public void Valid_Date_Range_Does_Not_Throw()
            {
                var ex = Record.Exception(() =>
                    ValidateContractDates(
                        new DateTime(2026, 1, 1),
                        new DateTime(2026, 12, 31)));
                Assert.Null(ex);
            }

            [Fact]
            public void One_Day_Contract_Is_Valid()
            {
                var start = DateTime.Today;
                var end = DateTime.Today.AddDays(1);
                var ex = Record.Exception(() => ValidateContractDates(start, end));
                Assert.Null(ex);
            }

            [Fact]
            public void Multi_Year_Contract_Is_Valid()
            {
                var ex = Record.Exception(() =>
                    ValidateContractDates(
                        new DateTime(2020, 1, 1),
                        new DateTime(2030, 12, 31)));
                Assert.Null(ex);
            }

            // Invalid Date Ranges
            [Fact]
            public void End_Date_Before_Start_Date_Throws()
            {
                var ex = Assert.Throws<InvalidOperationException>(() =>
                    ValidateContractDates(
                        new DateTime(2026, 6, 1),
                        new DateTime(2026, 1, 1)));
                Assert.Contains("End date must be after start date", ex.Message);
            }

            [Fact]
            public void Same_Start_And_End_Date_Throws()
            {
                // The same day is not a valid contract period
                var date = new DateTime(2026, 6, 15);
                Assert.Throws<InvalidOperationException>(
                    () => ValidateContractDates(date, date));
            }

            [Fact]
            public void End_Date_One_Second_Before_Start_Throws()
            {
                var start = new DateTime(2026, 6, 1, 12, 0, 0);
                var end = new DateTime(2026, 6, 1, 11, 59, 59);
                Assert.Throws<InvalidOperationException>(
                    () => ValidateContractDates(start, end));
            }
        }
    }