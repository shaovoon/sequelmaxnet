# sequelmaxnet
New SAX parsing model for .NET

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Employees>
    <Employee EmployeeID="1286" SupervisorID="666">
        <Name>Amanda Dion</Name>
        <Salary>2200</Salary>
        <Gender>Female</Gender>
        <!--Hardworking employee!-->
    </Employee>
    <Employee EmployeeID="1287" SupervisorID="666">
        <Name>John Smith</Name>
        <Salary>3200</Salary>
        <Gender>Male</Gender>
        <!--Hardly working employee!-->
    </Employee>
    <Employee EmployeeID="1288" SupervisorID="666">
        <Name>Sheldon Cohn</Name>
        <Salary>5600</Salary>
        <Gender>Male</Gender>
    </Employee>
</Employees>
```

To parse the XML above, we need to write our parsing code as below with delegates.

```csharp
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
```

[CodeProject Tutorial](http://www.codeproject.com/Articles/744632/SequelMax-NET)
