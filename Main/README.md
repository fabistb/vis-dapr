# Main

# State Store
## Query state store
Because of simplicity reasons and to ensure that this example application runs on as many clients as possible a mongo db ist used for the state store query example.

**Install Mongo Db Docker** 
```
$ docker run -d --rm -p 27017:27017 --name mongodb mongo:5
```
### Explanation
1. The redis installed with ```dapr init``` doesn't contain the redisjson module
2. ```edislabs/rejson``` only support an amd64 architecture
3. Mongo Db doesn't have the architecture limitations and doesn't require to set the query indexes manually

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
# Binding
## Input binding
Dapr utilizes an _options_ request to determine if an application subscribes to an input binding.
If the application returns 2xx or 405. While both variants are valid for operations it might be preferable to return 2xx to reduce the amount of failed requests in the logs.
[Dapr Docs](https://docs.dapr.io/developing-applications/building-blocks/bindings/bindings-overview/#input-bindings)


# Outbox Pattern
Example implemenation of the Dapr outbox pattern.

**Note:**
At least for the .Net SDK this appears to be slightly inconsistent.
When the CloudEvent middleware is activated it always returns 415 - Unsupported Media Type.

Also, the message pushed to the messagebus looks different depending on if the state change was triggered by the dapr api or the dotnet sdk.

**Dapr API**
```json
{
    "data": "{\"data\":\"map[age:35 firstName:Max key:person1 lastName:Mustermann]\",\"datacontenttype\":\"text/plain\",\"id\":\"bd2deacf-c9ef-496a-b613-edd509eb4a7e\",\"pubsubname\":\"messagebus\",\"source\":\"main\",\"specversion\":\"1.0\",\"time\":\"2024-07-22T21:05:02+02:00\",\"topic\":\"outbox-topic\",\"traceid\":\"00-9acbe9a5baa4127251ed30db1dae9d59-28539b42a5f2f32a-01\",\"traceparent\":\"00-9acbe9a5baa4127251ed30db1dae9d59-28539b42a5f2f32a-01\",\"tracestate\":\"\",\"type\":\"com.dapr.event.sent\"}"
}
```

**.Net SDK**
```json
{
    "data": "{\"data\":\"{\\\"Key\\\":\\\"person1\\\",\\\"FirstName\\\":\\\"Max\\\",\\\"LastName\\\":\\\"Mustermann\\\",\\\"Age\\\":40}\",\"datacontenttype\":\"text/plain\",\"id\":\"aa21e718-d94e-47ca-8906-45df7dde620b\",\"pubsubname\":\"messagebus\",\"source\":\"main\",\"specversion\":\"1.0\",\"time\":\"2024-07-22T21:09:38+02:00\",\"topic\":\"outbox-topic\",\"traceid\":\"00-a13d4c515ef09fd1e409ffdb54f77f5d-d2ff4e5e6fa508a7-01\",\"traceparent\":\"00-a13d4c515ef09fd1e409ffdb54f77f5d-d2ff4e5e6fa508a7-01\",\"tracestate\":\"\",\"type\":\"com.dapr.event.sent\"}"
}
```

[DaprDocs](https://docs.dapr.io/developing-applications/building-blocks/state-management/howto-outbox/)
