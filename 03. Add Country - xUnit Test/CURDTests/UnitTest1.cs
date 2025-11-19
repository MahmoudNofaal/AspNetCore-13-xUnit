using CURDTests;
using Xunit;

namespace CURDTests;

public class UnitTest1
{

   // the atribute [Fact] is used to indicate that this is a test method
   // it is part of the xUnit testing framework
   [Fact]
   public void Test1()
   {
      // the are three main parts of a unit test: Arrange, Act, Assert
      // Arrange: set up any necessary objects or conditions for the test

      var m = new MyMath();

      int input1 = 10, input2 = 5;
      int expected = 15; // expected result of adding input1 and input2

      // Act: perform the action that you want to test

      // Actual result of adding input1 and input2
      int actual = m.Add(input1, input2); // Call the method being tested

      // Assert: verify that the action produced the expected result

      Assert.Equal(expected, actual); // Check if actual result matches expected result

   }

}

