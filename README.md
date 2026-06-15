# Performance Testing and Caching Optimization

## Load Testing with k6

To evaluate the API's behavior under concurrent user load, k6 was integrated into the project.

### Test Script

A load test script (`load-test/users-test.js`) was created:

```javascript
import http from "k6/http";

export const options = {
  vus: 100,
  iterations: 100,
};

export default function () {
  http.get("https://localhost:7265/api/users");
}
```

### Running the Test

```bash
k6 run load-test/users-test.js
```

The following load levels were tested:

* 100 Concurrent Users
* 200 Concurrent Users
* 300 Concurrent Users

The objective was to measure:

* Response Time
* Throughput
* Failure Rate
* Application Scalability

---

## Performance Bottleneck Investigation

Load testing revealed that the `GET /api/users` endpoint executed a database query on every request.

Request Flow:

```text
Client
  ↓
Controller
  ↓
MediatR Query
  ↓
Repository
  ↓
SQL Server
```

Under higher concurrency, repeated database access increased response times and caused request failures.

---

## Caching Optimization

To reduce database load, in-memory caching was implemented using `IMemoryCache`.

### Service Registration

```csharp
builder.Services.AddMemoryCache();
```

### Query Handler Caching

```csharp
public async Task<List<UserResponseDto>> Handle(
    GetUsersQuery request,
    CancellationToken cancellationToken)
{
    if (_cache.TryGetValue("users", out List<UserResponseDto>? cachedUsers))
    {
        return cachedUsers!;
    }

    var users = await _userRepository.GetAllAsync();

    var result = users.Select(user => new UserResponseDto
    {
        Id = user.Id,
        Email = user.Email,
        Role = user.Role
    }).ToList();

    _cache.Set(
        "users",
        result,
        TimeSpan.FromMinutes(5));

    return result;
}
```

### Cache Invalidation

The cache is cleared whenever user data changes:

```csharp
_cache.Remove("users");
```

Implemented after:

* Create User
* Update User
* Delete User

---

## Updated Request Flow

```text
Client
  ↓
Controller
  ↓
MediatR Query
  ↓
Memory Cache
  ↓
Return Response
```

Database access occurs only on a cache miss.

---

## Performance Results

### 100 Concurrent Users

| Metric            | Without Cache | With Cache |
| ----------------- | ------------- | ---------- |
| Avg Response Time | 2.97s         | 1.30s      |
| P95 Response Time | 3.92s         | 1.94s      |
| Failures          | 0%            | 0%         |

### 200 Concurrent Users

| Metric            | Without Cache | With Cache |
| ----------------- | ------------- | ---------- |
| Avg Response Time | 2.43s         | 2.13s      |
| P95 Response Time | 3.05s         | 2.86s      |
| Failures          | 0%            | 0%         |

### 300 Concurrent Users

| Metric            | Without Cache | With Cache |
| ----------------- | ------------- | ---------- |
| Avg Response Time | 1.83s         | 1.87s      |
| P95 Response Time | 3.30s         | 3.31s      |
| Failures          | 33.00%        | 18.66%     |

---

## Key Findings

* Implemented load testing using k6.
* Identified database access as a performance bottleneck.
* Added in-memory caching using `IMemoryCache`.
* Reduced average response time by approximately 56% for 100 concurrent users.
* Reduced failed requests at 300 concurrent users from 99 failures to 56 failures.
* Improved API scalability and reduced database pressure.

This exercise demonstrates practical performance testing, bottleneck analysis, and optimization techniques in ASP.NET Core using MediatR, CQRS, EF Core, and IMemoryCache.
