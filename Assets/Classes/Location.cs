using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{
    public class Location : MonoBehaviour
    {
        public delegate void PlayerEnteredEventHandler();
        public PlayerEnteredEventHandler OnPlayerEntered = ()=> {}; 

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {

            }
        }
    }
}
