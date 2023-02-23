namespace SourceGenerator.ConsoleApp.Model
{
    public class Person
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        //public override string ToString()
        //{
        //    var stringBuilder = new StringBuilder();
        //    var first = true;
        //    foreach (var propertyInfo in GetType().GetProperties())
        //    {
        //        if (first)
        //        {
        //            first = false;
        //        }
        //        else
        //        {
        //            stringBuilder.Append("; ");
        //        }
        //        var propertyName = propertyInfo.Name;
        //        var propertyValue = propertyInfo.GetValue(this);
        //        stringBuilder.Append($"{propertyName}:{propertyName}");
        //    }
        //    return stringBuilder.ToString();
        //}
    }
}
