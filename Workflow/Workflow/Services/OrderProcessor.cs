using Microsoft.Extensions.Logging;
using Workflow.Activities;
using Workflow.Models;

namespace Workflow.Workflows;

public class OrderProcessor
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public OrderProcessor(ILogger logger, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<OrderProcessingResult> ProcessOrderAsync(OrderRequest request)
    {
        _logger.LogInformation("Starting order processing workflow for order {OrderId}", request.OrderId);

        // Step 1: Validate order
        var validateActivity = new ValidateOrderActivity(_loggerFactory.CreateLogger<ValidateOrderActivity>());
        var validationResult = await validateActivity.RunAsync(null!, request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Order {OrderId} validation failed: {Errors}",
                request.OrderId, string.Join(", ", validationResult.ValidationErrors));

            return new OrderProcessingResult
            {
                OrderId = request.OrderId,
                Status = "ValidationFailed",
                Message = $"Validation errors: {string.Join(", ", validationResult.ValidationErrors)}",
                ProcessedAt = DateTime.UtcNow
            };
        }

        _logger.LogInformation("Order {OrderId} validated successfully", request.OrderId);

        // Step 2: Check stock
        var checkStockActivity = new CheckStockActivity(_loggerFactory.CreateLogger<CheckStockActivity>());
        var stockResult = await checkStockActivity.RunAsync(null!, request);

        if (!stockResult.IsInStock)
        {
            _logger.LogWarning("Order {OrderId} cannot be fulfilled - out of stock items: {Items}",
                request.OrderId, string.Join(", ", stockResult.OutOfStockItems));

            return new OrderProcessingResult
            {
                OrderId = request.OrderId,
                Status = "OutOfStock",
                Message = $"Out of stock items: {string.Join(", ", stockResult.OutOfStockItems)}",
                ProcessedAt = DateTime.UtcNow
            };
        }

        _logger.LogInformation("Order {OrderId} stock check passed", request.OrderId);

        // Step 3: Calculate price using child workflow/activity
        var calculatePriceActivity = new CalculatePriceActivity(_loggerFactory.CreateLogger<CalculatePriceActivity>());
        var priceCalculation = await calculatePriceActivity.RunAsync(null!, request);

        _logger.LogInformation("Order {OrderId} price calculated: {TotalPrice:C}", 
            request.OrderId, priceCalculation.TotalPrice);

        // Step 4: Send offer
        var sendOfferActivity = new SendOfferActivity(_loggerFactory.CreateLogger<SendOfferActivity>());
        var offerMessage = await sendOfferActivity.RunAsync(null!, (request, priceCalculation));

        _logger.LogInformation("Offer sent for order {OrderId}: {Message}", 
            request.OrderId, offerMessage);

        // Return success with pending confirmation
        return new OrderProcessingResult
        {
            OrderId = request.OrderId,
            Status = "OfferSent",
            PriceCalculation = priceCalculation,
            Message = "Offer has been sent to customer. Awaiting confirmation.",
            ProcessedAt = DateTime.UtcNow
        };
    }
}
