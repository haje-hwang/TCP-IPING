using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Linq;

public class UserList : ISingleton<UserList>
{
    //Concurrent: thread safe type, since  .NET Framework 4.6
    private ConcurrentDictionary<uint, User> userMap;
    public UserList()
    {
        userMap = new ConcurrentDictionary<uint, User>();
    }
    // 새 유저가 추가될 때
    public User CreateNewUser()
    {
        UID id = UID.NewUID();
        while(userMap.ContainsKey(id)) { id = UID.NewUID(); }   //userID중복 방지
        
        User newUser = new User(id, "Anonymous");
        userMap.TryAdd(newUser.id, newUser);
        return newUser;
    }
    public void UpdateUser(User user)
    {
        userMap.AddOrUpdate(user.id, user, (key, existingVal) => user);
    }
    public User ReadUser(uint id)
    {
        return userMap[id];
    }
    public bool DeleteUser(uint id)
    {
        return userMap.TryRemove(id, out User RemovedUser);
    }
}
