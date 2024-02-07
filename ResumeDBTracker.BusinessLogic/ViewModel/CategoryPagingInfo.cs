using ResumeDBTracker.Core.Models;
using X.PagedList;

namespace ResumeDBTracker.Business.ViewModel
{
    public class CategoryPagingInfo
    {
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public List<Category>? CategoryList { get; set; }
    }
} 