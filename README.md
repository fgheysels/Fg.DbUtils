# Fg.DbUtils

This project introduces a `DbSession` class which manages the connection to a (relational) database and the related transaction (if any).

## Usage

I'm using the `DbSession` class when using Dapper to communicate with a relational database.  When starting a transaction on the `DbSession`, you do not need to pass in the `IDbTransaction` object with each database request.
Since `IDbSession` implements the `IDbConnection` interface, integration with Dapper is fairly easy.

### Executing transactional DB queries

```csharp
DbSession session = new DbSession(_connection);

await session.WithTransactionAsycn( async () => 
{
    // Note that we do not need to pass in the transaction parameter.
    await session.ExecuteAsync("INSERT INTO Persons (FirstName, LastName) VALUES (@FirstName, @LastName)",
                               param: new { FirstName = "Frederik", LastName = "Gheysels" });
                               
    if( new Random().NextValue(3) % 2 != 0 )
    {
        // Throwing an exception will make sure that the transaction is rollbacked.
        throw new InvalidOperationException();
    }
                               
    // When no exception occurs, the transaction is committed
});
```
