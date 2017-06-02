using Assets.Classes;
using UnityEngine;

public class RescueChallenge : Challenge{
    private Location _endLocation;

    public RescueChallenge(string name, string description, Location startLocation, Location location, Location endLocation) : base(name, description, startLocation, location)
    {
        _endLocation = endLocation;
    }

    public Location GetEndLocation()
    {
        return _endLocation;
        
    }
}
