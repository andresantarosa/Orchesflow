using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Orchesflow.Example.Handlers.AddCustomer;
using Orchesflow.Example.TestExecutionVerifiers;
using Xunit;

namespace Orchesflow.Tests.Integration;

public class FallbackTests : Fixture
{
    [Fact]
    public async Task AddCustomer_ShouldFallbackPreCommitEventEvent1_WhenErrorsOccursInPreCommitEventEvent2()
    {
        try
        {
            // Arrange
            var command = new AddCustomerCommand()
            {
                Name = "Abc"
            };
            // Act
            var response = await HttpClient.PostAsJsonAsync("api/customer", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            TestExecutionVerify.Executions.Count.Should().Be(2);
            TestExecutionVerify.Executions.First().Should().Be(("AddCustomerDummyPreEvent1Handler", true));
            TestExecutionVerify.Executions.Skip(1).First().Should().Be(("AddCustomerDummyPreEvent1HandlerFallback", false));
        }
        finally
        {
            TestExecutionVerify.Restart();
        }
    }
    
    [Fact]
    public async Task AddCustomer_ShouldFallbackAllPreCommitEvents_WhenErrorsOccursInCommit()
    {
        try
        {
            // Arrange
            var command = new AddCustomerCommand()
            {
                // 51 chars will cause commit failure
                Name = "qkbkhapnclufkkorsubihcrsgcfjrqgncweasmndaicqermwiqkfdfadsfdafas"
            };
            // Act
            var response = await HttpClient.PostAsJsonAsync("api/customer", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            TestExecutionVerify.Executions.Count.Should().Be(5);
            TestExecutionVerify.Executions.First().Should().Be(("AddCustomerDummyPreEvent1Handler", true));
            TestExecutionVerify.Executions.Skip(1).First().Should().Be(("AddCustomerDummyPreEvent2Handler", true));
            TestExecutionVerify.Executions.Skip(2).First().Should().Be(("AddCustomerDummyPreEvent3Handler", true));
            TestExecutionVerify.Executions.Skip(3).First().Should().Be(("AddCustomerDummyPreEvent2HandlerFallback", false));
            TestExecutionVerify.Executions.Skip(4).First().Should().Be(("AddCustomerDummyPreEvent1HandlerFallback", false));

        }
        finally
        {
            TestExecutionVerify.Restart();
        }

    }
    
    [Fact]
    public async Task AddCustomer_ShouldFallbackAfterCommitEventEvent1_WhenErrorsOccursInAfterCommitEventEvent2()
    {
        try
        {
            // Arrange
            var command = new AddCustomerCommand()
            {
                Name = "Def"
            };
            // Act
            var response = await HttpClient.PostAsJsonAsync("api/customer", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            TestExecutionVerify.Executions.Count.Should().Be(8);
            TestExecutionVerify.Executions.First().Should().Be(("AddCustomerDummyPreEvent1Handler", true));
            TestExecutionVerify.Executions.Skip(1).First().Should().Be(("AddCustomerDummyPreEvent2Handler", true));
            TestExecutionVerify.Executions.Skip(2).First().Should().Be(("AddCustomerDummyPreEvent3Handler", true));
            TestExecutionVerify.Executions.Skip(3).First().Should().Be(("AddCustomerDummyAfterEvent1Handler", true));
            TestExecutionVerify.Executions.Skip(4).First().Should().Be(("AddCustomerDummyAfterEvent1HandlerFallback", false));
            TestExecutionVerify.Executions.Skip(5).First().Should().Be(("HandlerFallback", false));
            TestExecutionVerify.Executions.Skip(6).First().Should().Be(("AddCustomerDummyPreEvent2HandlerFallback", false));
            TestExecutionVerify.Executions.Skip(7).First().Should().Be(("AddCustomerDummyPreEvent1HandlerFallback", false));


        }
        finally
        {
            TestExecutionVerify.Restart();
        }
        
    }
}