using FluentAssertions;

namespace AutodartsLocalTraining.Modes.Tests;

public class RoundTheBoardSingleFieldsTests
{
    [Fact]
    public void InitialState_TargetsFirstNumber()
    {
        var mode = new RoundTheBoardSingleFields();

        mode.PrimaryDisplayValue.Should().Be("1");
        mode.Score.Should().Be(0);
        mode.IsComplete.Should().BeFalse();
        mode.CurrentTurnThrows.Should().BeEmpty();
    }

    [Fact]
    public void SingleHitOnTarget_IncrementsScore()
    {
        var mode = new RoundTheBoardSingleFields();

        mode.ProcessThrow(new DartThrow(1, 1));

        mode.Score.Should().Be(1);
    }

    [Theory]
    [InlineData(1, 2)] // double
    [InlineData(1, 3)] // triple
    [InlineData(2, 1)] // wrong number
    public void NonSingleOrWrongNumberHit_DoesNotScore(int number, int multiplier)
    {
        var mode = new RoundTheBoardSingleFields();

        mode.ProcessThrow(new DartThrow(number, multiplier));

        mode.Score.Should().Be(0);
    }

    [Fact]
    public void ThirdDart_CompletesTurnWithoutAdvancingDisplay()
    {
        var mode = new RoundTheBoardSingleFields();

        mode.ProcessThrow(new DartThrow(1, 1));
        mode.ProcessThrow(new DartThrow(1, 3));
        var turnCompleted = mode.ProcessThrow(new DartThrow(5, 1));

        turnCompleted.Should().BeTrue();
        mode.PrimaryDisplayValue.Should().Be("1");
        mode.CurrentTurnThrows.Should().HaveCount(3);
    }

    [Fact]
    public void ProcessThrow_NoOpsWhileTurnPendingAdvance()
    {
        var mode = new RoundTheBoardSingleFields();
        mode.ProcessThrow(new DartThrow(1, 1));
        mode.ProcessThrow(new DartThrow(1, 1));
        mode.ProcessThrow(new DartThrow(1, 1));

        var turnCompleted = mode.ProcessThrow(new DartThrow(1, 1));

        turnCompleted.Should().BeFalse();
        mode.Score.Should().Be(3);
    }

    [Fact]
    public void AdvanceToNextTurn_ClearsThrowsAndAdvancesTarget()
    {
        var mode = new RoundTheBoardSingleFields();
        mode.ProcessThrow(new DartThrow(1, 1));
        mode.ProcessThrow(new DartThrow(1, 1));
        mode.ProcessThrow(new DartThrow(1, 1));

        mode.AdvanceToNextTurn();

        mode.PrimaryDisplayValue.Should().Be("2");
        mode.CurrentTurnThrows.Should().BeEmpty();
    }

    [Fact]
    public void AdvanceToNextTurn_NoOpsWhenTurnNotCompleted()
    {
        var mode = new RoundTheBoardSingleFields();

        mode.AdvanceToNextTurn();

        mode.PrimaryDisplayValue.Should().Be("1");
    }

    [Fact]
    public void FullRun_CompletesAfterNumber20_WithExpectedMaxScore()
    {
        var mode = new RoundTheBoardSingleFields();

        mode.MaxScore.Should().Be(60);

        for (var target = 1; target <= 20; target++)
        {
            mode.ProcessThrow(new DartThrow(target, 1));
            mode.ProcessThrow(new DartThrow(target, 1));
            var turnCompleted = mode.ProcessThrow(new DartThrow(target, 1));
            turnCompleted.Should().BeTrue();
            mode.AdvanceToNextTurn();
        }

        mode.IsComplete.Should().BeTrue();
        mode.Score.Should().Be(60);
    }

    [Fact]
    public void ProcessThrow_NoOpsOnceComplete()
    {
        var mode = new RoundTheBoardSingleFields();
        for (var target = 1; target <= 20; target++)
        {
            mode.ProcessThrow(new DartThrow(target, 1));
            mode.ProcessThrow(new DartThrow(target, 1));
            mode.ProcessThrow(new DartThrow(target, 1));
            mode.AdvanceToNextTurn();
        }

        mode.ProcessThrow(new DartThrow(1, 1));

        mode.Score.Should().Be(60);
    }
}
