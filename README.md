# Spectre.Verify.Extensions

![Continuous Integration](https://github.com/spectresystems/spectre.verify.extensions/workflows/Continuous%20Integration/badge.svg)
[![NuGet Status](https://img.shields.io/nuget/v/Spectre.Verify.Extensions.svg)](https://www.nuget.org/packages/Spectre.Verify.Extensions/)

Add an attribute driven file naming convention to [Verify](https://github.com/VerifyTests/Verify).


## NuGet package

https://nuget.org/packages/Spectre.Verify.Extensions/


## Usage

At startup pass `Expectations.Initialize` to [VerifierSettings.DerivePathInfo](https://github.com/VerifyTests/Verify/blob/master/docs/naming.md#derivepathinfo):

<!-- snippet: Initialize -->
<a id='snippet-initialize'></a>
```cs
public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        Verifier.DerivePathInfo(Expectations.Initialize);
    }
}
```
<sup><a href='/src/Spectre.Verify.Extensions.Tests/ExpectationTests.cs#L40-L49' title='Snippet source file'>snippet source</a> | <a href='#snippet-initialize' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Then the following test

<!-- snippet: Usage -->
<a id='snippet-usage'></a>
```cs
[ExpectationPath("Foo")]
public static class ExpectationTests
{
    public static class MyOtherTestClass
    {
        [ExpectationPath("Bar/Qux")]
        public class MyOtherOtherTestClass
        {
            [Fact]
            [Expectation("Waldo")]
            public async Task Test1()
            {
                await Verifier.Verify("w00t");
            }
        }
    }

    [ExpectationPath("Baz")]
    public class YetAnotherTestClass
    {
        [Fact]
        [Expectation("Corgi", "Lol")]
        public async Task Test1()
        {
            await Verifier.Verify("lol");
        }
    }
}
```
<sup><a href='/src/Spectre.Verify.Extensions.Tests/ExpectationTests.cs#L9-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-usage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will produce the following directory structure:

```
ProjectDir
  - Expectations
    - Foo
      - Bar
        - Qux
          - Waldo.Output.verified.txt
      - Baz
        - Corgi.Lol.verified.txt
```
