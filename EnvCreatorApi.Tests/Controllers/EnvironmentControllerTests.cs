using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EnvCreatorApi.Controllers;
using EnvCreatorApi.Data;
using EnvCreatorApi.DTOs;
using EnvCreatorApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EnvCreatorApi.Tests
{
	[TestClass]
	public class EnvironmentControllerTests
	{
		private EnvCreatorContext _context;
		private Mock<UserManager<User>> _userManagerMock;
		private EnvironmentController _controller;
		private string _userId = "test-user-id";

		[TestInitialize]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<EnvCreatorContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new EnvCreatorContext(options);
			_context.Database.EnsureDeleted();
			_context.Database.EnsureCreated();

			_context.Users.Add(new User
			{
				Id = _userId,
				UserName = "testuser"
			});
			_context.SaveChanges();

			var store = new Mock<IUserStore<User>>();
			_userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

			_controller = new EnvironmentController(_context, _userManagerMock.Object);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, _userId)
					}, "mock"))
				}
			};
		}

		[TestMethod]
		public async Task CreateEnvironment_Should_Return_BadRequest_When_User_Has_Too_Many_Worlds()
		{
			// Arrange
			for (int i = 0; i < 5; i++)
			{
				_context.Environments.Add(new Environment2D
				{
					Name = $"World{i}",
					UserId = _userId,
					MaxHeight = 100,
					MaxWidth = 100
				});
			}
			await _context.SaveChangesAsync();

			var dto = new EnvironmentDto
			{
				Name = "TooMuch",
				MaxHeight = 100,
				MaxWidth = 100
			};

			// Act
			var result = await _controller.CreateEnvironment(dto);

			// Assert
			Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
		}

		[TestMethod]
		public async Task GetMyEnvironments_Should_Return_Only_User_Environments()
		{
			// Arrange
			_context.Environments.Add(new Environment2D { Name = "World1", UserId = _userId });
			_context.Environments.Add(new Environment2D { Name = "World2", UserId = "another-user" });
			await _context.SaveChangesAsync();

			// Act
			var result = await _controller.GetMyEnvironments();

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
			var okResult = result as OkObjectResult;
			Assert.IsNotNull(okResult);

			// Omdat controller waarschijnlijk anonieme objecten of DTO's terugstuurt:
			var environments = (okResult.Value as IEnumerable<object>)?.ToList();
			Assert.IsNotNull(environments);
			Assert.AreEqual(1, environments.Count);
		}

		[TestMethod]
		public async Task AddObjectToEnvironment_Should_Return_NotFound_For_Foreign_Environment()
		{
			// Arrange
			var env = new Environment2D { Id = 99, Name = "OtherEnv", UserId = "another-user" };
			_context.Environments.Add(env);
			await _context.SaveChangesAsync();

			var dto = new ObjectDto
			{
				EnvironmentId = 99,
				PrefabId = "tree",
				PositionX = 0,
				PositionY = 0,
				ScaleX = 1,
				ScaleY = 1,
				RotationZ = 0,
				SortingLayer = 0
			};

			// Act
			var result = await _controller.AddObjectToEnvironment(dto);

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
		}
	}
}
