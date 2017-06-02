using Assets.Classes;
using UnityEngine;

public class TimeChallenge : Challenge {
    /// <summary>
    /// time of the challenge in seconds
    /// </summary>
    private int _time;

    public TimeChallenge(string name, string description, Location startLocation, Location location, int time) : base(name, description, startLocation, location)
    {
        _time = time;
    }

    public int GetTime()
    {
        return _time;
    }

}
