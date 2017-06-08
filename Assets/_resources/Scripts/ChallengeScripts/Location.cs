using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{
    public class Location : MonoBehaviour
    {
        public delegate void PlayerEnteredEventHandler(PlayerChallengeModule challengeModule, Location location);
        public PlayerEnteredEventHandler OnPlayerEntered = (challengeModule, location)=> {};

        public LocationType Type;
        public int SequenceIndex;

        private Collider trigger;

        void Awake()
        {
            trigger = GetComponent<Collider>();
        }


        void OnTriggerEnter(Collider other)
        {
            PlayerChallengeModule challengeModule = other.GetComponent<PlayerChallengeModule>();
            if (challengeModule == null) return;
            OnPlayerEntered(challengeModule, this);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawSphere(trigger.bounds.center, trigger.bounds.size.x);
        }

        public enum LocationType
        {
            Start,
            Checkpoint,
            Finish
        }
    }
}
