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

        mode.ProcessThrows([new DartThrow(1, 1)]);

        mode.Score.Should().Be(1);
    }

    [Theory]
    [InlineData(1, 2)] // double
    [InlineData(1, 3)] // triple
    [InlineData(2, 1)] // wrong number
    public void NonSingleOrWrongNumberHit_DoesNotScore(int number, int multiplier)
    {
        var mode = new RoundTheBoardSingleFields();

        mode.ProcessThrows([new DartThrow(number, multiplier)]);

        mode.Score.Should().Be(0);
    }

    [Fact]
    public void ThirdDart_CompletesTurnWithoutAdvancingDisplay()
    {
        var mode = new RoundTheBoardSingleFields();

        var turnCompleted = mode.ProcessThrows([new DartThrow(1, 1), new DartThrow(1, 3), new DartThrow(5, 1)]);

        turnCompleted.Should().BeTrue();
        mode.PrimaryDisplayValue.Should().Be("1");
        mode.CurrentTurnThrows.Should().HaveCount(3);
    }

    [Fact]
    public void ProcessThrows_NoOpsWhileTurnPendingAdvance()
    {
        var mode = new RoundTheBoardSingleFields();
        mode.ProcessThrows([new DartThrow(1, 1), new DartThrow(1, 1), new DartThrow(1, 1)]);

        var turnCompleted = mode.ProcessThrows([new DartThrow(1, 1), new DartThrow(1, 1), new DartThrow(1, 1), new DartThrow(1, 1)]);

        turnCompleted.Should().BeFalse();
        mode.Score.Should().Be(3);
    }

    [Fact]
    public void AdvanceToNextTurn_ClearsThrowsAndAdvancesTarget()
    {
        var mode = new RoundTheBoardSingleFields();
        mode.ProcessThrows([new DartThrow(1, 1), new DartThrow(1, 1), new DartThrow(1, 1)]);

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
            var turnCompleted = mode.ProcessThrows([new DartThrow(target, 1), new DartThrow(target, 1), new DartThrow(target, 1)]);
            turnCompleted.Should().BeTrue();
            mode.AdvanceToNextTurn();
        }

        mode.IsComplete.Should().BeTrue();
        mode.Score.Should().Be(60);
    }

    [Fact]
    public void ProcessThrows_NoOpsOnceComplete()
    {
        var mode = new RoundTheBoardSingleFields();
        for (var target = 1; target <= 20; target++)
        {
            mode.ProcessThrows([new DartThrow(target, 1), new DartThrow(target, 1), new DartThrow(target, 1)]);
            mode.AdvanceToNextTurn();
        }

        mode.ProcessThrows([new DartThrow(1, 1)]);

        mode.Score.Should().Be(60);
    }

    [Fact]
    public void BoardResetMidTurn_ResyncsDisplayedThrows()
    {
        var mode = new RoundTheBoardSingleFields();
        mode.ProcessThrows([new DartThrow(1, 1), new DartThrow(1, 1)]);
        mode.Score.Should().Be(2);

        // Board reports a shorter throws array than previously seen (e.g. darts removed / turn reset).
        // The displayed throw list resyncs to the shorter list; darts replayed from index 0 are re-scored
        // (a pre-existing characteristic of this reset-detection approach, not something this change alters).
        mode.ProcessThrows([new DartThrow(1, 1)]);

        mode.Score.Should().Be(3);
        mode.CurrentTurnThrows.Should().ContainSingle();
    }
}
