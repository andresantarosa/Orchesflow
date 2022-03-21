using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using Orchesflow.Notifications;
using Orchesflow.UnitOfWork;
using Xunit;

namespace Orchesflow.Tests.Unit
{
    public class UnitOfWorkTests
    {
        private readonly AutoMocker _mocker;
        public UnitOfWorkTests()
        {
            _mocker = new AutoMocker();
        }

        [Fact]
        public async Task Commit_ShouldReturnTrue_WhenNoExceptionOccurs()
        {
            // Arrange
            var domainNotifications = _mocker.GetMock<IDomainNotifications>();
            DbContextOptions<DbContext> dummyOptions = new DbContextOptionsBuilder<DbContext>().Options;
            var dbContextMock = new DbContextMock<DbContext>(dummyOptions);
            var unitOfWork = new UnitOfWork<DbContext>(dbContextMock.Object, domainNotifications.Object);

            // Act
            var commitResponse = await unitOfWork.Commit();

            // Assert
            commitResponse.Should().BeTrue();
            domainNotifications.Verify(x => x.AddNotification(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Commit_ShouldReturnFalseTrue_WhenExceptionIsThorwn()
        {
            // Arrange
            var domainNotifications = _mocker.GetMock<IDomainNotifications>();
            DbContextOptions<DbContext> dummyOptions = new DbContextOptionsBuilder<DbContext>().Options;
            var dbContextMock = new DbContextMock<DbContext>(dummyOptions);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Callback(() => throw new Exception("Generic database error"));
            var unitOfWork = new UnitOfWork<DbContext>(dbContextMock.Object, domainNotifications.Object);

            // Act
            var commitResponse = await unitOfWork.Commit();
            // Assert
            commitResponse.Should().BeFalse();
            domainNotifications.Verify(x => x.AddNotification("Generic database error"), Times.Never);
        }
    }
}
