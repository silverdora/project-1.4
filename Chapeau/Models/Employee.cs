using System;
using Chapeau.Enumerations;

namespace Chapeau.Models
{
	public class Employee
	{
        public int employeeID { get; set; } 
        public string employeeName { get; set; }
        public Role Role { get; set; }
        public Employee()
		{
		}

        public Employee(int employeeID, string employeeName, Role role)
        {
            this.employeeID = employeeID;
            this.employeeName = employeeName;
            Role = role;
        }
    }
}

