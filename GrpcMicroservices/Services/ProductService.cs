using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcMicroservices.Protos;

namespace GrpcMicroservices.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private static readonly List<Product> _products = new();

        public override Task<Product> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Product ID is required."));

            var product = _products.FirstOrDefault(p => p.Id == request.Id);
            if (product == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID '{request.Id}' not found."));

            return Task.FromResult(product);
        }

        public override Task<ProductListResponse> GetAllProducts(Empty request, ServerCallContext context)
        {
            var response = new ProductListResponse();
            response.Products.AddRange(_products);

            return Task.FromResult(response);
        }

        public override Task<Product> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var newProduct = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Quantity = request.Quantity
            };

            _products.Add(newProduct);
            return Task.FromResult(newProduct);
        }

        public override Task<Product> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var product = _products.FirstOrDefault(p => p.Id == request.Id);
            if (product == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID '{request.Id}' not found."));

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Quantity = request.Quantity;

            return Task.FromResult(product);
        }

        public override Task<Empty> DeleteProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = _products.FirstOrDefault(p => p.Id == request.Id);
            if (product == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID '{request.Id}' not found."));

            _products.Remove(product);
            return Task.FromResult(new Empty());
        }
    }
}
