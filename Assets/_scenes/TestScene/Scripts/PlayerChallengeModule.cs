using Assets.TestScene.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes;
using UnityEngine;

public class PlayerChallengeModule : MonoBehaviour
{
    public DeltaFlyer DeltaFlyer;
    public Dictionary<Challenge, float> CompletedChallengeResults;
    public Challenge ActiveChallenge;
    public Location CurrentTargetLocation;

    public delegate void PlayerCompletedChallenge(PlayerChallengeModule challengeModule);
    public PlayerCompletedChallenge OnPlayerCompletedChallenge = (challengeModule) => {};

    public IEnumerator StartChallenge(Challenge challenge)
    {
        ActiveChallenge = challenge;
        float startTime = Time.time;

        OnEnterLocation(challenge.LocationsInOrder.First().Value);
        foreach (var locationEntry in ActiveChallenge.LocationsInOrder)
        {
            if (locationEntry.Value.Type == Location.LocationType.Finish) break;
            yield return new WaitUntil(()=> locationEntry.Value.SequenceIndex != CurrentTargetLocation.SequenceIndex);
        }
        float elapsedTime = Time.time - startTime;
        OnChallengeCompleted(elapsedTime);
    }

    public void OnEnterLocation(Location enteredLocation)
    {
        if (CurrentTargetLocation != null && enteredLocation != CurrentTargetLocation) return;
        CurrentTargetLocation = ActiveChallenge.LocationsInOrder.Values.FirstOrDefault(l=>l.SequenceIndex == enteredLocation.SequenceIndex + 1);
    }

    public void OnChallengeCompleted(float elapsedTime)
    {
        if (CompletedChallengeResults.ContainsKey(ActiveChallenge) &&
            CompletedChallengeResults[ActiveChallenge] > elapsedTime)
        {
            CompletedChallengeResults[ActiveChallenge] = elapsedTime;
        }
        else
        {
            CompletedChallengeResults.Add(ActiveChallenge, elapsedTime);
        }
        OnPlayerCompletedChallenge(this);
        ActiveChallenge = null;
    }
}
