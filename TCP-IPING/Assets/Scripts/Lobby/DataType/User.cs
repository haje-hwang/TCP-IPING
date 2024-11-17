using System;

public class User
{
    public Guid id {get; private set;}
    public string nickName;
    public User(Guid uid, string name)
    {
        id = uid;
        nickName = name;
    }
    public User Empty()
    {
        return new User(Guid.Empty, "Anonnymous");
    }
}
