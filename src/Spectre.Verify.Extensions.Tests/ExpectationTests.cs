using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Spectre.Verify.Extensions.Tests
{
    [ExpectationPath("Foo")]
    public static class ExpectationTests
    {
        public static class MyOtherTestClass
        {
            [UsesVerify]
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

        [UsesVerify]
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

    public static class VerifyConfig
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifierSettings.DerivePathInfo(Expectations.Initialize);
        }
    }
}
