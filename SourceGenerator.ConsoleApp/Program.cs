using SourceGenerator.ConsoleApp.Model;

Console.WriteLine("Hello, World!");

var person = new Person()
{
    FirstName = "Zeynel",

    LastName = "Şahin"
};

var personAsString = person.ToString();
Console.WriteLine(personAsString);
Console.ReadLine();