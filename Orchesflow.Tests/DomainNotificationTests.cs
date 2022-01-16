using FluentAssertions;
using Moq.AutoMock;
using Orchesflow.Notifications;
using Xunit;

namespace Orchesflow.Tests
{
    public class DomainNotificationTests
    {
        private readonly AutoMocker _mocker;
        public DomainNotificationTests()
        {
            _mocker = new AutoMocker();
        }


        [Fact]
        public void AddNotification_ShouldAddNotification_AndContainerShouldHaveNotifications()
        {
            // Arrange
            IDomainNotifications domainNotifications = _mocker.CreateInstance<DomainNotifications>();
            string notification = "My notification";

            // Act
            domainNotifications.AddNotification(notification);

            // Assert
            domainNotifications.GetAll().Should().HaveCount(1).And.Contain(notification);
            domainNotifications.HasNotifications().Should().BeTrue();
        }

        [Fact]
        public void RemoveNotifications_ShouldRemoveNotiication_AndContainerShouldHaveNoNotificatoins()
        {
            // Arrange
            IDomainNotifications domainNotifications = _mocker.CreateInstance<DomainNotifications>();
            string notification = "My notification";
            domainNotifications.AddNotification(notification);

            // Act
            domainNotifications.CleanNotifications();

            // Assert
            domainNotifications.GetAll().Should().BeEmpty();
            domainNotifications.HasNotifications().Should().BeFalse();
        }
    }
}
