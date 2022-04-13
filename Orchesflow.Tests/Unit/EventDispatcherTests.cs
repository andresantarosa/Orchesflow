using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using Orchesflow.Events;
using Orchesflow.Tests.Unit.Fakes;
using Xunit;

namespace Orchesflow.Tests.Unit
{
    public class EventDispatcherTests
    {
        private readonly AutoMocker _mocker;
        public EventDispatcherTests()
        {
            _mocker = new AutoMocker();
        }

        [Fact]
        public void AddPreCommitEvent_ShouldAddPreCommitEvent_AndHaveCountEquals1()
        {
            // Arrange
            var eventDispatcher = _mocker.CreateInstance<EventDispatcher>();
            var notification = new FakeNotification(1, "event");

            // Act
            eventDispatcher.AddPreCommitEvent(notification);

            // Assert
            eventDispatcher.GetPreCommitEvents().Should().HaveCount(1);
            eventDispatcher.GetPreCommitEvents().FirstOrDefault().Should().BeEquivalentTo(notification);

        }

        [Fact]
        public void RemovePreCommitEvent_ShouldRemovePreCommitEvent_AndHaveCountEquals0()
        {
            // Arrange
            var eventDispatcher = _mocker.CreateInstance<EventDispatcher>();
            var notification = new FakeNotification(1, "event");
            eventDispatcher.AddPreCommitEvent(notification);

            // Act
            eventDispatcher.RemovePreCommitEvent(notification);

            // Assert
            eventDispatcher.GetPreCommitEvents().Should().BeEmpty();
        }

        [Fact]
        public async Task FirePreCommitEvent_ShouldFirePreCommitEvent_WithNoErrors()
        {
            // Arrange
            _mocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(new List<INotification>());
            var eventDispatcher = _mocker.CreateInstance<EventDispatcher>();
            var notification1 = new FakeNotification(1, "event1");
            var notification2 = new FakeNotification(2, "event2");

            eventDispatcher.AddPreCommitEvent(notification1);
            eventDispatcher.AddPreCommitEvent(notification2);


            // Act
            await eventDispatcher.FirePreCommitEvents();

            // Assert
            var notifications = eventDispatcher.GetPreCommitEvents();
            foreach (var notification in notifications)
                _mocker.GetMock<IMediator>().Verify(x => x.Publish(notification, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IMediator>().Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        }

        [Fact]
        public void AddAfterCommitEvent_ShouldAddAfterCommitEvent_AndHaveCountEquals1()
        {
            // Arrange
            var eventDispatcher = _mocker.CreateInstance<EventDispatcher>();
            var notification = new FakeNotification(1, "event");

            // Act
            eventDispatcher.AddAfterCommitEvent(notification);

            // Assert
            eventDispatcher.GetAfterCommitEvents().Should().HaveCount(1);
            eventDispatcher.GetAfterCommitEvents().FirstOrDefault().Should().BeEquivalentTo(notification);

        }

        [Fact]
        public void RemoveAfterCommitEvent_ShouldRemoveAfterCommitEvent_AndHaveCountEquals0()
        {
            // Arrange
            var eventDispatcher = _mocker.CreateInstance<EventDispatcher>();
            var notification = new FakeNotification(1, "event");
            eventDispatcher.AddAfterCommitEvent(notification);

            // Act
            eventDispatcher.RemoveAfterCommitEvent(notification);

            // Assert
            eventDispatcher.GetAfterCommitEvents().Should().BeEmpty();
        }

        [Fact]
        public async Task FireAfterCommitEvent_ShouldFireAfterCommitEvent_WithNoErrors()
        {
            // Arrange
            _mocker.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(new List<INotification>());
            var eventDispatcher = _mocker.CreateInstance<EventDispatcher>();
            var notification1 = new FakeNotification(1, "event1");
            var notification2 = new FakeNotification(2, "event2");

            eventDispatcher.AddAfterCommitEvent(notification1);
            eventDispatcher.AddAfterCommitEvent(notification2);


            // Act
            await eventDispatcher.FireAfterCommitEvents();

            // Assert
            var notifications = eventDispatcher.GetAfterCommitEvents();
            foreach (var notification in notifications)
                _mocker.GetMock<IMediator>().Verify(x => x.Publish(notification, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IMediator>().Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        }
    }
}
