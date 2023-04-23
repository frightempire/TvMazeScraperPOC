# TvMazeScraperPOC

This is a test project for RTL. Solution consists of a web API and a background service that synchronizes data with an external TvMaze API. The solution uses SQLite database to store the data.

# Technologies

* .NET 7
* EF Core 7 + SQLite
* Polly
* AutoMapper 
* Serilog
* Docker + Docker Compose
    
# How to run

```
docker compose up
```

then navigate to http://localhost:8000/swagger

# Potential for Expansion

* Incorporate a messaging queue in order to support increased scalability and reliability as the solution grows. This could involve introducing a message broker such as RabbitMQ or Apache Kafka to handle communication between services.
* Introduce additional layers of caching to improve performance and reduce the load on the database. This could include using a distributed cache such as Redis or Memcached to cache frequently accessed data.
* Incorporate more sophisticated monitoring and alerting, using tools such as Prometheus or Grafana to provide visibility into the health and performance of the various components of the system.
* Increase Unit test coverage :flushed:
