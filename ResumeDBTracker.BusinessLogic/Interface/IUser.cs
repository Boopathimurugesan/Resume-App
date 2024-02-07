using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Core.Models;

namespace ResumeDBTracker.Business.Interface
{
    public interface IUser
    {
        User login(string username, string password);
        List<User> UserList(string email);
        UserResponse UpdateUser(User user);
        UserResponse InsertUser(User user);
        UserResponse DeleteUser(string userid);
    }
}
