namespace UnitTest.Ordering.Application;

using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;

public class OrderCompleteCommandHandlerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IOrderQueries> _orderQueriesMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<ILogger<OrdersController>> _loggerMock;

    public OrderCompleteCommandHandlerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _orderQueriesMock = new Mock<IOrderQueries>();
        _identityServiceMock = new Mock<IIdentityService>();
        _loggerMock = new Mock<ILogger<OrdersController>>();
    }

    [Fact]
    public async Task Complete_order_with_requestId_success()
    {
        //Arrange
        _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CompleteOrderCommand, bool>>(), default(CancellationToken)))
            .Returns(Task.FromResult(true));

        //Act
        var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.CompleteOrderAsync(new CompleteOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

        //Assert
        Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);

    }

    [Fact]
    public async Task Complete_order_bad_request()
    {
        //Arrange
        _mediatorMock.Setup(x => x.Send(It.IsAny<IdentifiedCommand<CompleteOrderCommand, bool>>(), default(CancellationToken)))
            .Returns(Task.FromResult(true));

        //Act
        var orderController = new OrdersController(_mediatorMock.Object, _orderQueriesMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        var actionResult = await orderController.CompleteOrderAsync(new CompleteOrderCommand(1), String.Empty) as BadRequestResult;

        //Assert
        Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
    }


}

