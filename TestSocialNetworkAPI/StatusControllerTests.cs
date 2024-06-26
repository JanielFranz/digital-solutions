using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialNetwork.API.Interactions.Domain.Model.Commands;
using SocialNetwork.API.Interactions.Domain.Model.Entities;
using SocialNetwork.API.Interactions.Domain.Model.Queries;
using SocialNetwork.API.Interactions.Domain.Repositories;
using SocialNetwork.API.Interactions.Domain.Services;
using SocialNetwork.API.Interactions.Interfaces.REST;
using SocialNetwork.API.Interactions.Interfaces.REST.Resources;
using SocialNetwork.API.Interactions.Interfaces.REST.Transform;

namespace TestSocialNetworkAPI;

[TestClass]
public class StatusControllerTests
{
    // Objetos que seran usados en el test
    private Mock<IStatusRepository> _statusRepoMock;
    private Fixture _fixture;
    private StatusController _controller;

    public StatusControllerTests()
    {
        _fixture = new Fixture();
        _statusRepoMock = new Mock<IStatusRepository>();
    }

    // Testeando GetAllStatusByUser
    [TestMethod]
    public async Task Get_Status_ReturnOk()
    {
        // creando mocks y listas de status
        var statusList = _fixture.CreateMany<Status>(3).ToList();
        var user = "testUser";
        _statusRepoMock.Setup(repo => repo.FindAllByUserAsync(user)).ReturnsAsync(statusList);

        var mockStatusCommandService = new Mock<IStatusCommandService>();
        var mockStatusQueryService = new Mock<IStatusQueryService>();
        mockStatusQueryService.Setup(service => service.Handle(It.IsAny<GetAllStatusByUserQuery>())).ReturnsAsync(statusList);

        _controller = new StatusController(mockStatusCommandService.Object, mockStatusQueryService.Object);

        // Metodo GetAllStatusByUser
        
        var result = await _controller.GetAllStatusByUser(user);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var statuses = okResult.Value as IEnumerable<StatusResource>;
        Assert.AreEqual(3, statuses.Count());
    }
    
    
    // Testeando Create  Status
    [TestMethod]
    public async Task Create_Status_ReturnsCreated()
    {
        var createStatusResource = _fixture.Create<CreateStatusResource>();
        var status = _fixture.Create<Status>();
        var createStatusCommand = CreateStatusCommandFromResourceAssembler.ToCommandFromResource(createStatusResource);

        var mockStatusCommandService = new Mock<IStatusCommandService>();
        var mockStatusQueryService = new Mock<IStatusQueryService>();
        mockStatusCommandService.Setup(service => service.Handle(createStatusCommand)).ReturnsAsync(status);

        _controller = new StatusController(mockStatusCommandService.Object, mockStatusQueryService.Object);
        // Metodo CreateStatus
        var result = await _controller.CreateStatus(createStatusResource);
  
        var createdAtActionResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtActionResult);
        Assert.AreEqual(201, createdAtActionResult.StatusCode);

        var returnedStatus = createdAtActionResult.Value as StatusResource;
        Assert.IsNotNull(returnedStatus);
        Assert.AreEqual(status.User, returnedStatus.user);
        Assert.AreEqual(status.Message, returnedStatus.message);
    }
    
    
    
    
}

