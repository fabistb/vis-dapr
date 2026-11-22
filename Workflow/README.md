# Workflow
## Order processing
Dapr Workflows let you write durable, long-running processes as code.
This sample runs a simple order process. It shows:
- activity chaining
- fan-out / fan-in
- child workflows
- external events with a timeout
- retry policies
- compensation

All activities run in-memory inside the app (no external services); only workflow state is stored in Redis.
Inventory is in-memory and resets on restart; reservations are idempotent per order.

[Dapr Docs](https://docs.dapr.io/developing-applications/building-blocks/workflow/workflow-overview/)

## Run
Requires the Dapr CLI and `dapr init` (Redis, placement and scheduler containers).
```sh
cd Workflow
dapr run -f .
```
Stop with `dapr stop -f .`.

`dapr.yaml` starts the sidecar and the app (`dotnet run`).
Starting the app alone (IDE or `dotnet run`) starts no sidecar, so workflows fail.

## Endpoints
See `Workflow/requests/workflow.http`.
- `POST /api/v1/orders` starts an order workflow
- `GET /api/v1/orders/{id}` returns the workflow status
- `POST /api/v1/orders/{id}/response` accepts or declines the offer (raises an external event); the state check is best-effort, so 202 means submitted, poll GET for the outcome

Order id is the workflow instance id. Seeded products: `widget`, `gadget`, `gizmo` (stock 5), `sold-out`.
Payment fails for customer `blocked` or totals above 10000. Orders of 100 or more get 10% off.
`offerTimeout` (e.g. `"00:00:10"`) is optional, default 5 minutes.

## Tests
```sh
cd Workflow
dotnet test --solution Workflow.sln
```
Integration tests use Testcontainers, so Docker must be running.
