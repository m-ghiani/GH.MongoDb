# Generic MongoDb Repository .Net Core 3.0

## Synopsis

Is an repository pattern implementation for mongodb.
It contains abstract classes for __CRUD__ operations.

## Code Example

You must create classes derived from IDocument and the repositories derived from GenericRepositoryAsync or GenericBucketRepositoryAsync or GenericGeoRepositoryAsync:

```cs
public class Person: IDocument<ObjectId>
    {
	    public ObjectId Id {get; set;}
	    public string FirstName {get; set;}
	    public string LastName {get; set;}
    }

public sealed class PeopleRepositoryAsync:GenericRepositoryAsync<Person, ObjectId>
{
    public PeopleRepositoryAsync(IMongoDbConnector connector) : base(connector, "people")
    {
    }
}
```
"people" is collection name.
then you can use repository:

```cs
var myConnector = new MongoDbConnector("localhost", "mydatabase")
var myRepo = new PeopleRepositoryAsync(myConnector)

myRepo.Get() //return all people in my collection 
myRepo.Get(myID) //return person with myID as id
myRepo.Get(p=> p.FirstName == "John") //return all people who have name john
```

and so on..

## Installation

by NUGET

Install-Package GH.MongoDb.GenericRepository -Version 2.0.7
