using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recruitment.Models
{
    public class CandidateListModel
    {
        public List<string> Candidates { get; set; }
        public string Password { get; set; }
    }
}