using System;
using System.Collections;
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
        public bool IsVisible;
        public GameObject WaypointMarker;

        private Collider trigger;
        private bool _showMarker;

        public bool ShowMarker
        {
            get
            {
                return _showMarker;
            }
            set
            {
                if (_showMarker == value)
                    return;

                if (value)
                    StartCoroutine(UpdateMarkerScreenPosition());

                

                _showMarker = value;
            }
        }

        void Awake()
        {
            trigger = GetComponent<Collider>();
        }

        private IEnumerator UpdateMarkerScreenPosition()
        {
            while (ShowMarker)
            {
                if (IsVisible)
                {

                }
                else
                {
                    
                }
                WaypointMarker.transform.position = Camera.main.WorldToScreenPoint(GetMarkerPosition());
                yield return null;
            }
        }

        private Vector3 GetMarkerPosition()
        {
            Camera.main.ViewportToWorldPoint(Vector3.zero);
            Bounds bounds = new Bounds(Camera.main.);
            Camera.main.
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

        private void OnBecameVisible()
        {
            if (Type == LocationType.Start)
                return;

            IsVisible = true;
        }

        private void OnBecameInvisible()
        {
            if (Type == LocationType.Start)
                return;

            IsVisible = false;
        }


        public enum LocationType
        {
            Start,
            Checkpoint,
            Finish
        }
    }
}
