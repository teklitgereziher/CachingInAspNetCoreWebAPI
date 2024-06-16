# Caching In ASP.NET Core Web API
Caching is a way of storing most frequently accessed and less frequently modified data in a temporary 
storage called cache so that subsequent requests will access the data from the cache. Caching can 
provide the following advantages:
- It can improve perfomance by avoiding request to the external source
- It can reduce costs when cost is calculated per request to the original data source  
- It can reduce the load to the external server that receives requests by minimizing the number of requests

ASP.NET Core offers out of the box support for different types of caching:
1. In-Memory Caching - Data is cached within the server’s memory.
2. Distributed Caching - Data is stored external to the application in sources such as Redis cache.
3. Hybrid Caching - A new type of caching introduced in .NET 9 to bridge the gap between the distributed cache and the in-memory cache.

## In-Memory Caching in ASP.NET Core


## Distributed Caching in ASP.NET Core


## Hybrid Caching in ASP.NET Core