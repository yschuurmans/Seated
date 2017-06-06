using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    /// <summary>
    /// name of the challenge
    /// </summary>
    public string name;
    /// <summary>
    /// description of the challenge 
    /// </summary>
    public string description;
    /// <summary>
    /// List of IDs of participating players
    /// </summary>
    protected List<Participant> _participants;

    /// <summary>
    /// Locations for the challenge with sequence index as key, e.g. 1: Start, 2: Checkpoint1, 3: Finish
    /// </summary>
    [SerializeField]
    protected Dictionary<int, Location> LocationsInOrder;

    void Awake()
    {
        LocationsInOrder = GetComponentsInChildren<Location>().OrderBy(l=>l.SequenceIndex).ToDictionary(l=>l.SequenceIndex);
    }

    protected virtual void Start()
    {
        if (LocationsInOrder != null)
        {
            //LocationsInOrder.ForEach(l=>l.OnPlayerEntered+=OnPlayerEnteredLocation);
        }
        
    }

    private void OnPlayerEnteredLocation(int playerID, Location location)
    {
        //Is player participating in the challenge?
        if (_participants.All(p => p.PlayerId != playerID))
            return;

        Participant participant = _participants.First(p => p.PlayerId == playerID);

        //Is this the current goal of the player?
        if (participant.CurrentGoal != location)
            return;

        participant.LastVisitedLocation = participant.CurrentGoal;
        //participant.CurrentGoal = Locations.

    }

    public void CompleteChallenge(int playerId)
    {
        foreach (Participant p in _participants)
        {
            if (p.PlayerId == playerId)
            {
                p.CompletedChallenges.Add(this);
            }
        }
    }

    public struct Participant
    {
        public int PlayerId;
        public float TimeElapsed;
        public Location LastVisitedLocation;
        public Location CurrentGoal;
        public List<Challenge> CompletedChallenges;

    }
}

