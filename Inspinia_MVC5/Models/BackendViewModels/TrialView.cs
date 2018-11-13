using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enact.Models.BackendViewModels
{
    public class TrialView
    {
        public Trial trial { get; set; }
        public Allocation allocation { get; set; }
        public Audit audit { get; set; }
        public TrialRun run { get; set; }
    }
}