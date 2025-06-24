using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcMicroservices.Protos;

namespace GrpcMicroservices.Services
{
    public class OrderService : OrderProtoService.OrderProtoServiceBase
    {
        private static readonly List<Order> _orders = new();
        private static int orderCounter = 1;

        public override Task<Order> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.ProductId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Product ID is required."));

            if (request.Quantity <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Quantity must be greater than 0."));

            // Fake product price logic (in real-world, fetch from ProductService/database)
            double fakeProductPrice = 99.99;

            var newOrder = new Order
            {
                Id = orderCounter.ToString(),
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TotalPrice = fakeProductPrice * request.Quantity
            };

            _orders.Add(newOrder);
            orderCounter++;

            return Task.FromResult(newOrder);
        }

        public override Task<Order> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            var order = _orders.FirstOrDefault(o => o.Id == request.Id);
            if (order == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Order with ID '{request.Id}' not found."));

            return Task.FromResult(order);
        }

        public override Task<OrderListResponse> GetAllOrders(Empty request, ServerCallContext context)
        {
            var response = new OrderListResponse();
            response.Orders.AddRange(_orders);
            return Task.FromResult(response);
        }
    }
}