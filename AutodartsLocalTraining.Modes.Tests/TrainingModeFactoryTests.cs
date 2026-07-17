using FluentAssertions;

namespace AutodartsLocalTraining.Modes.Tests;

public class TrainingModeFactoryTests
{
    [Fact]
    public void Create_RoundTheBoardSingleFields_ReturnsMatchingType()
    {
        var mode = TrainingModeFactory.Create(TrainingMode.RoundTheBoardSingleFields);

        mode.Should().BeOfType<RoundTheBoardSingleFields>();
        mode.PrimaryDisplayValue.Should().Be("1");
    }

    [Fact]
    public void Create_ThreeDartCheckout_ReturnsMatchingType()
    {
        var mode = TrainingModeFactory.Create(TrainingMode.ThreeDartCheckout);

        mode.Should().BeOfType<ThreeDartCheckout>();
        mode.PrimaryDisplayValue.Should().Be("3");
    }

    [Fact]
    public void Create_ReturnsFreshInstanceEachCall()
    {
        var first = TrainingModeFactory.Create(TrainingMode.RoundTheBoardSingleFields);
        var second = TrainingModeFactory.Create(TrainingMode.RoundTheBoardSingleFields);

        first.Should().NotBeSameAs(second);

        first.ProcessThrows([new DartThrow(1, 1)]);

        first.Score.Should().Be(1);
        second.Score.Should().Be(0);
    }
}
