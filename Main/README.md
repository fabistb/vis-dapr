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

# Binding
## Input binding
Dapr utilizes an _options_ request to determine if an application subscribes to an input binding.
If the application returns 2xx or 405. While both variants are valid for operations it might be preferable to return 2xx to reduce the amount of failed requests in the logs.
[Dapr Docs](https://docs.dapr.io/developing-applications/building-blocks/bindings/bindings-overview/#input-bindings)
