using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteIt.ViewModels
{
    public class DashboardViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPinned { get; set; }
        public bool IsShared { get; set; }
        public string Url { get; set; }
    }
}
