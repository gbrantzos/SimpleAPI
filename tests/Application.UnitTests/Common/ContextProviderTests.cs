using SimpleAPI.Application.Common;
using Xunit.Abstractions;

namespace SimpleAPI.Application.UnitTests.Common;

public class ContextProviderTests
{
    private readonly ITestOutputHelper _outputHelper;

    public ContextProviderTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    private class TestContext
    {
        public string ID { get; set; } = Guid.NewGuid().ToString("N");
    }

    [Fact]
    public void When_AwaitableIsCalled_ContextPreserved()
    {
        // Arrange
        var context = new TestContext();
        var contextProvider = new ContextProvider<TestContext>
        {
            CurrentContext = context
        };

        // Act
        _outputHelper.WriteLine(
            $"Before calling awaitable methods: {contextProvider.CurrentContext?.ID} - {Environment.CurrentManagedThreadId}");
        Task.Run(AwaitableModifiesContext);
        Task.Run(AwaitableOnlyConsumeContext);
        _outputHelper.WriteLine(
            $"After calling awaitable methods: {contextProvider.CurrentContext?.ID} - {Environment.CurrentManagedThreadId}");

        // Assert
        contextProvider.Should().NotBeNull();
        contextProvider.CurrentContext.Should().NotBeNull();
        contextProvider.CurrentContext.Should().BeEquivalentTo(context);

        return;


        async Task AwaitableOnlyConsumeContext()
        {
            await Task.Yield();
            _outputHelper.WriteLine(
                $"Inside consuming awaitable methods: {contextProvider.CurrentContext?.ID} - {Environment.CurrentManagedThreadId}");
            contextProvider.Should().NotBeNull();
            contextProvider.CurrentContext?.ID.Should().NotBeNull();
            contextProvider.CurrentContext.Should().BeEquivalentTo(context);
        }

        async Task AwaitableModifiesContext()
        {
            await Task.Yield();
            _outputHelper.WriteLine(
                $"Inside modifying awaitable methods: {contextProvider.CurrentContext?.ID} - {Environment.CurrentManagedThreadId}");
            contextProvider.Should().NotBeNull();
            contextProvider.CurrentContext.Should().NotBeNull();
            contextProvider.CurrentContext!.ID = "Modified ID";
            _outputHelper.WriteLine(
                $"Inside modifying awaitable methods: {contextProvider.CurrentContext?.ID} - {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
