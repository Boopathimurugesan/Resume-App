using ResumeDBTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeDBTracker.Business.Interface
{
    public interface ICategory
    {
        List<Category> CategoryList();
    }
}
