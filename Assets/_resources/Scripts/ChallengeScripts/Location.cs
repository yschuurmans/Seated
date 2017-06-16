using System.Collections;
using UnityEngine;

namespace Assets._resources.Scripts.ChallengeScripts
{
    public class Location : MonoBehaviour
    {
        public delegate void PlayerEnteredEventHandler(PlayerChallengeModule challengeModule, Location location);
        public PlayerEnteredEventHandler OnPlayerEntered = (challengeModule, location) => { };

        public LocationType Type;
        public int SequenceIndex;
        public GameObject ScreenWaypointMarker;
        public GameObject WorldWaypointMarker;
        private bool ShowMarker = false;

        private Collider trigger;
        private bool _showMarker;

        float screenMargin = 50;
        Rect screenRect;

        void Awake()
        {
            screenRect = new Rect(screenMargin, screenMargin, Screen.width - (screenMargin * 2), Screen.height - (screenMargin * 2));
            trigger = GetComponent<Collider>();
            SequenceIndex = transform.GetSiblingIndex();
        }

        public void SetIsTargetLocation(PlayerChallengeModule player)
        {
            SetShowMarkerForPlayer(player);

            if (Type != LocationType.Start)
            {
                gameObject.layer = ShowMarker ? LayerMask.NameToLayer("VisibleLocation") : LayerMask.NameToLayer("InvisibleLocation");
            }

        }

        public void SetShowMarkerForPlayer(PlayerChallengeModule player)
        {
            if (ShowMarker == (player != null))
                return;

            ShowMarker = player != null;

            if (ShowMarker)
                StartCoroutine(UpdateMarkerScreenPosition(player));

            WorldWaypointMarker.SetActive(ShowMarker);
        }

        private IEnumerator UpdateMarkerScreenPosition(PlayerChallengeModule player)
        {
            while (ShowMarker)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0, 0));
                if (screenRect.Contains(pos))
                {
                    if (pos.z >= 0)
                    {
                        WorldWaypointMarker.transform.position = transform.position;
                        WorldWaypointMarker.transform.LookAt(player.transform);
                    }

                    if (!WorldWaypointMarker.activeSelf) WorldWaypointMarker.SetActive(true);
                    if (ScreenWaypointMarker.activeSelf) ScreenWaypointMarker.SetActive(false);
                }
                else
                {

                    ScreenWaypointMarker.transform.position = GetMarkerPosition();
                    if (!ScreenWaypointMarker.activeSelf) ScreenWaypointMarker.SetActive(true);
                    if (WorldWaypointMarker.activeSelf) WorldWaypointMarker.SetActive(false);
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
            if (!Application.isPlaying) return;
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
