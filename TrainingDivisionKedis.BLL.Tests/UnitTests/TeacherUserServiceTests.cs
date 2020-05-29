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
    public class TeacherUserServiceTests
    {
        TeacherUserService _sut;

        private static List<SPAuthenticateUser> GetTestAuthenticateUser()
        {
            return new List<SPAuthenticateUser> {
                new SPAuthenticateUser { Id = 1, Login = "Login1", Name = "Name1", Role = "Role1" },
                new SPAuthenticateUser { Id = 2, Login = "Login1", Name = "Name1", Role = "Role2" }
            };
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(ITeachersQuery teachersQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .Options;
            var mockDbContext = new AppDbContext(options);

            QueryExtensions.TeachersQueryFactory = context => teachersQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(mockDbContext);
            return mockDbContextFactory;
        }

        [Fact]
        public void Constructor_ShouldThrowErrorWhenArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut = new TeacherUserService(null));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnDto()
        {
            // ARRANGE
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(GetTestAuthenticateUser());

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
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
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<SPAuthenticateUser>());

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
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
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
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
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
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
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);

            // ACT
            var actual = await _sut.ChangePasswordAsync(null);

            //ASSERT
            Assert.NotNull(actual.Error);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.ChangePassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
            var request = new ChangeUserPasswordRequest { OldPassword = "123", NewPassword = "123456", Id = 1 };

            // ACT
            var actual = await _sut.ChangePasswordAsync(request);

            //ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }

        [Fact]
        public async Task ChangeLoginAsync_ShouldReturnTrue()
        {
            // ARRANGE
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.ChangeLogin(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
            var request = new ChangeUserLoginRequest { Id = 123, NewLogin = "New login" };

            // ACT
            var actual = await _sut.ChangeLoginAsync(request);

            //ASSERT
            Assert.True(actual.Entity);
        }

        [Fact]
        public async Task ChangeLoginAsync_ShouldReturnErrorWhenExceptionInQuery()
        {
            // ARRANGE
            var mockQuery = new Mock<ITeachersQuery>();
            mockQuery
                .Setup(s => s.ChangeLogin(It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mock exception"));

            var mockContextFactory = SetupContextFactory(mockQuery.Object);
            _sut = new TeacherUserService(mockContextFactory.Object);
            var request = new ChangeUserLoginRequest { Id = 123, NewLogin = "New login" };

            // ACT
            var actual = await _sut.ChangeLoginAsync(request);

            //ASSERT
            Assert.Equal("Mock exception", actual.Error.Message);
        }
    }
}
