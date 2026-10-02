# what is this project?

this project name is additional service,

this project code provides api implementation of basic CURD operation for book and person models uses Sqlite.

# how to run?

dotnet run

# important note

this project code base is only intended to learn C# programming.

```

Case 1 — Exception → Retry → Circuit Breaker → Fallback

                   ┌─────────────────────┐
                   │    GetEntitiesAsync │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │ ResilientCache      │
                   │ Service             │
                   └──────────┬──────────┘
                              │
                   Retry / Circuit / Timeout
                              │
                              ▼
                   ┌─────────────────────┐
                   │ InMemoryCacheService│
                   └──────────┬──────────┘
                              │
                         Exception
                              │
                              ▼
                   ┌─────────────────────┐
                   │       Retry #1      │
                   └──────────┬──────────┘
                              │
                         Exception
                              │
                              ▼
                   ┌─────────────────────┐
                   │       Retry #2      │
                   └──────────┬──────────┘
                              │
                         Exception
                              │
                              ▼
                   ┌─────────────────────┐
                   │       Retry #3      │
                   └──────────┬──────────┘
                              │
                         Exception
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Circuit Breaker     │
                   │ records failure     │
                   └──────────┬──────────┘
                              │
                              ▼
                   Exception propagated
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Fall back to        │
                   │ Database            │
                   └─────────────────────┘

Case 2 — Timeout → Retry → Circuit Breaker → Fallback
Important: With MaxRetryAttempts = 3, there are 4 total attempts: initial attempt + 3 retries.

                   ┌─────────────────────┐
                   │    GetEntitiesAsync │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │ ResilientCache      │
                   │ Service             │
                   └──────────┬──────────┘
                              │
                   Retry / Circuit / Timeout
                              │
                              ▼
                   ┌─────────────────────┐
                   │ InMemoryCacheService│
                   └──────────┬──────────┘
                              │
                       Task.Delay(20s)
                              │
                     Timeout = 15 seconds
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Timeout             │
                   │ cancellation        │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │ TimeoutRejected     │
                   │ Exception           │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │       Retry #1      │
                   └──────────┬──────────┘
                              │
                       Task.Delay(20s)
                              │
                     Timeout = 15 seconds
                              │
                              ▼
                   ┌─────────────────────┐
                   │ TimeoutRejected     │
                   │ Exception           │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │       Retry #2      │
                   └──────────┬──────────┘
                              │
                       Task.Delay(20s)
                              │
                     Timeout = 15 seconds
                              │
                              ▼
                   ┌─────────────────────┐
                   │ TimeoutRejected     │
                   │ Exception           │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │       Retry #3      │
                   └──────────┬──────────┘
                              │
                       Task.Delay(20s)
                              │
                     Timeout = 15 seconds
                              │
                              ▼
                   ┌─────────────────────┐
                   │ TimeoutRejected     │
                   │ Exception           │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Circuit Breaker     │
                   │ records failures    │
                   └──────────┬──────────┘
                              │
                              ▼
                   TimeoutRejectedException
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Fall back to        │
                   │ Database            │
                   └─────────────────────┘


Case 3 — Successful Cache Execution

                   ┌─────────────────────┐
                   │    GetEntitiesAsync │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │ ResilientCache      │
                   │ Service             │
                   └──────────┬──────────┘
                              │
                   Retry / Circuit / Timeout
                              │
                              ▼
                   ┌─────────────────────┐
                   │ InMemoryCacheService│
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Cache lookup        │
                   └──────────┬──────────┘
                              │
                         Cache Hit
                              │
                              ▼
                   ┌─────────────────────┐
                   │ Return cached       │
                   │ BookDTOs            │
                   └──────────┬──────────┘
                              │
                              ▼
                   ┌─────────────────────┐
                   │    GetEntitiesAsync │
                   │      returns        │
                   │    BookDTOs         │
                   └─────────────────────┘

The overall behavior

                         GetEntitiesAsync
                                │
                                ▼
                      ResilientCacheService
                                │
                    Retry / Circuit / Timeout
                                │
                                ▼
                       InMemoryCacheService
                                │
              ┌─────────────────┼─────────────────┐
              │                 │                 │
              ▼                 ▼                 ▼
          Success           Exception          Timeout
              │                 │                 │
              │                 ▼                 ▼
              │              Retry #1          Retry #1
              │                 │                 │
              │                 ▼                 ▼
              │              Retry #2          Retry #2
              │                 │                 │
              │                 ▼                 ▼
              │              Retry #3          Retry #3
              │                 │                 │
              │                 ▼                 ▼
              │          Circuit evaluation  Circuit evaluation
              │                 │                 │
              │                 └────────┬────────┘
              │                          │
              ▼                          ▼
        Return Cache              Fallback to DB

```