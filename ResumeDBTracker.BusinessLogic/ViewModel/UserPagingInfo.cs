using ResumeDBTracker.Core.Models;
using X.PagedList;

namespace ResumeDBTracker.Business.ViewModel
{
    public class UserPagingInfo
    {
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public List<User>? UserList { get; set; }
    }
}