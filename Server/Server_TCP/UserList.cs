using System.Collections.Concurrent;

public class UserList
{
    //Concurrent: thread safe type, since  .NET Framework 4.6
    private ConcurrentDictionary<Guid, User> userMap;
    public UserList()
    {
        userMap = new ConcurrentDictionary<Guid, User>();
    }
    // 새 유저가 추가될 때
    public User CreateNewUser()
    {
        Guid id = Guid.NewGuid();
        while (userMap.ContainsKey(id)) { id = Guid.NewGuid(); }   //userID중복 방지

        User newUser = new User(id, "Anonymous");
        userMap.TryAdd(newUser.id, newUser);
        return newUser;
    }
    public void UpdateUser(User user)
    {
        userMap.AddOrUpdate(user.id, user, (key, existingVal) => user);
    }
    public User ReadUser(Guid id)
    {
        return userMap[id];
    }
    public bool DeleteUser(Guid id)
    {
        return userMap.TryRemove(id, out User RemovedUser);
    }
}