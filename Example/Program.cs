using System;
using System.Collections.Generic;
using System.Text;
using SequelMaxNet;

namespace Example
{

    class Employee 
    {
	    public int EmployeeID;
	    public int SupervisorID;
	    public string Name;
	    public string Gender;
	    public double Salary;
	    public string Comment;
    };

    class Program
    {
        static string GetFolderPath()
        {
            return @"..\..\..\";
        }

        static void Main(string[] args)
        {
            List<Employee> list = new List<Employee>();
            if (ReadDoc(GetFolderPath() + "emp.xml", list))
                DisplayDoc(list);
            else
                Console.WriteLine("Cannot read file!");
        }

        static bool ReadDoc(string file, List<Employee> list)
        {
            SequelMaxNet.Document doc = new SequelMaxNet.Document();

            doc.RegisterStartElementDelegate("Employees|Employee", (elem) =>
            {
                Employee emp = new Employee();
                emp.EmployeeID = elem.Attr("EmployeeID").GetInt32(0);
                emp.SupervisorID = elem.Attr("SupervisorID").GetInt32(0);
                list.Add(emp);
            });
            doc.RegisterEndElementDelegate("Employees|Employee|Name", (text) =>
            {
                list[list.Count - 1].Name = text;
            });
            doc.RegisterEndElementDelegate("Employees|Employee|Gender", (text) =>
            {
                list[list.Count - 1].Gender = text;
            });
            doc.RegisterEndElementDelegate("Employees|Employee|Salary", (text) =>
            {
                Double.TryParse(text, out list[list.Count - 1].Salary);
            });
            doc.RegisterCommentDelegate("Employees|Employee", (text) =>
            {
                list[list.Count - 1].Comment = text;
            });

            return doc.Open(file);
        }

        static void DisplayDoc(List<Employee> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Console.WriteLine("Name: {0}", list[i].Name);
                Console.WriteLine("EmployeeID: {0}", list[i].EmployeeID);
                Console.WriteLine("SupervisorID: {0}", list[i].SupervisorID);
                Console.WriteLine("Gender: {0}", list[i].Gender);
                Console.WriteLine("Salary: {0}", list[i].Salary);

                if (string.IsNullOrEmpty(list[i].Comment) == false)
                    Console.WriteLine("Comment: {0}", list[i].Comment);

                Console.WriteLine();
            }
        }
    }
}
