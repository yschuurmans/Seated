using Assets.Classes;
using UnityEngine;

public class DeliverChallenge : Challenge{
    private string _item;

    public DeliverChallenge(string name, string description, Location startLocation, Location location, string item) : base(name, description, startLocation, location)
    {
        _item = item;
    }

    public string GetItem()
    {
        return _item;
    }
}
