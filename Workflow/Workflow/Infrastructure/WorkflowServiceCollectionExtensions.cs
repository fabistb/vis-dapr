using Dapr.Workflow;
using Workflow.Activities;
using Workflow.Services;
using Workflow.Workflows;

namespace Workflow.Infrastructure;

public static class WorkflowServiceCollectionExtensions
{
    public static IServiceCollection AddOrderWorkflows(this IServiceCollection services)
    {
        services.AddSingleton<IInMemoryInventory, InMemoryInventory>();
        services.AddDaprWorkflow(options =>
        {
            options.RegisterWorkflow<OrderProcessingWorkflow>();
            options.RegisterWorkflow<InventoryCheckWorkflow>();
            options.RegisterWorkflow<FulfillmentWorkflow>();

            options.RegisterActivity<ValidateOrderActivity>();
            options.RegisterActivity<CheckStockActivity>();
            options.RegisterActivity<CalculatePriceActivity>();
            options.RegisterActivity<SendOfferActivity>();
            options.RegisterActivity<NotifyDeclinedActivity>();
            options.RegisterActivity<ReserveStockActivity>();
            options.RegisterActivity<ProcessPaymentActivity>();
            options.RegisterActivity<ReleaseStockActivity>();
            options.RegisterActivity<ShipOrderActivity>();
        });
        // AddDaprWorkflow only registers the concrete client; expose the interface so consumers can be unit tested.
        services.AddScoped<IDaprWorkflowClient>(sp => sp.GetRequiredService<DaprWorkflowClient>());
        return services;
    }
}
