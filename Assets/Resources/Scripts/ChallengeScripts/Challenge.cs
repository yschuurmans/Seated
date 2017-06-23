using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._resources.Scripts.ChallengeScripts
{
    public abstract class Challenge : MonoBehaviour
    {
        public enum ChallengeMedal
        {
            None,
            Bronze,
            Silver,
            Gold
        }

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
        public Dictionary<PlayerChallengeModule, bool> ParticipantStatus;

        /// <summary>
        /// Locations for the challenge with sequence index as key, e.g. 1: Start, 2: Checkpoint1, 3: Finish
        /// </summary>
        [SerializeField]
        public Dictionary<int, Location> LocationsInOrder;

        public Dictionary<ChallengeMedal, float> MedalRequirements = new Dictionary<ChallengeMedal, float>();

        public float BronzeMedalTime;
        public float SilverMedalTime;
        public float GoldMedalTime;

        public bool IsRunning;
        public float StartTime;
        public int ParticipantsRequired;
        public bool DebugStartChallenge;

        void Awake()
        {
            ParticipantStatus = new Dictionary<PlayerChallengeModule, bool>();
            MedalRequirements = new Dictionary<ChallengeMedal, float>();
        }

        protected virtual void Start()
        {
            LocationsInOrder = GetComponentsInChildren<Location>().OrderBy(l => l.SequenceIndex).ToDictionary(l => l.SequenceIndex);
            if (LocationsInOrder != null)
            {
                LocationsInOrder.Values.ToList().ForEach(l => l.OnPlayerEntered += OnPlayerEnteredLocation);
            }

            MedalRequirements.Add(ChallengeMedal.Gold, GoldMedalTime);
            MedalRequirements.Add(ChallengeMedal.Silver, SilverMedalTime);
            MedalRequirements.Add(ChallengeMedal.Bronze, BronzeMedalTime);
            MedalRequirements.Add(ChallengeMedal.None, 10000000);
        }

        void Update()
        {
            if (DebugStartChallenge && !IsRunning)
            {
                StartChallenge();
                DebugStartChallenge = false;
            }
        }

        public void SubmitParticipant(PlayerChallengeModule participant)
        {
            ParticipantStatus.Add(participant, false);
            if (ParticipantStatus.Count >= ParticipantsRequired)
                StartChallenge();
            Debug.Log("Player " + participant.DeltaFlyer.raptor.ID + " is now participating in a challenge");
        }

        public void StartChallenge()
        {
            StartTime = Time.time;
            IsRunning = true;
            ParticipantStatus.Keys.ToList().ForEach(p =>
            {
                p.OnPlayerCompletedChallenge += OnPlayerCompletedChallenge;
                p.StartChallenge(this);
            });
        }

        public void OnPlayerEnteredLocation(PlayerChallengeModule participant, Location location)
        {
            if (!IsRunning)
            {
                //Is player participating in the challenge?
                if (!ParticipantStatus.ContainsKey(participant))
                {
                    SubmitParticipant(participant);
                }
            }
            else
            {
                participant.OnEnterLocation(location);
            }
        }

        public void OnPlayerCompletedChallenge(PlayerChallengeModule participant)
        {
            //Is player participating in the challenge?
            if (!ParticipantStatus.ContainsKey(participant))
                return;

            //Unsubscribe from event
            participant.OnPlayerCompletedChallenge -= OnPlayerCompletedChallenge;

            Debug.Log("Player " + participant.DeltaFlyer.raptor.ID + " | completed the challenge in " + (Time.time - StartTime) + " seconds");

            //Set finished bool
            ParticipantStatus[participant] = true;

            //Have all participants finished the challenge?
            if (ParticipantStatus.All(p => p.Value))
            {
                FinalizeChallenge();
            }
        }

        public ChallengeMedal GetAchievedMedal(float time)
        {
            return MedalRequirements.OrderBy(pair => pair.Value).FirstOrDefault(pair => time < pair.Value).Key;
        }

        private void FinalizeChallenge()
        {
            Debug.Log("Challenge Finished");
            IsRunning = false;

            var enumerator = ParticipantStatus.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                    enumerator.Current.ActiveChallenge = null;
            }
            enumerator.Dispose();

            ParticipantStatus = new Dictionary<PlayerChallengeModule, bool>();
        }
    }
}

