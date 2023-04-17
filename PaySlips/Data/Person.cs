using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payslips.Data
{
    class Person
    {
        public Person(string fullName, string email)
        {
            FullName = fullName;
            Email = email;
        }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
