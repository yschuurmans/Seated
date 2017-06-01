using UnityEngine;

public class TimeChallenge : Challenge {
    /// <summary>
    /// time of the challenge in seconds
    /// </summary>
    private int _time;

    public TimeChallenge(string name, string description, Vector3 startLocation, Vector3 location, int time) : base(name, description, startLocation, location)
    {
        _time = time;
    }

    public int GetTime()
    {
        return _time;
    }

}
