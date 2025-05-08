using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeModule.Entities
{
    public class EmployeeCreate_ResultEntities
    {
        public int EmployeeAccountId { get; set; }
        public int IsNewUser { get; set; }
        public int NeedSetPassword { get; set; }
        public int NeedSetCompany { get; set; }
    }
}
