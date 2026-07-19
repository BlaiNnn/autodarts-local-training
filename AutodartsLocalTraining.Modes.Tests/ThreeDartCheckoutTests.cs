using FluentAssertions;

namespace AutodartsLocalTraining.Modes.Tests;

public class ThreeDartCheckoutTests
{
    [Fact]
    public void InitialState_TargetsFirstLeftover()
    {
        var mode = new ThreeDartCheckout();

        mode.PrimaryDisplayValue.Should().Be("3");
        mode.Score.Should().Be(0);
        mode.MaxScore.Should().Be(40);
        mode.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void Bust_WhenRemainderGoesNegative()
    {
        var mode = new ThreeDartCheckout();

        var turnCompleted = mode.ProcessThrow(new DartThrow(5, 3)); // T5 = 15, leftover 3 -> -12

        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(0);
        mode.CurrentTurnThrows.Should().ContainSingle()
            .Which.Outcome.Should().Be(ThrowOutcome.Bad);
    }

    [Fact]
    public void Bust_WhenRemainderIsExactlyOne()
    {
        var mode = new ThreeDartCheckout();

        var turnCompleted = mode.ProcessThrow(new DartThrow(2, 1)); // S2 = 2, leftover 3 -> 1

        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(0);
    }

    [Fact]
    public void Bust_WhenReachingZeroWithoutADouble()
    {
        var mode = new ThreeDartCheckout();

        var turnCompleted = mode.ProcessThrow(new DartThrow(1, 3)); // T1 = 3, leftover 3 -> 0 but not a double

        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(0);
        mode.CurrentTurnThrows[0].Outcome.Should().Be(ThrowOutcome.Bad);
    }

    [Fact]
    public void ProcessThrow_NoOpsWhileTurnPendingAdvance()
    {
        var mode = new ThreeDartCheckout();

        // First dart busts (T5=15 > leftover 3); a second dart in the same turn must not be evaluated.
        mode.ProcessThrow(new DartThrow(5, 3));
        var turnCompleted = mode.ProcessThrow(new DartThrow(20, 2));

        turnCompleted.Should().BeFalse();
        mode.CurrentTurnThrows.Should().ContainSingle();
        mode.Score.Should().Be(0);
    }

    [Fact]
    public void Checkout_OnSecondDart_Scores2Points()
    {
        var mode = new ThreeDartCheckout(); // leftover starts at 3; advance to 5 first
        mode.ProcessThrow(new DartThrow(1, 3)); // busts leftover 3 (reaches 0 non-double)
        mode.AdvanceToNextTurn(); // now targeting leftover 5

        mode.ProcessThrow(new DartThrow(1, 1)); // S1 -> remaining 4
        var turnCompleted = mode.ProcessThrow(new DartThrow(2, 2)); // D2 -> remaining 0

        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(2);
        mode.CurrentTurnThrows[^1].Outcome.Should().Be(ThrowOutcome.Good);
    }

    [Fact]
    public void Checkout_OnThirdDart_Scores1Point()
    {
        var mode = new ThreeDartCheckout();
        mode.ProcessThrow(new DartThrow(1, 3)); // busts leftover 3 (reaches 0 non-double)
        mode.AdvanceToNextTurn(); // leftover 5
        mode.ProcessThrow(new DartThrow(5, 3)); // T5 = 15, busts leftover 5 (goes negative)
        mode.AdvanceToNextTurn(); // leftover 7

        mode.ProcessThrow(new DartThrow(3, 1)); // S3 -> remaining 4 (neutral)
        mode.ProcessThrow(new DartThrow(2, 1)); // S2 -> remaining 2 (neutral)
        var turnCompleted = mode.ProcessThrow(new DartThrow(1, 2)); // D1 -> remaining 0 (checkout)

        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(1);
        mode.CurrentTurnThrows[^1].Outcome.Should().Be(ThrowOutcome.Good);
    }

    [Fact]
    public void NoCheckoutAfterThreeDarts_ScoresZero_AndAdvanceResetsLeftoverToNextTarget()
    {
        var mode = new ThreeDartCheckout();
        mode.ProcessThrow(new DartThrow(1, 3)); // leftover 3 -> bust
        mode.AdvanceToNextTurn(); // leftover 5

        mode.ProcessThrow(new DartThrow(1, 1)); // remaining 4 (neutral)
        var stillPlaying = mode.ProcessThrow(new DartThrow(1, 1)); // remaining 3 (neutral)
        mode.AdvanceToNextTurn(); // turn never resolved (only 2 darts); advance is a no-op

        stillPlaying.Should().BeFalse();
        mode.PrimaryDisplayValue.Should().Be("3"); // live leftover reflects progress so far, no-op confirmed

        var turnCompleted = mode.ProcessThrow(new DartThrow(1, 1)); // remaining 2, 3rd dart, no bust/checkout -> 0 points
        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(0);

        mode.AdvanceToNextTurn();

        mode.PrimaryDisplayValue.Should().Be("7"); // leftover reset to the next target, not carried from prior turn
    }

    [Fact]
    public void Bull_AlwaysBusts_SinceItExceedsAnyLeftoverInThisMode()
    {
        var mode = new ThreeDartCheckout();

        var turnCompleted = mode.ProcessThrow(new DartThrow(25, 2)); // double bull = 50, leftover starts at 3 (max target is 41)

        turnCompleted.Should().BeTrue();
        mode.Score.Should().Be(0);
        mode.CurrentTurnThrows[0].Outcome.Should().Be(ThrowOutcome.Bad);
    }

    [Fact]
    public void Miss_IsDisplayedAsMiss_NotAsZero_AndLeavesLeftoverUnchanged()
    {
        var mode = new ThreeDartCheckout();

        var turnCompleted = mode.ProcessThrow(new DartThrow(0, 0));

        turnCompleted.Should().BeFalse();
        mode.PrimaryDisplayValue.Should().Be("3");
        mode.CurrentTurnThrows.Should().ContainSingle()
            .Which.DisplayText.Should().Be("Miss");
    }

    [Fact]
    public void DartValue_ComputesNumberTimesMultiplier()
    {
        new DartThrow(25, 2).Value.Should().Be(50);
        new DartThrow(20, 3).Value.Should().Be(60);
        new DartThrow(1, 1).Value.Should().Be(1);
    }

    [Fact]
    public void FullRun_AlwaysBusting_CompletesAfter20TargetsWithZeroScore()
    {
        var mode = new ThreeDartCheckout();
        var turnsPlayed = 0;

        for (var target = 3; target <= 41; target += 2)
        {
            mode.IsComplete.Should().BeFalse();
            var turnCompleted = mode.ProcessThrow(new DartThrow(20, 3)); // T20 = 60, always exceeds any target here -> bust
            turnCompleted.Should().BeTrue();
            mode.AdvanceToNextTurn();
            turnsPlayed++;
        }

        turnsPlayed.Should().Be(20);
        mode.IsComplete.Should().BeTrue();
        mode.Score.Should().Be(0);
    }

    [Fact]
    public void AdvanceToNextTurn_NoOpsWhenTurnNotCompleted()
    {
        var mode = new ThreeDartCheckout();

        mode.AdvanceToNextTurn();

        mode.PrimaryDisplayValue.Should().Be("3");
    }
}
