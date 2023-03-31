using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class MissionsEntity
    {
        public MissionsEntity(string template, int difficulty, bool whiteList, bool choose)
        {
            Template = template;
            Difficulty = difficulty;
            WhiteList = whiteList;
            Choose = choose;

        }

        public MissionsEntity()
        {
        }

        public string Template { get; set; }
        public int Difficulty { get; set; }
        public bool WhiteList { get; set; }
        public bool Choose { get; set; }

    }
}
