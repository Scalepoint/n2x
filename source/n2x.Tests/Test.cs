using NUnit.Framework;

namespace n2x
{
    public struct TestCategoryProvider
    {
        public const string FullRegression = "FullRegression";
    }

    [Xunit.Trait("Category", TestCategoryProvider.FullRegression)]
    public class Test
    {
        [Test]
        public void should_do_the_magic()
        {
        }
    }
}

