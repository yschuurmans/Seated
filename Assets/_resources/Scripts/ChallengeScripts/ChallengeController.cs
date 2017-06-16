using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets._resources.Scripts.ChallengeScripts
{
    public class ChallengeController : MonoBehaviour
    {
        public static ChallengeController Instance;

        public Challenge Challenge;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this.gameObject);
            }
            Challenge = GetComponentInChildren<Challenge>();
        }
    }
}
