# Workflow

This project demonstrates Dapr Workflow building block using .NET 10 and Dapr SDK 1.16.1.

## Overview

The example implements an order processing workflow that showcases the key features of Dapr Workflows:

- **Order Validation** - Validates order data
- **Stock Check** - Checks product availability
- **Price Calculation** - Calculates order total with taxes, shipping, and discounts
- **Send Offer** - Sends offer to customer
- **Process Response** - Handles customer acceptance or rejection

## Features

### Workflows
- **OrderProcessingWorkflow** - Main workflow orchestrating the entire order process
- **PricingWorkflow** - Child workflow for price calculation

### Activities
- `ValidateOrderActivity` - Validates order data
- `CheckStockActivity` - Checks inventory stock levels
- `CalculatePriceActivity` - Calculates prices, taxes, shipping, and discounts
- `SendOfferActivity` - Sends offer notification to customer
- `ProcessAcceptedOfferActivity` - Processes accepted offers
- `ProcessDeclinedOfferActivity` - Processes declined offers

### Workflow Features Demonstrated
- Activity execution
- Child workflows
- External events (waiting for customer response)
- Workflow state management
- Error handling

## Prerequisites

- .NET 10 SDK
- Dapr CLI
- Redis (installed with `dapr init`)

## Running the Application

### With Dapr CLI

**Note:** DaprSidekick is not used in this project as it currently doesn't support the Workflows API.

Start the application with Dapr:

```bash
cd Workflow
dapr run --app-id workflow --components-path ./components --config ./configuration.yaml --dapr-http-port 3500 --dapr-grpc-port 53570 --app-port 5080 -- dotnet run
```

### Testing the Workflow

1. **Create an Order**

```bash
curl -X POST http://localhost:5080/api/order \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD-001",
    "customerId": "CUST-123",
    "deliveryAddress": "123 Main St, City, Country",
    "items": [
      {
        "productId": "PROD-001",
        "productName": "Laptop",
        "quantity": 2,
        "unitPrice": 999.99
      }
    ]
  }'
```

This will return a workflow instance ID.

2. **Respond to Offer** (within 5 minutes)

```bash
# Accept offer
curl -X POST http://localhost:5080/api/order/ORD-001/respond \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD-001",
    "isAccepted": true,
    "reason": ""
  }'

# Or decline offer
curl -X POST http://localhost:5080/api/order/ORD-002/respond \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD-002",
    "isAccepted": false,
    "reason": "Price too high"
  }'
```

3. **Check Workflow Status**

```bash
curl http://localhost:5080/api/order/{instanceId}/status
```

## Components

### Redis Actor State Store
The workflow uses Redis as the actor state store for maintaining workflow state. The component is configured in `components/actorstatestore.yaml`.

## Project Structure

```
Workflow/
├── Activities/           # Workflow activities
├── Workflows/           # Workflow definitions
├── Controllers/         # API controllers
├── Models/             # Data models
├── components/         # Dapr component configurations
├── requests/           # Sample HTTP requests
└── Program.cs          # Application entry point
```

## Testing

The solution includes two test projects:

- **Workflow.UnitTests** - Unit tests for activities and workflow logic
- **Workflow.IntegrationTests** - Integration tests using Testcontainers

Run tests:
```bash
dotnet test
```

## Documentation

For more information about Dapr Workflows, see:
- [Dapr Workflow Overview](https://docs.dapr.io/developing-applications/building-blocks/workflow/workflow-overview/)
- [Dapr Workflow .NET SDK](https://docs.dapr.io/developing-applications/sdks/dotnet/dotnet-workflow/)
