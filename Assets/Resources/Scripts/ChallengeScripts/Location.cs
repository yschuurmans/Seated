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
        public PlayerEnteredEventHandler OnPlayerEntered = (challengeModule, location) => { };

        public LocationType Type;
        public int SequenceIndex;
        public GameObject WaypointMarker;

        private Collider trigger;
        private bool _showMarker;

        float screenMargin = 50;
        Rect screenRect;

        public bool IsTargetLocation
        {
            set
            {
                ShowMarker = value;

                if (Type != LocationType.Start)
                {
                    gameObject.layer = value ? LayerMask.NameToLayer("VisibleLocation") : LayerMask.NameToLayer("InvisibleLocation");
                }
            }
        }

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

                _showMarker = value;

                if (value)
                    StartCoroutine(UpdateMarkerScreenPosition());
            }
        }

        void Awake()
        {
            screenRect = new Rect(screenMargin, screenMargin, Screen.width - (screenMargin * 2), Screen.height - (screenMargin * 2));
            trigger = GetComponent<Collider>();
        }

        private IEnumerator UpdateMarkerScreenPosition()
        {
            while (ShowMarker)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 4, 0));
                if (screenRect.Contains(pos))
                {
                    if (pos.z >= 0)
                    {
                        WaypointMarker.transform.position = pos;
                    }
                }
                else
                {
                    WaypointMarker.transform.position = GetMarkerPosition();
                }
                yield return null;
            }
        }

        private Vector3 GetMarkerPosition()
        {
            Bounds bounds = new Bounds(screenRect.center, screenRect.size);
            return bounds.ClosestPoint(Camera.main.WorldToScreenPoint(transform.position));
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
