using Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderCompleted;

public class OrderCompletedDomainEventHandler
                : INotificationHandler<OrderCompletedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILoggerFactory _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderCompletedDomainEventHandler(
        IOrderRepository orderRepository,
        ILoggerFactory logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderCompletedDomainEvent completedOrderDomainEvent, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderCompletedDomainEvent>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                completedOrderDomainEvent.Order.Id, nameof(OrderStatus.Completed), OrderStatus.Completed.Id);

        var order = await _orderRepository.GetAsync(completedOrderDomainEvent.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        var orderStatusChangedToCompletedIntegrationEvent = new OrderStatusChangedToCompletedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToCompletedIntegrationEvent);
    }
}
