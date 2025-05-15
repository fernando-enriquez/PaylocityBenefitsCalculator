using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Api.Dtos.Paycheck;
using Api.Models;
using System.Net.Http.Json;

namespace ApiTests.IntegrationTests
{
    public class PaycheckIntegrationTest: IntegrationTest
    {
    [Fact]
        //test for the correct calculated values
        public async Task Should_Calculate_Paycheck_For_Employee_5()
        {
            var response = await HttpClient.GetAsync("/api/v1/paycheck/5");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaycheckDto>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();


            var paycheck = apiResponse.Data!;
            paycheck.Employee.FirstName.Should().Be("Laura");
            paycheck.GrossPayment.Should().BeApproximately(3269.23m, 0.01m);
            paycheck.NetPayment.Should().BeApproximately(2096.15m, 0.01m);
            paycheck.TotalDeductionPerPayCheck.Should().BeApproximately(1173.07m, 0.01m);
        }
    }
}
