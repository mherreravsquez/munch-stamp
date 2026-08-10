using munch_stamp.Models;

namespace munch_stamp.Services;

// Pure business logic: no UI, no file I/O. Just answers questions
// about a card's reward state, so any screen (or future customer
// interface) can ask the same question and get the same answer.
public static class RewardService
{
    public static bool HasEarnedReward(LoyaltyCard card) =>
        card.VisitsCount >= card.VisitsRequired;

    public static int VisitsRemaining(LoyaltyCard card) =>
        Math.Max(0, card.VisitsRequired - card.VisitsCount);
}