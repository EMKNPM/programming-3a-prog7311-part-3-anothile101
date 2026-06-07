
using Xunit;

namespace Practice_assignment.Tests
{ 
        public class CurrencyCalculationTests
        {
            // Conversion logic extracted from CurrencyService
            private static decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
                => Math.Round(usdAmount * rate, 2);


        // Tests the USD to ZAR currency conversion math.
            [Fact]
            public void Convert_100_Usd_At_Rate_18_50_Returns_1850()
            {
                var result = ConvertUsdToZar(100m, 18.50m);
                Assert.Equal(1850.00m, result);
            }

            [Fact]
            public void Convert_1_Usd_At_Rate_18_50_Returns_18_50()
            {
                var result = ConvertUsdToZar(1m, 18.50m);
                Assert.Equal(18.50m, result);
            }

            [Fact]
            public void Convert_Usd_Result_Is_Rounded_To_2_Decimal_Places()
            {
                
                var result = ConvertUsdToZar(10m, 18.333m);
                Assert.Equal(183.33m, result);
            }

            [Theory]
            [InlineData(100, 18.50, 1850.00)]
            [InlineData(250, 18.50, 4625.00)]
            [InlineData(1000, 18.50, 18500.00)]
            [InlineData(0.50, 18.50, 9.25)]
            [InlineData(99.99, 18.50, 1849.82)]
            public void Convert_Various_Amounts_Returns_Correct_Zar(
                decimal usd, decimal rate, decimal expectedZar)
            {
                var result = ConvertUsdToZar(usd, rate);
                Assert.Equal(expectedZar, result);
            }

            

            [Fact]
            public void Convert_Zero_Usd_Returns_Zero_Zar()
            {
                var result = ConvertUsdToZar(0m, 18.50m);
                Assert.Equal(0.00m, result);
            }

            [Fact]
            public void Convert_Very_Small_Amount_Rounds_Correctly()
            {
                
                var result = ConvertUsdToZar(0.01m, 18.50m);
                Assert.Equal(0.18m, result);
            }

            [Fact]
            public void Convert_Large_Amount_Does_Not_Overflow()
            {
                // 1 million USD
                var result = ConvertUsdToZar(1_000_000m, 18.50m);
                Assert.Equal(18_500_000.00m, result);
            }

            [Fact]
            public void Convert_With_High_Precision_Rate_Rounds_Result_Correctly()
            {
                // Rate with many decimals result must still be 2dp
                var result = ConvertUsdToZar(100m, 18.123456m);
                Assert.Equal(1812.35m, result);
            }

            [Fact]
            public void Convert_Result_Never_Has_More_Than_2_Decimal_Places()
            {
                var result = ConvertUsdToZar(33m, 18.33m);
                // Check that decimal places are at most 2
                var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(result)[3])[2];
                Assert.True(decimalPlaces <= 2);
            }
        }
    }
