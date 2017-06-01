using UnityEngine;

public class RescueChallenge : Challenge{
    private Vector3 _endLocation;

    public RescueChallenge(string name, string description, Vector3 startLocation, Vector3 location, Vector3 endLocation) : base(name, description, startLocation, location)
    {
        _endLocation = endLocation;
    }

    public Vector3 GetEndLocation()
    {
        return _endLocation;
    }
}
