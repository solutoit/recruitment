using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Recruitment.DataAccess;

namespace Recruitment.Models
{
    public class CandidateModel
    {
        public string Name { get; set; }
        public List<UserAction> Actions { get; set; }
        public string Solution { get; set; }
    }
}