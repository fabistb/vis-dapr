# Middleware
Simple example project to demonstrate the dapr middleware functionality.

This example utilizes the rate limit and the uppercase middleware.
In a first step the middleware checks if the amount of requests per seconds has been reached.
If this isn't the case the body will be returned as uppercase.