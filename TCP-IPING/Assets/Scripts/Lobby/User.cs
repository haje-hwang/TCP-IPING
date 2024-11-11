using System;

public class User
{
    public uint id {get; private set;}
    public string nickName;
    public User(uint uid, string name)
    {
        id = uid;
        nickName = name;
    }
}
