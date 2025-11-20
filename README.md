# ASP.NET Core Unit Testing with xUnit - Complete Guide

## Table of Contents
1. [Introduction](#introduction)
2. [xUnit Framework](#xunit-framework)
3. [AAA Pattern](#aaa-pattern)
4. [Testing CRUD Operations](#testing-crud-operations)
5. [Mocking Dependencies](#mocking-dependencies)
6. [Best Practices](#best-practices)
7. [Common Patterns](#common-patterns)
8. [Interview Preparation](#interview-preparation)

---

## Introduction

Unit testing is a software development practice where you write code to test individual units (classes or methods) in isolation from the rest of the application.

### Why Unit Test?

| Benefit | Description |
|---------|-------------|
| **Early Bug Detection** | Identify and fix issues early in the development cycle |
| **Refactoring Confidence** | Make changes knowing tests will alert you if something breaks |
| **Improved Design** | Encourages modular, loosely coupled code |
| **Living Documentation** | Tests illustrate how code should be used |
| **Regression Prevention** | Prevents old bugs from reappearing |
| **Faster Development** | Less time debugging, more time building features |

### What Makes a Good Unit Test?

✅ **Fast:** Runs in milliseconds  
✅ **Isolated:** Tests one unit without dependencies  
✅ **Repeatable:** Produces same results every time  
✅ **Self-Validating:** Clear pass/fail result  
✅ **Timely:** Written before or with production code  

---

## xUnit Framework

xUnit is a popular open-source unit testing framework for .NET, known for its modern approach and extensibility.

### Why Choose xUnit?

| Feature | Benefit |
|---------|---------|
| **Extensibility** | Create custom attributes and assertions |
| **Community** | Large, active community with extensive resources |
| **Integration** | Works seamlessly with Visual Studio, Rider, VS Code |
| **Performance** | Fast test execution |
| **Modern Design** | Built with modern .NET practices |
| **Parallel Execution** | Tests run in parallel by default |

### Setting Up xUnit

#### 1. Create a Test Project

```bash
# Create a new xUnit test project
dotnet new xunit -n YourProject.Tests

# Add reference to the project you want to test
cd YourProject.Tests
dotnet add reference ../YourProject/YourProject.csproj

# Add necessary packages
dotnet add package Moq              # For mocking
dotnet add package FluentAssertions # For better assertions (optional)
```

#### 2. Project Structure

```
Solution/
├── YourProject/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   └── YourProject.csproj
└── YourProject.Tests/
    ├── Controllers/
    ├── Services/
    ├── YourProject.Tests.csproj
    └── usings.cs
```

### Core xUnit Attributes

#### [Fact]
Marks a method as a simple test case that should always pass.

```csharp
[Fact]
public void Add_TwoPositiveNumbers_ReturnsCorrectSum()
{
    // Arrange
    var calculator = new Calculator();
    
    // Act
    var result = calculator.Add(2, 3);
    
    // Assert
    Assert.Equal(5, result);
}
```

#### [Theory] with [InlineData]
Marks a method for data-driven testing with multiple inputs.

```csharp
[Theory]
[InlineData(2, 3, 5)]
[InlineData(0, 0, 0)]
[InlineData(-1, 1, 0)]
[InlineData(100, 200, 300)]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    // Arrange
    var calculator = new Calculator();
    
    // Act
    var result = calculator.Add(a, b);
    
    // Assert
    Assert.Equal(expected, result);
}
```

#### [ClassFixture]
Shares a fixture instance across all tests in a class (for expensive setup).

```csharp
public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }
    
    public DatabaseFixture()
    {
        // Expensive setup - runs once per test class
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        Context = new ApplicationDbContext(options);
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}

public class PersonServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly ApplicationDbContext _context;
    
    public PersonServiceTests(DatabaseFixture fixture)
    {
        _context = fixture.Context;
    }
    
    [Fact]
    public void Test_UsesSharedContext()
    {
        // Test implementation
    }
}
```

#### [Collection]
Groups test classes to share context and run sequentially.

```csharp
[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created.
    // Its purpose is to be the place to apply [CollectionDefinition]
}

[Collection("Database collection")]
public class Test1
{
    DatabaseFixture _fixture;
    
    public Test1(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
}

[Collection("Database collection")]
public class Test2
{
    DatabaseFixture _fixture;
    
    public Test2(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
}
```

### xUnit Assertions

#### Equality Assertions

```csharp
// Basic equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Strict equality (no type conversion)
Assert.StrictEqual(expected, actual);

// Collection equality (order matters)
Assert.Equal(expectedList, actualList);

// Collection equality (order doesn't matter)
Assert.Equal(expectedList.OrderBy(x => x), actualList.OrderBy(x => x));
```

#### Boolean Assertions

```csharp
Assert.True(condition);
Assert.False(condition);
```

#### Null Assertions

```csharp
Assert.Null(obj);
Assert.NotNull(obj);
```

#### Type Assertions

```csharp
Assert.IsType<ExpectedType>(obj);
Assert.IsNotType<UnexpectedType>(obj);
Assert.IsAssignableFrom<BaseType>(obj);
```

#### Exception Assertions

```csharp
// Assert that code throws specific exception
Assert.Throws<ArgumentNullException>(() => 
{
    service.DoSomething(null);
});

// Assert and capture exception for further inspection
var exception = Assert.Throws<ArgumentException>(() => 
{
    service.DoSomething("invalid");
});
Assert.Equal("Parameter cannot be invalid", exception.Message);

// Assert that code doesn't throw
var exception = Record.Exception(() => service.DoSomething("valid"));
Assert.Null(exception);
```

#### Collection Assertions

```csharp
// Contains/DoesNotContain
Assert.Contains(item, collection);
Assert.DoesNotContain(item, collection);

// Contains with predicate
Assert.Contains(collection, item => item.Name == "John");

// Empty/NotEmpty
Assert.Empty(collection);
Assert.NotEmpty(collection);

// Single item
Assert.Single(collection);
Assert.Single(collection, item => item.Id == 1);

// Collection size
Assert.Equal(expectedCount, collection.Count);

// All items match condition
Assert.All(collection, item => Assert.True(item.IsValid));
```

#### String Assertions

```csharp
Assert.StartsWith("Hello", text);
Assert.EndsWith("World", text);
Assert.Contains("middle", text);
Assert.DoesNotContain("xyz", text);
Assert.Matches(@"^\d{3}-\d{3}-\d{4}$", phoneNumber); // Regex
```

#### Numeric Assertions

```csharp
Assert.Equal(expected, actual, precision: 2); // For doubles/decimals
Assert.InRange(value, low, high);
Assert.NotInRange(value, low, high);
```

---

## AAA Pattern

The **Arrange-Act-Assert (AAA)** pattern is the gold standard for structuring unit tests.

### Pattern Breakdown

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // ========== ARRANGE ==========
    // Set up test data, create objects, configure mocks
    var dependency = new Mock<IDependency>();
    dependency.Setup(d => d.GetValue()).Returns(10);
    
    var service = new MyService(dependency.Object);
    var input = new Input { Value = 5 };
    
    // ========== ACT ==========
    // Execute the method under test
    var result = service.ProcessInput(input);
    
    // ========== ASSERT ==========
    // Verify the outcome
    Assert.NotNull(result);
    Assert.Equal(15, result.Value);
    dependency.Verify(d => d.GetValue(), Times.Once);
}
```

### Arrange Phase

**Purpose:** Set up the test's preconditions.

**What to do:**
- Create instances of the class under test
- Initialize input data
- Configure mock objects
- Set up any required state

```csharp
// Arrange
var mockRepository = new Mock<IPersonRepository>();
var mockLogger = new Mock<ILogger<PersonService>>();

mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
    .ReturnsAsync(new Person { Id = Guid.NewGuid(), Name = "John" });

var service = new PersonService(mockRepository.Object, mockLogger.Object);
var personId = Guid.NewGuid();
```

### Act Phase

**Purpose:** Execute the code under test.

**What to do:**
- Call the method you're testing
- Capture the result

```csharp
// Act
var result = await service.GetPersonByIdAsync(personId);
```

**Best Practices:**
- Keep this section minimal (ideally one line)
- If you need complex setup, it belongs in Arrange
- If you need multiple assertions, consider splitting into separate tests

### Assert Phase

**Purpose:** Verify the outcome matches expectations.

**What to do:**
- Check return values
- Verify object state
- Confirm mock interactions

```csharp
// Assert
Assert.NotNull(result);
Assert.Equal("John", result.Name);
mockRepository.Verify(r => r.GetByIdAsync(personId), Times.Once);
mockLogger.Verify(
    l => l.Log(
        LogLevel.Information,
        It.IsAny<EventId>(),
        It.IsAny<It.IsAnyType>(),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
    Times.Once);
```

### Complete AAA Example

```csharp
public class CalculatorTests
{
    [Fact]
    public void Divide_PositiveNumbers_ReturnsCorrectQuotient()
    {
        // Arrange
        var calculator = new Calculator();
        var dividend = 10;
        var divisor = 2;
        var expectedQuotient = 5;
        
        // Act
        var actualQuotient = calculator.Divide(dividend, divisor);
        
        // Assert
        Assert.Equal(expectedQuotient, actualQuotient);
    }
    
    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        var calculator = new Calculator();
        var dividend = 10;
        var divisor = 0;
        
        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => 
        {
            calculator.Divide(dividend, divisor);
        });
    }
}
```

---

## Testing CRUD Operations

CRUD operations (Create, Read, Update, Delete) are fundamental to most applications. Here's how to test them effectively.

### Service Architecture

```csharp
// Entity
public class Person
{
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}

// DTO - Request
public class PersonAddRequest
{
    public string PersonName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}

// DTO - Response
public class PersonResponse
{
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}

// Service Interface
public interface IPersonsService
{
    PersonResponse AddPerson(PersonAddRequest request);
    List<PersonResponse> GetAllPersons();
    PersonResponse? GetPersonByPersonId(Guid personId);
    PersonResponse UpdatePerson(PersonUpdateRequest request);
    bool DeletePerson(Guid personId);
}
```

### Testing CREATE Operations

```csharp
public class PersonsServiceTest
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    
    public PersonsServiceTest()
    {
        _countriesService = new CountriesService();
        _personsService = new PersonsService(_countriesService);
    }
    
    #region AddPerson Tests
    
    [Fact]
    public void AddPerson_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        PersonAddRequest? request = null;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
        {
            _personsService.AddPerson(request);
        });
    }
    
    [Fact]
    public void AddPerson_NullPersonName_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest request = new PersonAddRequest 
        { 
            PersonName = null 
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
        {
            _personsService.AddPerson(request);
        });
    }
    
    [Fact]
    public void AddPerson_InvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest request = new PersonAddRequest 
        { 
            PersonName = "John",
            Email = "invalid-email" 
        };
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
        {
            _personsService.AddPerson(request);
        });
        
        Assert.Contains("email", exception.Message.ToLower());
    }
    
    [Fact]
    public void AddPerson_ValidDetails_ReturnsPersonResponseWithId()
    {
        // Arrange
        CountryAddRequest countryRequest = new CountryAddRequest 
        { 
            CountryName = "USA" 
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryRequest);
        
        PersonAddRequest personRequest = new PersonAddRequest
        {
            PersonName = "John Doe",
            Email = "john@example.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            CountryId = countryResponse.CountryId,
            Address = "123 Main St",
            ReceiveNewsLetters = true
        };
        
        // Act
        PersonResponse response = _personsService.AddPerson(personRequest);
        List<PersonResponse> allPersons = _personsService.GetAllPersons();
        
        // Assert
        Assert.NotEqual(Guid.Empty, response.PersonId);
        Assert.Equal("John Doe", response.PersonName);
        Assert.Equal("john@example.com", response.Email);
        Assert.Contains(response, allPersons);
    }
    
    [Fact]
    public void AddPerson_DuplicateEmail_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest request1 = new PersonAddRequest
        {
            PersonName = "John",
            Email = "john@example.com"
        };
        
        PersonAddRequest request2 = new PersonAddRequest
        {
            PersonName = "Jane",
            Email = "john@example.com" // Same email
        };
        
        _personsService.AddPerson(request1);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
        {
            _personsService.AddPerson(request2);
        });
    }
    
    #endregion
}
```

### Testing READ Operations

```csharp
public class PersonsServiceTest
{
    #region GetAllPersons Tests
    
    [Fact]
    public void GetAllPersons_EmptyList_ReturnsEmptyList()
    {
        // Act
        List<PersonResponse> persons = _personsService.GetAllPersons();
        
        // Assert
        Assert.Empty(persons);
    }
    
    [Fact]
    public void GetAllPersons_WithFewPersons_ReturnsAllPersons()
    {
        // Arrange
        var country = _countriesService.AddCountry(new CountryAddRequest { CountryName = "USA" });
        
        List<PersonAddRequest> personRequests = new List<PersonAddRequest>
        {
            new PersonAddRequest { PersonName = "John", Email = "john@test.com", CountryId = country.CountryId },
            new PersonAddRequest { PersonName = "Jane", Email = "jane@test.com", CountryId = country.CountryId },
            new PersonAddRequest { PersonName = "Bob", Email = "bob@test.com", CountryId = country.CountryId }
        };
        
        List<PersonResponse> expectedPersons = new List<PersonResponse>();
        foreach (var request in personRequests)
        {
            expectedPersons.Add(_personsService.AddPerson(request));
        }
        
        // Act
        List<PersonResponse> actualPersons = _personsService.GetAllPersons();
        
        // Assert
        Assert.Equal(3, actualPersons.Count);
        foreach (var expectedPerson in expectedPersons)
        {
            Assert.Contains(expectedPerson, actualPersons);
        }
    }
    
    #endregion
    
    #region GetPersonByPersonId Tests
    
    [Fact]
    public void GetPersonByPersonId_NullPersonId_ReturnsNull()
    {
        // Arrange
        Guid? personId = null;
        
        // Act
        PersonResponse? response = _personsService.GetPersonByPersonId(personId);
        
        // Assert
        Assert.Null(response);
    }
    
    [Fact]
    public void GetPersonByPersonId_InvalidPersonId_ReturnsNull()
    {
        // Arrange
        Guid personId = Guid.NewGuid();
        
        // Act
        PersonResponse? response = _personsService.GetPersonByPersonId(personId);
        
        // Assert
        Assert.Null(response);
    }
    
    [Fact]
    public void GetPersonByPersonId_ValidPersonId_ReturnsCorrectPerson()
    {
        // Arrange
        var country = _countriesService.AddCountry(new CountryAddRequest { CountryName = "Canada" });
        PersonAddRequest request = new PersonAddRequest
        {
            PersonName = "Alice",
            Email = "alice@test.com",
            CountryId = country.CountryId
        };
        
        PersonResponse addedPerson = _personsService.AddPerson(request);
        
        // Act
        PersonResponse? retrievedPerson = _personsService.GetPersonByPersonId(addedPerson.PersonId);
        
        // Assert
        Assert.NotNull(retrievedPerson);
        Assert.Equal(addedPerson.PersonId, retrievedPerson.PersonId);
        Assert.Equal(addedPerson.PersonName, retrievedPerson.PersonName);
        Assert.Equal(addedPerson.Email, retrievedPerson.Email);
    }
    
    #endregion
}
```

### Testing UPDATE Operations

```csharp
public class PersonsServiceTest
{
    #region UpdatePerson Tests
    
    [Fact]
    public void UpdatePerson_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        PersonUpdateRequest? request = null;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
        {
            _personsService.UpdatePerson(request);
        });
    }
    
    [Fact]
    public void UpdatePerson_InvalidPersonId_ThrowsArgumentException()
    {
        // Arrange
        PersonUpdateRequest request = new PersonUpdateRequest
        {
            PersonId = Guid.NewGuid(), // Non-existent ID
            PersonName = "Updated Name"
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
        {
            _personsService.UpdatePerson(request);
        });
    }
    
    [Fact]
    public void UpdatePerson_ValidDetails_ReturnsUpdatedPerson()
    {
        // Arrange
        var country = _countriesService.AddCountry(new CountryAddRequest { CountryName = "UK" });
        
        PersonAddRequest addRequest = new PersonAddRequest
        {
            PersonName = "Original Name",
            Email = "original@test.com",
            CountryId = country.CountryId
        };
        
        PersonResponse addedPerson = _personsService.AddPerson(addRequest);
        
        PersonUpdateRequest updateRequest = new PersonUpdateRequest
        {
            PersonId = addedPerson.PersonId,
            PersonName = "Updated Name",
            Email = "updated@test.com",
            Address = "New Address"
        };
        
        // Act
        PersonResponse updatedPerson = _personsService.UpdatePerson(updateRequest);
        PersonResponse? retrievedPerson = _personsService.GetPersonByPersonId(addedPerson.PersonId);
        
        // Assert
        Assert.Equal(addedPerson.PersonId, updatedPerson.PersonId);
        Assert.Equal("Updated Name", updatedPerson.PersonName);
        Assert.Equal("updated@test.com", updatedPerson.Email);
        Assert.Equal("New Address", updatedPerson.Address);
        Assert.Equal(updatedPerson, retrievedPerson);
    }
    
    [Fact]
    public void UpdatePerson_UpdateEmailToDuplicate_ThrowsArgumentException()
    {
        // Arrange
        PersonAddRequest request1 = new PersonAddRequest
        {
            PersonName = "Person1",
            Email = "person1@test.com"
        };
        
        PersonAddRequest request2 = new PersonAddRequest
        {
            PersonName = "Person2",
            Email = "person2@test.com"
        };
        
        PersonResponse person1 = _personsService.AddPerson(request1);
        PersonResponse person2 = _personsService.AddPerson(request2);
        
        PersonUpdateRequest updateRequest = new PersonUpdateRequest
        {
            PersonId = person2.PersonId,
            PersonName = "Person2",
            Email = "person1@test.com" // Duplicate email
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
        {
            _personsService.UpdatePerson(updateRequest);
        });
    }
    
    #endregion
}
```

### Testing DELETE Operations

```csharp
public class PersonsServiceTest
{
    #region DeletePerson Tests
    
    [Fact]
    public void DeletePerson_InvalidPersonId_ReturnsFalse()
    {
        // Arrange
        Guid personId = Guid.NewGuid();
        
        // Act
        bool result = _personsService.DeletePerson(personId);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void DeletePerson_ValidPersonId_ReturnsTrue()
    {
        // Arrange
        PersonAddRequest request = new PersonAddRequest
        {
            PersonName = "ToDelete",
            Email = "delete@test.com"
        };
        
        PersonResponse addedPerson = _personsService.AddPerson(request);
        
        // Act
        bool deleteResult = _personsService.DeletePerson(addedPerson.PersonId);
        PersonResponse? retrievedPerson = _personsService.GetPersonByPersonId(addedPerson.PersonId);
        
        // Assert
        Assert.True(deleteResult);
        Assert.Null(retrievedPerson);
    }
    
    [Fact]
    public void DeletePerson_RemovesFromAllPersonsList()
    {
        // Arrange
        PersonAddRequest request1 = new PersonAddRequest { PersonName = "Person1", Email = "p1@test.com" };
        PersonAddRequest request2 = new PersonAddRequest { PersonName = "Person2", Email = "p2@test.com" };
        PersonAddRequest request3 = new PersonAddRequest { PersonName = "Person3", Email = "p3@test.com" };
        
        PersonResponse person1 = _personsService.AddPerson(request1);
        PersonResponse person2 = _personsService.AddPerson(request2);
        PersonResponse person3 = _personsService.AddPerson(request3);
        
        // Act
        _personsService.DeletePerson(person2.PersonId);
        List<PersonResponse> remainingPersons = _personsService.GetAllPersons();
        
        // Assert
        Assert.Equal(2, remainingPersons.Count);
        Assert.Contains(person1, remainingPersons);
        Assert.DoesNotContain(person2, remainingPersons);
        Assert.Contains(person3, remainingPersons);
    }
    
    #endregion
}
```

---

## Mocking Dependencies

Mocking allows you to isolate the unit under test by replacing its dependencies with controlled test doubles.

### Why Mock?

✅ **Isolation:** Test only the code you're interested in  
✅ **Speed:** Avoid slow operations (database, network)  
✅ **Control:** Simulate specific scenarios (errors, edge cases)  
✅ **Focus:** Test your logic, not external systems  

### Popular Mocking Frameworks

| Framework | Characteristics |
|-----------|-----------------|
| **Moq** | Most popular, rich feature set, lambda-based setup |
| **NSubstitute** | Simpler syntax, more readable, good for beginners |
| **FakeItEasy** | Easy to learn, fluent interface, great documentation |

### Moq Basics

#### Installation

```bash
dotnet add package Moq
```

#### Creating Mocks

```csharp
using Moq;

// Create a mock of an interface
var mockRepository = new Mock<IPersonRepository>();

// Create a mock with strict behavior (throws on unexpected calls)
var strictMock = new Mock<IPersonRepository>(MockBehavior.Strict);
```

#### Setup Method Calls

```csharp
// Setup a method to return a specific value
mockRepository
    .Setup(repo => repo.GetById(It.IsAny<Guid>()))
    .Returns(new Person { Id = Guid.NewGuid(), Name = "John" });

// Setup with specific parameter
mockRepository
    .Setup(repo => repo.GetById(specificId))
    .Returns(expectedPerson);

// Setup to throw an exception
mockRepository
    .Setup(repo => repo.GetById(It.IsAny<Guid>()))
    .Throws(new NotFoundException("Person not found"));

// Setup async methods
mockRepository
    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
    .ReturnsAsync(new Person { Id = Guid.NewGuid(), Name = "John" });

// Setup with callback
mockRepository
    .Setup(repo => repo.Add(It.IsAny<Person>()))
    .Returns((Person p) => p) // Return the input
    .Callback<Person>(p => Console.WriteLine($"Adding {p.Name}"));
```

#### Argument Matchers

```csharp
// Any value of type
mockRepo.Setup(r => r.GetById(It.IsAny<Guid>()))

// Specific value
mockRepo.Setup(r => r.GetById(specificGuid))

// Value that matches condition
mockRepo.Setup(r => r.GetByAge(It.Is<int>(age => age >= 18)))

// Range of values
mockRepo.Setup(r => r.GetByAge(It.IsInRange(18, 65, Range.Inclusive)))

// Regex match
mockRepo.Setup(r => r.GetByName(It.IsRegex(@"^[A-Z]"))) // Names starting with uppercase
```

#### Verifying Calls

```csharp
// Verify method was called once
mockRepository.Verify(repo => repo.GetById(It.IsAny<Guid>()), Times.Once);

// Verify method was called specific number of times
mockRepository.Verify(repo => repo.Add(It.IsAny<Person>()), Times.Exactly(3));

// Verify method was never called
mockRepository.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);

// Verify method was called at least once
mockRepository.Verify(repo => repo.GetAll(), Times.AtLeastOnce);

// Verify all setups were called
mockRepository.VerifyAll();

// Verify no other calls were made
mockRepository.VerifyNoOtherCalls();
```

### Complete Mocking Example

```csharp
public interface IPersonRepository
{
    Person? GetById(Guid id);
    List<Person> GetAll();
    Person Add(Person person);
    Person Update(Person person);
    bool Delete(Guid id);
}

public class PersonService
{
    private readonly IPersonRepository _repository;
    private readonly ILogger<PersonService> _logger;
    
    public PersonService(IPersonRepository repository, ILogger<PersonService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public PersonResponse AddPerson(PersonAddRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (string.IsNullOrEmpty(request.PersonName))
            throw new ArgumentException("Person name cannot be empty");
        
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = request.PersonName,
            Email = request.Email
        };
        
        var addedPerson = _repository.Add(person);
        _logger.LogInformation("Person added: {PersonId}", addedPerson.Id);
        
        return new PersonResponse
        {
            PersonId = addedPerson.Id,
            PersonName = addedPerson.Name,
            Email = addedPerson.Email
        };
    }
}

// Test class using Moq
public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _mockRepository;
    private readonly Mock<ILogger<PersonService>> _mockLogger;
    private readonly PersonService _service;
    
    public PersonServiceTests()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _mockLogger = new Mock<ILogger<PersonService>>();
        _service = new PersonService(_mockRepository.Object, _mockLogger.Object);
    }
    
    [Fact]
    public void AddPerson_ValidRequest_CallsRepositoryAndReturnsResponse()
    {
        // Arrange
        var request = new PersonAddRequest
        {
            PersonName = "John Doe",
            Email = "john@example.com"
        };
        
        var expectedPerson = new Person
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@example.com"
        };
        
        _mockRepository
            .Setup(repo => repo.Add(It.Is<Person>(p => 
                p.Name == "John Doe" && p.Email == "john@example.com")))
            .Returns(expectedPerson);
        
        // Act
        var result = _service.AddPerson(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPerson.Id, result.PersonId);
        Assert.Equal("John Doe", result.PersonName);
        Assert.Equal("john@example.com", result.Email);
        
        _mockRepository.Verify(
            repo => repo.Add(It.IsAny<Person>()), 
            Times.Once);
        
        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Person added")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
    
    [Fact]
    public void AddPerson_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        PersonAddRequest? request = null;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.AddPerson(request));
        
        // Verify repository was never called
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Person>()), Times.Never);
    }
    
    [Fact]
    public void AddPerson_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var request = new PersonAddRequest { PersonName = "John", Email = "john@test.com" };
        
        _mockRepository
            .Setup(repo => repo.Add(It.IsAny<Person>()))
            .Throws(new InvalidOperationException("Database error"));
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.AddPerson(request));
        
        Assert.Equal("Database error", exception.Message);
    }
}
```

### Mocking HttpContext in Controllers

```csharp
public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    
    public PersonsController(IPersonsService personsService)
    {
        _personsService = personsService;
    }
    
    [HttpPost]
    public IActionResult Create(PersonAddRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var response = _personsService.AddPerson(request);
        return CreatedAtAction(nameof(Get), new { id = response.PersonId }, response);
    }
}

// Test class
public class PersonsControllerTests
{
    private readonly Mock<IPersonsService> _mockService;
    private readonly PersonsController _controller;
    
    public PersonsControllerTests()
    {
        _mockService = new Mock<IPersonsService>();
        _controller = new PersonsController(_mockService.Object);
        
        // Setup ModelState for validation testing
        _controller.ModelState.Clear();
    }
    
    [Fact]
    public void Create_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new PersonAddRequest
        {
            PersonName = "John",
            Email = "john@test.com"
        };
        
        var expectedResponse = new PersonResponse
        {
            PersonId = Guid.NewGuid(),
            PersonName = "John",
            Email = "john@test.com"
        };
        
        _mockService
            .Setup(s => s.AddPerson(It.IsAny<PersonAddRequest>()))
            .Returns(expectedResponse);
        
        // Act
        var result = _controller.Create(request);
        
        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PersonsController.Get), createdResult.ActionName);
        Assert.Equal(expectedResponse.PersonId, ((dynamic)createdResult.RouteValues)?.id);
        
        var returnedPerson = Assert.IsType<PersonResponse>(createdResult.Value);
        Assert.Equal(expectedResponse.PersonId, returnedPerson.PersonId);
        
        _mockService.Verify(s => s.AddPerson(request), Times.Once);
    }
    
    [Fact]
    public void Create_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = new PersonAddRequest { PersonName = "John" };
        _controller.ModelState.AddModelError("Email", "Email is required");
        
        // Act
        var result = _controller.Create(request);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
        
        _mockService.Verify(s => s.AddPerson(It.IsAny<PersonAddRequest>()), Times.Never);
    }
}
```

---

## Best Practices

### 1. Test Naming Conventions

Good test names clearly communicate what is being tested and the expected outcome.

#### Pattern 1: MethodName_StateUnderTest_ExpectedBehavior

```csharp
[Fact]
public void AddPerson_NullRequest_ThrowsArgumentNullException()

[Fact]
public void GetAllPersons_EmptyDatabase_ReturnsEmptyList()

[Fact]
public void UpdatePerson_ValidDetails_ReturnsUpdatedPerson()

[Fact]
public void DeletePerson_NonExistentId_ReturnsFalse()
```

#### Pattern 2: Given_When_Then (BDD Style)

```csharp
[Fact]
public void Given_NullRequest_When_AddingPerson_Then_ThrowsException()

[Fact]
public void Given_ValidPerson_When_Saving_Then_ReturnsPersonWithId()

[Fact]
public void Given_ExistingEmail_When_AddingPerson_Then_ThrowsDuplicateException()
```

#### Pattern 3: Should_ExpectedBehavior_When_StateUnderTest

```csharp
[Fact]
public void Should_ThrowException_When_PersonNameIsNull()

[Fact]
public void Should_ReturnAllPersons_When_PersonsExist()

[Fact]
public void Should_UpdateSuccessfully_When_ValidDetailsProvided()
```

### 2. One Assert Per Test (Ideally)

```csharp
// ❌ BAD: Testing multiple things in one test
[Fact]
public void AddPerson_ChecksEverything()
{
    var person = _service.AddPerson(request);
    Assert.NotNull(person);
    Assert.NotEqual(Guid.Empty, person.PersonId);
    Assert.Equal("John", person.PersonName);
    Assert.True(person.ReceiveNewsLetters);
    Assert.NotNull(person.CountryName);
    // Too many things being tested!
}

// ✅ GOOD: Focused tests
[Fact]
public void AddPerson_ValidRequest_GeneratesNonEmptyPersonId()
{
    var person = _service.AddPerson(request);
    Assert.NotEqual(Guid.Empty, person.PersonId);
}

[Fact]
public void AddPerson_ValidRequest_PreservesPersonName()
{
    var person = _service.AddPerson(request);
    Assert.Equal("John", person.PersonName);
}

[Fact]
public void AddPerson_ValidRequest_IncludesCountryInformation()
{
    var person = _service.AddPerson(request);
    Assert.NotNull(person.CountryName);
}
```

### 3. Test Independence

```csharp
// ❌ BAD: Tests depend on each other
public class BadTests
{
    private static Guid _personId; // Shared state!
    
    [Fact]
    public void Test1_AddPerson()
    {
        var person = _service.AddPerson(request);
        _personId = person.PersonId; // Storing for next test
    }
    
    [Fact]
    public void Test2_GetPerson() // Depends on Test1
    {
        var person = _service.GetPerson(_personId);
        Assert.NotNull(person);
    }
}

// ✅ GOOD: Independent tests
public class GoodTests
{
    [Fact]
    public void AddPerson_ValidRequest_CanBeRetrieved()
    {
        // Arrange
        var addedPerson = _service.AddPerson(request);
        
        // Act
        var retrievedPerson = _service.GetPerson(addedPerson.PersonId);
        
        // Assert
        Assert.NotNull(retrievedPerson);
        Assert.Equal(addedPerson.PersonId, retrievedPerson.PersonId);
    }
}
```

### 4. Don't Test Implementation Details

```csharp
// ❌ BAD: Testing internal implementation
[Fact]
public void AddPerson_UsesSpecificAlgorithm()
{
    // This test knows too much about HOW the method works
    _mockRepository.Verify(r => r.CheckDuplicateEmail(It.IsAny<string>()), Times.Once);
    _mockRepository.Verify(r => r.ValidateCountry(It.IsAny<Guid>()), Times.Once);
    _mockRepository.Verify(r => r.SaveToDatabase(It.IsAny<Person>()), Times.Once);
}

// ✅ GOOD: Testing behavior
[Fact]
public void AddPerson_ValidRequest_ReturnsPersonWithCorrectDetails()
{
    // Arrange
    var request = new PersonAddRequest { PersonName = "John", Email = "john@test.com" };
    
    // Act
    var result = _service.AddPerson(request);
    
    // Assert
    Assert.Equal("John", result.PersonName);
    Assert.Equal("john@test.com", result.Email);
    Assert.NotEqual(Guid.Empty, result.PersonId);
}
```

### 5. Use Theory for Similar Tests

```csharp
// ❌ BAD: Repetitive tests
[Fact]
public void AddPerson_EmptyName_ThrowsException()
{
    var request = new PersonAddRequest { PersonName = "" };
    Assert.Throws<ArgumentException>(() => _service.AddPerson(request));
}

[Fact]
public void AddPerson_WhitespaceName_ThrowsException()
{
    var request = new PersonAddRequest { PersonName = "   " };
    Assert.Throws<ArgumentException>(() => _service.AddPerson(request));
}

[Fact]
public void AddPerson_NullName_ThrowsException()
{
    var request = new PersonAddRequest { PersonName = null };
    Assert.Throws<ArgumentException>(() => _service.AddPerson(request));
}

// ✅ GOOD: Single theory test
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
[InlineData("\t")]
public void AddPerson_InvalidName_ThrowsArgumentException(string invalidName)
{
    // Arrange
    var request = new PersonAddRequest { PersonName = invalidName };
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => _service.AddPerson(request));
}
```

### 6. Keep Tests Simple and Readable

```csharp
// ❌ BAD: Complex test with lots of logic
[Fact]
public void ComplexTest()
{
    var persons = new List<Person>();
    for (int i = 0; i < 10; i++)
    {
        var person = new Person();
        if (i % 2 == 0)
        {
            person.Name = "Even" + i;
        }
        else
        {
            person.Name = "Odd" + i;
        }
        persons.Add(person);
        _service.AddPerson(ConvertToPerson(person));
    }
    
    var result = _service.GetAllPersons();
    var evenCount = result.Where(p => p.PersonName.StartsWith("Even")).Count();
    Assert.Equal(5, evenCount);
}

// ✅ GOOD: Simple, clear test
[Fact]
public void GetAllPersons_WithEvenAndOddNames_ReturnsCorrectCount()
{
    // Arrange
    _service.AddPerson(new PersonAddRequest { PersonName = "Even0" });
    _service.AddPerson(new PersonAddRequest { PersonName = "Odd1" });
    _service.AddPerson(new PersonAddRequest { PersonName = "Even2" });
    _service.AddPerson(new PersonAddRequest { PersonName = "Odd3" });
    _service.AddPerson(new PersonAddRequest { PersonName = "Even4" });
    
    // Act
    var result = _service.GetAllPersons();
    var evenCount = result.Count(p => p.PersonName.StartsWith("Even"));
    
    // Assert
    Assert.Equal(3, evenCount);
}
```

### 7. Test Edge Cases and Boundaries

```csharp
[Theory]
[InlineData(0)]      // Boundary: minimum
[InlineData(1)]      // Just above minimum
[InlineData(18)]     // Legal age boundary
[InlineData(100)]    // Normal case
[InlineData(120)]    // Maximum reasonable age
[InlineData(150)]    // Beyond reasonable
public void AddPerson_VariousAges_HandlesCorrectly(int age)
{
    // Test implementation
}

[Fact]
public void AddPerson_MaximumLengthName_AcceptsSuccessfully()
{
    var longName = new string('A', 100); // Assuming 100 is max length
    var request = new PersonAddRequest { PersonName = longName };
    
    var result = _service.AddPerson(request);
    
    Assert.Equal(longName, result.PersonName);
}

[Fact]
public void AddPerson_ExceedsMaximumLengthName_ThrowsException()
{
    var tooLongName = new string('A', 101); // Exceeds max length
    var request = new PersonAddRequest { PersonName = tooLongName };
    
    Assert.Throws<ArgumentException>(() => _service.AddPerson(request));
}
```

### 8. Avoid Testing Private Methods

```csharp
// ❌ BAD: Using reflection to test private methods
[Fact]
public void PrivateMethod_Test()
{
    var method = typeof(PersonService).GetMethod("ValidateEmail", 
        BindingFlags.NonPublic | BindingFlags.Instance);
    var result = method.Invoke(_service, new object[] { "test@test.com" });
    Assert.True((bool)result);
}

// ✅ GOOD: Test through public interface
[Fact]
public void AddPerson_InvalidEmail_ThrowsException()
{
    // The private ValidateEmail method is tested indirectly
    var request = new PersonAddRequest 
    { 
        PersonName = "John",
        Email = "invalid-email" 
    };
    
    Assert.Throws<ArgumentException>(() => _service.AddPerson(request));
}
```

### 9. Use Test Data Builders

```csharp
// Test data builder pattern
public class PersonAddRequestBuilder
{
    private string _personName = "Default Name";
    private string _email = "default@test.com";
    private DateTime? _dateOfBirth = new DateTime(1990, 1, 1);
    private string? _gender = "Male";
    private Guid? _countryId = Guid.NewGuid();
    
    public PersonAddRequestBuilder WithName(string name)
    {
        _personName = name;
        return this;
    }
    
    public PersonAddRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }
    
    public PersonAddRequestBuilder WithDateOfBirth(DateTime dateOfBirth)
    {
        _dateOfBirth = dateOfBirth;
        return this;
    }
    
    public PersonAddRequest Build()
    {
        return new PersonAddRequest
        {
            PersonName = _personName,
            Email = _email,
            DateOfBirth = _dateOfBirth,
            Gender = _gender,
            CountryId = _countryId
        };
    }
}

// Usage in tests
[Fact]
public void AddPerson_ValidRequest_Success()
{
    // Arrange
    var request = new PersonAddRequestBuilder()
        .WithName("John Doe")
        .WithEmail("john@test.com")
        .Build();
    
    // Act
    var result = _service.AddPerson(request);
    
    // Assert
    Assert.NotNull(result);
}
```

### 10. Clean Up After Tests

```csharp
public class PersonServiceTests : IDisposable
{
    private readonly PersonService _service;
    private readonly List<Guid> _createdPersonIds = new();
    
    public PersonServiceTests()
    {
        _service = new PersonService();
    }
    
    [Fact]
    public void AddPerson_Test()
    {
        var person = _service.AddPerson(request);
        _createdPersonIds.Add(person.PersonId);
        // Test logic
    }
    
    public void Dispose()
    {
        // Clean up test data
        foreach (var id in _createdPersonIds)
        {
            _service.DeletePerson(id);
        }
    }
}
```

---

## Common Patterns

### Testing Async Methods

```csharp
public class PersonServiceAsync
{
    private readonly IPersonRepository _repository;
    
    public async Task<PersonResponse> AddPersonAsync(PersonAddRequest request)
    {
        // Implementation
        var person = await _repository.AddAsync(new Person());
        return new PersonResponse { PersonId = person.Id };
    }
}

public class PersonServiceAsyncTests
{
    [Fact]
    public async Task AddPersonAsync_ValidRequest_ReturnsPersonResponse()
    {
        // Arrange
        var mockRepo = new Mock<IPersonRepository>();
        mockRepo
            .Setup(r => r.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(new Person { Id = Guid.NewGuid() });
        
        var service = new PersonServiceAsync(mockRepo.Object);
        var request = new PersonAddRequest { PersonName = "John" };
        
        // Act
        var result = await service.AddPersonAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.PersonId);
    }
}
```

### Testing Exception Messages

```csharp
[Fact]
public void AddPerson_NullName_ThrowsExceptionWithMessage()
{
    // Arrange
    var request = new PersonAddRequest { PersonName = null };
    
    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() => 
        _service.AddPerson(request));
    
    Assert.Equal("Person name cannot be null or empty", exception.Message);
    Assert.Equal("PersonName", exception.ParamName);
}
```

### Testing Collections

```csharp
[Fact]
public void GetAllPersons_ReturnsCorrectOrder()
{
    // Arrange
    var person1 = _service.AddPerson(new PersonAddRequest { PersonName = "Alice" });
    var person2 = _service.AddPerson(new PersonAddRequest { PersonName = "Bob" });
    var person3 = _service.AddPerson(new PersonAddRequest { PersonName = "Charlie" });
    
    // Act
    var result = _service.GetAllPersons();
    
    // Assert
    Assert.Equal(3, result.Count);
    Assert.Collection(result,
        p => Assert.Equal("Alice", p.PersonName),
        p => Assert.Equal("Bob", p.PersonName),
        p => Assert.Equal("Charlie", p.PersonName)
    );
}
```

### Using Custom Assertions (FluentAssertions)

```bash
dotnet add package FluentAssertions
```

```csharp
using FluentAssertions;

[Fact]
public void AddPerson_ValidRequest_ReturnsCorrectPerson()
{
    // Arrange
    var request = new PersonAddRequest 
    { 
        PersonName = "John",
        Email = "john@test.com",
        DateOfBirth = new DateTime(1990, 1, 1)
    };
    
    // Act
    var result = _service.AddPerson(request);
    
    // Assert
    result.Should().NotBeNull();
    result.PersonId.Should().NotBeEmpty();
    result.PersonName.Should().Be("John");
    result.Email.Should().Be("john@test.com");
    result.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
    
    // More fluent assertions
    result.PersonName.Should().StartWith("J").And.HaveLength(4);
    result.Email.Should().Match("*@test.com");
}

[Fact]
public void GetAllPersons_ReturnsExpectedPersons()
{
    // Arrange & Act
    _service.AddPerson(new PersonAddRequest { PersonName = "John" });
    _service.AddPerson(new PersonAddRequest { PersonName = "Jane" });
    var result = _service.GetAllPersons();
    
    // Assert
    result.Should().HaveCount(2);
    result.Should().Contain(p => p.PersonName == "John");
    result.Should().OnlyContain(p => p.PersonId != Guid.Empty);
    result.Select(p => p.PersonName).Should().BeEquivalentTo(new[] { "John", "Jane" });
}
```

---

## Interview Preparation

### Key Concepts to Master

#### 1. What is Unit Testing?

**Answer:** Unit testing is testing individual units of code (classes, methods) in isolation from dependencies to verify they work correctly. Benefits include early bug detection, confident refactoring, improved design, and living documentation.

#### 2. Explain AAA Pattern

**Answer:** AAA stands for Arrange-Act-Assert:
- **Arrange**: Set up test data, create objects, configure mocks
- **Act**: Execute the method under test
- **Assert**: Verify the outcome matches expectations

This pattern makes tests clear, structured, and easy to understand.

#### 3. Difference Between [Fact] and [Theory]

**Answer:**
- **[Fact]**: Single test case with no parameters
- **[Theory]**: Data-driven test that runs multiple times with different inputs using [InlineData], [MemberData], or [ClassData]

#### 4. What is Mocking and Why Use It?

**Answer:** Mocking is creating fake objects that simulate real dependencies. We use mocking to:
- Isolate the unit under test
- Control dependency behavior
- Avoid hitting external systems (databases, APIs)
- Test edge cases and error scenarios
- Speed up tests

#### 5. Unit Tests vs Integration Tests

| Aspect | Unit Tests | Integration Tests |
|--------|------------|-------------------|
| **Scope** | Single unit | Multiple components |
| **Dependencies** | Mocked | Real or partially real |
| **Speed** | Fast (ms) | Slower (seconds) |
| **Purpose** | Test logic | Test interactions |
| **Database** | Never | May use test DB |

### Sample Interview Questions

**Q: How do you test a method that throws an exception?**

```csharp
[Fact]
public void Method_InvalidInput_ThrowsException()
{
    // Arrange
    var service = new MyService();
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => service.Method("invalid"));
}
```

**Q: How do you test async methods?**

```csharp
[Fact]
public async Task MethodAsync_ValidInput_ReturnsResult()
{
    // Arrange
    var service = new MyService();
    
    // Act
    var result = await service.MethodAsync();
    
    // Assert
    Assert.NotNull(result);
}
```

**Q: What is Test-Driven Development (TDD)?**

**Answer:** TDD is a development approach following the Red-Green-Refactor cycle:
1. **Red**: Write a failing test first
2. **Green**: Write minimal code to pass the test
3. **Refactor**: Improve code while keeping tests passing

Benefits: Better design, high test coverage, fewer bugs.

**Q: How do you verify a mock was called?**

```csharp
mockRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
mockRepository.Verify(r => r.Add(It.IsAny<Person>()), Times.Exactly(3));
mockRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
```

**Q: What makes a good unit test?**

**Answer:** A good unit test is:
- **Fast**: Runs in milliseconds
- **Isolated**: Tests one unit without dependencies
- **Repeatable**: Same result every time
- **Self-Validating**: Clear pass/fail
- **Focused**: Tests one behavior
- **Readable**: Clear what's being tested

### Common Challenges

**Q: How do you test private methods?**

**Answer:** Don't test private methods directly. Test them indirectly through public methods. If you feel the need to test a private method, it might indicate the method should be public in a separate class.

**Q: How do you test static methods?**

**Answer:** Static methods are hard to test. Best practices:
- Avoid static methods when possible
- Wrap static methods in instance methods
- Use dependency injection instead

**Q: What is test coverage and what's a good target?**

**Answer:** Test coverage measures the percentage of code executed by tests. While 100% isn't always necessary, aim for:
- 80%+ for critical business logic
- 100% for complex algorithms
- Lower for simple getters/setters

Focus on testing important paths rather than chasing percentage.

---

## Quick Reference

### xUnit Attributes

```csharp
[Fact]                              // Simple test
[Theory]                            // Data-driven test
[InlineData(1, 2, 3)]              // Inline test data
[MemberData(nameof(TestData))]     // Test data from member
[ClassData(typeof(TestDataClass))] // Test data from class
[Trait("Category", "Unit")]        // Categorize tests
[Skip("Reason")]                   // Skip test with reason
```

### Common Assertions

```csharp
// Equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Booleans
Assert.True(condition);
Assert.False(condition);

// Nullability
Assert.Null(obj);
Assert.NotNull(obj);

// Types
Assert.IsType<T>(obj);
Assert.IsAssignableFrom<T>(obj);

// Exceptions
Assert.Throws<TException>(() => code);

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Contains(item, collection);
Assert.DoesNotContain(item, collection);
Assert.Single(collection);
Assert.Collection(collection, assertions...);

// Strings
Assert.StartsWith(prefix, text);
Assert.EndsWith(suffix, text);
Assert.Contains(substring, text);
Assert.Matches(regex, text);

// Ranges
Assert.InRange(value, low, high);
```

### Moq Setup Patterns

```csharp
// Return value
mock.Setup(m => m.Method()).Returns(value);

// Return async
mock.Setup(m => m.MethodAsync()).ReturnsAsync(value);

// Throw exception
mock.Setup(m => m.Method()).Throws<Exception>();

// Callback
mock.Setup(m => m.Method()).Callback(() => /* action */);

// Argument matchers
It.IsAny<T>()
It.Is<T>(x => condition)
It.IsInRange<T>(low, high, Range.Inclusive)
It.IsRegex(pattern)

// Verification
mock.Verify(m => m.Method(), Times.Once);
mock.Verify(m => m.Method(), Times.Never);
mock.Verify(m => m.Method(), Times.Exactly(3));
mock.VerifyAll();
mock.VerifyNoOtherCalls();
```

### Test Naming Patterns

```csharp
// Pattern 1: MethodName_StateUnderTest_ExpectedBehavior
AddPerson_NullRequest_ThrowsArgumentNullException

// Pattern 2: Given_When_Then
Given_NullRequest_When_AddingPerson_Then_ThrowsException

// Pattern 3: Should_ExpectedBehavior_When_StateUnderTest
Should_ThrowException_When_RequestIsNull
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests in specific project
dotnet test YourProject.Tests

# Run with detailed output
dotnet test --verbosity detailed

# Run specific test
dotnet test --filter FullyQualifiedName~NameOfTest

# Run tests by trait
dotnet test --filter Category=Unit

# Generate code coverage
dotnet test /p:CollectCoverage=true
```

---

## Summary

Unit testing with xUnit in ASP.NET Core is essential for building reliable, maintainable applications.

### Key Takeaways

✅ **AAA Pattern**: Structure tests as Arrange-Act-Assert for clarity  
✅ **Isolation**: Mock dependencies to test units in isolation  
✅ **xUnit**: Use [Fact] for simple tests, [Theory] for data-driven tests  
✅ **Moq**: Mock dependencies with Setup() and verify with Verify()  
✅ **Best Practices**: Clear names, one assert per test, test independence  
✅ **CRUD Testing**: Test all operations including edge cases  
✅ **Fast Tests**: Keep tests fast by avoiding external dependencies  

### Testing Workflow

1. **Write Test First** (optional TDD approach)
2. **Arrange**: Set up test data and mocks
3. **Act**: Execute method under test
4. **Assert**: Verify expected outcome
5. **Refactor**: Improve test and production code
