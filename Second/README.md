# Second
# State Store
## Shared State
Dapr provides the option to share state between different applications. By default dapr only the application which writes the state can also access it.
However certain architectures might request sharing the state between different applications.
Dapr does this by setting the _keyPrefix_ value in the component. <br />

Currently available _keyPrefix_ values:
- appId (default)
- name
- namespace
- none

[Dapr Docs](https://docs.dapr.io/developing-applications/building-blocks/state-management/howto-share-state/)

In this example the application **main** sets the state while the application **second** reads the state.


## Run
Requires the Dapr CLI and `dapr init`.
```sh
cd Second
dapr run -f .
```
Stop with `dapr stop -f .`.

`dapr.yaml` starts the sidecar and the app (`dotnet run` with the `Second` launch profile).
Starting the app alone (IDE or `dotnet run`) starts no sidecar, so Dapr calls and actors fail.
To debug from the IDE, start via `dapr run -f .` and attach the debugger to the `Second` process.
