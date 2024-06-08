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
For more information and usages, check the Samples project.

## Known issues
There are currently two main issues in this repository:
### Unable to restore the original behavior
Once you have mocked the static class, the original behavior is lost. There is an experimental feature (`HijackRegister.EnableExperimentalDefaultInvoking`) which partially supports it, but it is far from being usable.
### Unable to mock a method if it has been called already
If you have executed the method before adding the mock, it won't be possible to set up any mocks until the end of the test run.