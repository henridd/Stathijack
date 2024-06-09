# Stathijack

## Introduction
Stathijack is a tool that allows developers to either mock or replace static classes in unit or integration tests, without any changes to your production code whatsoever. You can easily replace the logic of static method to a simple Func, allowing you to use static classes in your code without fearing how that's going to be tested. This is possible by "hijacking" the original method, so whenever your code tries to access the static class, it is redirected to the specified mock or fake class.

## Usage
```csharp
using var hijacker = new HijackRegister();
var mockingHijacker = new MockingHijacker(typeof(StaticClassUnderTest), hijacker);
mockingHijacker.MockMethod("MethodName", (string nameOfTheParameter) =>
{
  //Some logic
});
```
For more information and usages, check the Samples project and this article: https://intodot.net/mocking-static-classes-in-net-introducing-stathijack/

## Contributing
If you are in need of new functionalities, feel free to raise an issue. I'd also gladly accept help regarding the issues mentioned in the 'Known issues' section.

## Known issues
There are currently two main issues in this repository:
### Unable to restore the original behavior
Once you have mocked the static class, the original behavior is lost. There is an experimental feature (`HijackRegister.EnableExperimentalDefaultInvoking`) which partially supports it, but it is far from being usable.
### Unable to mock a method if it has been called already
If you have executed the method before adding the mock, it won't be possible to set up any mocks until the end of the test run.

There is also a weird behavior that is better shown than explained. Take the following situation:
```csharp
[Test]
public void Test1()
{
    using var hijacker = new HijackRegister();
    var mockingHijacker = new MockingHijacker(typeof(Factory), hijacker);
    mockingHijacker.MockMethod(nameof(Factory.Create), (string _) => { return "Fake result"; });
    var test = new FactoryConsumer().Consume("Real result");
    var test2 = Factory.Create("Real result");
}

public class FactoryConsumer
{
    public string Consume(string teste)
    {
        return Factory.Create(teste);
    }
}

public static class Factory
{
    public static string Create(string teste)
    {
        return teste;
    }
}
```
When you execute this test, both "test" and "test2" variables will have "Fake result" as their value. However, if you swap their order to something like this:
```csharp
var test = new FactoryConsumer().Consume("Real result");
var test2 = Factory.Create("Real result");
```
the mocking won't be applied at all, and their value will be "Real result".
