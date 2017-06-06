using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{
    public class Location : MonoBehaviour
    {
        public delegate void PlayerEnteredEventHandler(int id, Location location);
        public PlayerEnteredEventHandler OnPlayerEntered = (id, location)=> {};

        public LocationType Type;
        public int SequenceIndex;

        void OnTriggerEnter(Collider other)
        {
            OnPlayerEntered(other.GetComponent<DeltaFlyer>().raptor.ID, this);
        }

        public enum LocationType
        {
            Start,
            Checkpoint,
            Finish
        }
    }
}
