using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.DTO.User;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.SPModels.User;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.Extensions;
using Xunit;

namespace TrainingDivisionKedis.BLL.Tests
{
    public class StudentUserServiceTests
    {
        StudentUserService _sut;

        private static Mock<IAppDbContextFactory> SetupContextFactory(IStudentsQuery studentsQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .Options;
            var dbContext = new AppDbContext(options);

            QueryExtensions.StudentsQueryFactory = context => studentsQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public void Constructor_ShouldThrowErrorWhenArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut = new StudentUserService(null));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnDto()
        {
            // ARRANGE
            var mockQuery = new Mock<IStudentsQuery>();
            mockQuery
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string login, string pass) => new SPAuthenticateUser { Id = 1, Login = login, Name = "Name1", Role = "Role1" });

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new StudentUserService(mockContextFactory.Object);
            var userDto = new UserDto { Login = "SomeLogin", Password = "123456" };

            // ACT
            var actual = await _sut.AuthenticateAsync(userDto);
            userDto.Id = 1;
            userDto.Name = "Name1";
            userDto.Roles = new List<string> { "Role1" };

            //ASSERT
            Assert.Equal(ComparableObject.Convert(userDto), ComparableObject.Convert(actual.Entity));
            Assert.Null(actual.Entity.Password);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnErrorWhenQueryReturnsNull()
        {
            // ARRANGE
            var mockQuery = new Mock<IStudentsQuery>();
            mockQuery
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((SPAuthenticateUser) null);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new StudentUserService(mockContextFactory.Object);
            var userDto = new UserDto { Login = "SomeLogin", Password = "123456" };

            // ACT
            var actual = await _sut.AuthenticateAsync(userDto);

            //ASSERT
            Assert.Equal("Неверный логин или пароль", actual.Error.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IStudentsQuery>();
            mockQuery
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new StudentUserService(mockContextFactory.Object);
            var userDto = new UserDto { Login = "SomeLogin", Password = "123456" };

            // ACT
            var actual = await _sut.AuthenticateAsync(userDto);

            //ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var mockQuery = new Mock<IStudentsQuery>();
            mockQuery
                .Setup(s => s.ChangePassword(It.IsAny<int>(),It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new StudentUserService(mockContextFactory.Object);
            var request = new ChangeUserPasswordRequest { OldPassword = "123", NewPassword = "123456", Id = 1 };

            // ACT
            var actual = await _sut.ChangePasswordAsync(request);

            //ASSERT
            Assert.True(actual.Entity);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnErrorWhenArgumentIsNull()
        {
            // ARRANGE
            var mockQuery = new Mock<IStudentsQuery>();
            mockQuery
                .Setup(s => s.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new StudentUserService(mockContextFactory.Object);

            // ACT
            var actual = await _sut.ChangePasswordAsync(null);

            //ASSERT
            Assert.NotNull(actual.Error);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<IStudentsQuery>();
            mockQuery
                .Setup(s => s.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new StudentUserService(mockContextFactory.Object);
            var request = new ChangeUserPasswordRequest { OldPassword = "123", NewPassword = "123456", Id = 1 };

            // ACT
            var actual = await _sut.ChangePasswordAsync(request);

            //ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }
    }
}
