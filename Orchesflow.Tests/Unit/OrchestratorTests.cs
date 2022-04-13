using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using Orchesflow.Events;
using Orchesflow.Notifications;
using Orchesflow.Orchestration;
using Orchesflow.Tests.Unit.Fakes;
using Orchesflow.UnitOfWork;
using Xunit;

namespace Orchesflow.Tests.Unit
{
    public class OrchestratorTests
    {
        private readonly AutoMocker _mocker;

        public OrchestratorTests()
        {
            _mocker = new AutoMocker();
        }

        [Fact]
        public async Task SendCommand_ShouldReturnError_WhenNotificationContainerHasMessage()
        {
            // Arrange
            var request = new FakeRequest();
            var orchestrator = _mocker.CreateInstance<Orchestrator<FakeDbContext>>();
            var errors = new List<string> {"Generic error 1", "Generic error 2"};

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.HasNotifications())
                .Returns(true);

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.GetAll())
                .Returns(errors);

            // Act
            var sut = await orchestrator.SendCommand<FakeRequest, FakeResponse>(request);

            // Assert
            sut.Data.Should().BeNull();
            sut.Success.Should().BeFalse();
            sut.Messages.Should().BeEquivalentTo(errors);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FirePreCommitEvents(), Times.Once);
            _mocker.GetMock<IUnitOfWork<FakeDbContext>>().Verify(x => x.Commit(), Times.Never);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FireAfterCommitEvents(), Times.Never);
        }

        [Fact]
        public async Task SendCommand_ShouldCommitAndNotFireAfterCommitEvents_WhenCommitErrorOccurrs()
        {
            // Arrange
            var request = new FakeRequest();
            var orchestrator = _mocker.CreateInstance<Orchestrator<FakeDbContext>>();
            var errors = new List<string> {"Failed to record"};

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.HasNotifications())
                .Returns(false);

            _mocker.GetMock<IUnitOfWork<FakeDbContext>>()
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.GetAll())
                .Returns(errors);

            // Act
            var sut = await orchestrator.SendCommand<FakeRequest, FakeResponse>(request);

            // Assert
            sut.Data.Should().BeNull();
            sut.Success.Should().BeFalse();
            sut.Messages.Should().BeEquivalentTo(errors);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FirePreCommitEvents(), Times.Once);
            _mocker.GetMock<IUnitOfWork<FakeDbContext>>().Verify(x => x.Commit(), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FireAfterCommitEvents(), Times.Never);
        }

        [Fact]
        public async Task SendCommand_ShouldCommitAndFireAfterCommitEvents_WhenNoCommitErrorOccurrs()
        {
            // Arrange
            var request = new FakeRequest();
            var response = new FakeResponse(1);
            var orchestrator = _mocker.CreateInstance<Orchestrator<FakeDbContext>>();

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.HasNotifications())
                .Returns(false);

            _mocker.GetMock<IUnitOfWork<FakeDbContext>>()
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.GetAll())
                .Returns(new List<string>());

            _mocker.GetMock<IMediator>()
                .Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var sut = await orchestrator.SendCommand<FakeRequest, FakeResponse>(request);

            // Assert
            sut.Data.Should().BeEquivalentTo(response);
            sut.Success.Should().BeTrue();
            sut.Messages.Should().BeEmpty();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FirePreCommitEvents(), Times.Once);
            _mocker.GetMock<IUnitOfWork<FakeDbContext>>().Verify(x => x.Commit(), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FireAfterCommitEvents(), Times.Once);
        }

        [Fact]
        public async Task SendQuery_ShouldReturnError_WhenNotificationContainerHasMessage()
        {
            // Arrange
            var request = new FakeRequest();
            var orchestrator = _mocker.CreateInstance<Orchestrator<FakeDbContext>>();
            var errors = new List<string> {"Generic error 1", "Generic error 2"};

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.HasNotifications())
                .Returns(true);

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.GetAll())
                .Returns(errors);

            // Act
            var sut = await orchestrator.SendQuery(request);

            // Assert
            sut.Data.Should().BeNull();
            sut.Success.Should().BeFalse();
            sut.Messages.Should().BeEquivalentTo(errors);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FirePreCommitEvents(), Times.Never);
            _mocker.GetMock<IUnitOfWork<FakeDbContext>>().Verify(x => x.Commit(), Times.Never);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FireAfterCommitEvents(), Times.Never);
        }

        [Fact]
        public async Task SendQuery_ShouldReturnSuccess_WhenNotificationContainerHasNoMessage()
        {
            // Arrange
            var request = new FakeRequest();
            var response = new FakeResponse(1);
            var orchestrator = _mocker.CreateInstance<Orchestrator<FakeDbContext>>();

            _mocker.GetMock<IMediator>()
                .Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.HasNotifications())
                .Returns(false);

            _mocker.GetMock<IDomainNotifications>()
                .Setup(x => x.GetAll())
                .Returns(new List<string>());

            // Act
            var sut = await orchestrator.SendQuery(request);

            // Assert
            sut.Data.Should().BeEquivalentTo(response);
            sut.Success.Should().BeTrue();
            sut.Messages.Should().BeEmpty();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FirePreCommitEvents(), Times.Never);
            _mocker.GetMock<IUnitOfWork<FakeDbContext>>().Verify(x => x.Commit(), Times.Never);
            _mocker.GetMock<IEventDispatcher>().Verify(x => x.FireAfterCommitEvents(), Times.Never);
        }
    }
}