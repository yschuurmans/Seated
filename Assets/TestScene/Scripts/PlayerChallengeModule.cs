﻿using Assets.TestScene.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes;
using Assets.TestScene.Scripts.HelperClasses;
using UnityEngine;

public class PlayerChallengeModule : MonoBehaviour
{
    public DeltaFlyer DeltaFlyer;

    //Completed challenges with fastest achieved time
    public Dictionary<Challenge, float> CompletedChallengeResults = new Dictionary<Challenge, float>();

    //Current Challenge Variables
    public float StartTime;
    private Location _currentTargetLocation;
    private Challenge _activeChallenge;

    //EventHandlers
    public delegate void PlayerCompletedChallenge(PlayerChallengeModule challengeModule);
    public PlayerCompletedChallenge OnPlayerCompletedChallenge = (challengeModule) => {};

    public Location CurrentTargetLocation
    {
        get
        {
            return _currentTargetLocation;
        }
        set
        {
            //Guard condition
            if (_currentTargetLocation == value)
                return;

            //Hide old target location
            if (_currentTargetLocation != null)
                _currentTargetLocation.gameObject.layer = LayerMask.NameToLayer("InvisibleLocation");

            //Assign new location as current
            _currentTargetLocation = value;

            //Show new target location
            if (_currentTargetLocation != null)
                _currentTargetLocation.gameObject.layer = LayerMask.NameToLayer("VisibleLocation");
        }
    }

    //Challenge player is currently participating in
    public Challenge ActiveChallenge
    {
        get
        {
            return _activeChallenge;
        }
        set
        {
            //Guard condition
            if (_activeChallenge == value)
                return;

            //Make challenge start triggers visible to camera depending on if player is currently in a challenge
            if (value == null)
            {
                CameraCullingMaskHelper.ShowLayer("ChallengeStart");
                Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("ChallengeStart");
            }
            else
            {
                CameraCullingMaskHelper.HideLayer("ChallengeStart");
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("ChallengeStart"));
            }

            _activeChallenge = value;
        }
    }

    public void StartChallenge(Challenge challenge)
    {
        ActiveChallenge = challenge;
        StartTime = Time.time;
        
        //Entered starting location
        OnEnterLocation(challenge.LocationsInOrder.First().Value);
    }

    public void OnEnterLocation(Location enteredLocation)
    {
        if (ActiveChallenge == null || CurrentTargetLocation != null && enteredLocation != CurrentTargetLocation) return;
        if (enteredLocation.Type == Location.LocationType.Finish)
        {
            OnChallengeCompleted();
            CurrentTargetLocation = null;
            return;
        }
        CurrentTargetLocation = ActiveChallenge.LocationsInOrder.Values.FirstOrDefault(l=>l.SequenceIndex == enteredLocation.SequenceIndex + 1);
    }

    public void OnChallengeCompleted()
    {
        float elapsedTime = Time.time - StartTime;

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
