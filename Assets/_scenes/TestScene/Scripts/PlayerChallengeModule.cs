using Assets.TestScene.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TestScene.Scripts.HelperClasses;
using Assets._resources.Scripts.ChallengeScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChallengeModule : MonoBehaviour
{
    public DeltaFlyer DeltaFlyer;

    //Completed challenges with fastest achieved time
    public Dictionary<Challenge, float> CompletedChallengeResults = new Dictionary<Challenge, float>();

    public Text TextField;

    //Current Challenge Variables
    public float StartTime;
    private Location _currentTargetLocation;
    private Challenge _activeChallenge;

    //EventHandlers
    public delegate void PlayerCompletedChallenge(PlayerChallengeModule challengeModule);
    public PlayerCompletedChallenge OnPlayerCompletedChallenge = (challengeModule) => { };

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

            if (_currentTargetLocation != null)
                _currentTargetLocation.SetIsTargetLocation(null);
            //Assign new location as current
            _currentTargetLocation = value;

            if (_currentTargetLocation != null)
                _currentTargetLocation.SetIsTargetLocation(this);
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
                ToggleStartLocationVisibility(true);
            }
            else
            {
                ToggleStartLocationVisibility(false);
                //Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("ChallengeStart"));
            }

            _activeChallenge = value;
        }
    }

    void Awake()
    {
        DeltaFlyer = GetComponent<DeltaFlyer>();
    }

    void Start()
    {
        ToggleStartLocationVisibility(true);
    }

    void Update()
    {
        if (ActiveChallenge != null && TextField != null)
        {
            TextField.text = "Time: \t" + (Time.time - StartTime);
        }
    }
    private void ToggleStartLocationVisibility(bool setVisible)
    {
        if(setVisible)
            CameraCullingMaskHelper.ShowLayer("StartLocation");
        else
            CameraCullingMaskHelper.HideLayer("StartLocation");

        //CurrentTargetLocation = ChallengeController.Instance.Challenge.LocationsInOrder.First(
        //            loc => loc.Value.Type == Location.LocationType.Start).Value;
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

        DeltaFlyer.spawnPoint = enteredLocation.transform;
        //Transform spawn = DeltaFlyer.spawnPoint;
        //spawn.transform.position = enteredLocation.transform.position;
        //spawn.transform.LookAt(CurrentTargetLocation.transform);
        //DeltaFlyer.spawnPoint = spawn;
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
        Challenge.ChallengeMedal medal = ActiveChallenge.GetAchievedMedal(elapsedTime);

        if (TextField != null)
        {
            TextField.text = medal == Challenge.ChallengeMedal.None ? "You were too slow! Better luck next time!" : "Congratulations, you received a " + medal + " medal with a time of " + elapsedTime + " seconds!";
            Debug.Log("Player achieved a " + medal + " medal!");
        }
        ActiveChallenge = null;
    }
}