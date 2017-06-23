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
        private Vector3 debugWorldPos;
        float screenMargin = 50;
        Rect screenRect;


        //gizmosvars

        Vector3 objPosScreenPos;
        Vector3 gizNewPos;
        Vector3 forwardVec;

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
            else
            {
                WorldWaypointMarker.SetActive(false);
                ScreenWaypointMarker.SetActive(false);
            }
            WorldWaypointMarker.SetActive(ShowMarker);
        }

        private IEnumerator UpdateMarkerScreenPosition(PlayerChallengeModule player)
        {
            while (ShowMarker)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                debugWorldPos = pos;
                if (screenRect.Contains(pos) && pos.z >= 0)
                {
                    //waypoint in the world
                    WorldWaypointMarker.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 15, 0));
                   

                    if (!WorldWaypointMarker.activeSelf) WorldWaypointMarker.SetActive(true);
                    if (ScreenWaypointMarker.activeSelf) ScreenWaypointMarker.SetActive(false);
                }
                else
                {
                    //waypoint on screen
                    ScreenWaypointMarker.transform.position = GetMarkerPosition(pos);
                    if (!ScreenWaypointMarker.activeSelf) ScreenWaypointMarker.SetActive(true);
                    if (WorldWaypointMarker.activeSelf) WorldWaypointMarker.SetActive(false);
                }
                yield return null;
            }
        }

        private Vector3 GetMarkerPosition(Vector3 objPos)
        {
            Vector2 objPos2d = objPos;
            Vector2 forward = (screenRect.center - objPos2d).normalized;

            if (objPos.z > 0)
            {
                forward *= -1;
            }

            Vector2 newPos = screenRect.center;
            while (screenRect.Contains(newPos))
            {
                newPos += forward;
            }

            objPos = newPos;

            gizNewPos = newPos;
            objPosScreenPos = Camera.main.WorldToScreenPoint(objPos);
            forwardVec = forward;

            Bounds bounds = new Bounds(screenRect.center, screenRect.size);
            Vector3 point = bounds.ClosestPoint(objPos);
            return point;
        }


        void dingen()
        {
            Vector3 noClampPosition = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 clampedPosition = new Vector3(Mathf.Clamp(noClampPosition.x, 0 + screenMargin, Screen.width - screenMargin),
                                                                    Mathf.Clamp(noClampPosition.y, 0 + screenMargin, Screen.height - screenMargin),
                                                                      noClampPosition.z);

            //myRectTransform.position = clampToScreen ? clampedPosition : noClampPosition;
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
            Gizmos.color = Color.red;
            Gizmos.DrawLine(screenRect.center, debugWorldPos);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(objPosScreenPos, 100);


            Gizmos.color = Color.black;
            Gizmos.DrawSphere(gizNewPos, 100);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(screenRect.center, forwardVec * 100);
        }

        public enum LocationType
        {
            Start,
            Checkpoint,
            Finish
        }
    }
}
