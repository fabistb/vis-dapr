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

