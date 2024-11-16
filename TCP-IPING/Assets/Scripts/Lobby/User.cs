using System;

public class User
{
    public UID id {get; private set;}
    public string nickName;
    public User(UID uid, string name)
    {
        id = uid;
        nickName = name;
    }
    public User Empty()
    {
        return new User(UID.Empty(), "Anonnymous");
    }
}
