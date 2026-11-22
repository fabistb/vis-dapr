# Middleware
Simple example project to demonstrate the dapr middleware functionality.

This example utilizes the rate limit and the uppercase middleware.
In a first step the middleware checks if the amount of requests per seconds has been reached.
If this isn't the case the body will be returned as uppercase.

## Run
Requires the Dapr CLI and `dapr init`.
```sh
cd Middleware
dapr run -f .
```
Stop with `dapr stop -f .`.

`dapr.yaml` starts the sidecar and the app (`dotnet run` with the `Middleware` launch profile).
Starting the app alone (IDE or `dotnet run`) starts no sidecar, so the Dapr middleware pipeline is not applied.
To debug from the IDE, start via `dapr run -f .` and attach the debugger to the `Middleware` process.
