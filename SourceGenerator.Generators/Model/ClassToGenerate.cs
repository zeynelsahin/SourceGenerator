using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerator.Generators.Model
{
    internal class ClassToGenerate : IEquatable<ClassToGenerate?>
    {
        public ClassToGenerate(string nameSpaceName, string className,
            IEnumerable<string> propertyNames)
        {
            NameSpaceName = nameSpaceName;
            ClassName = className;
            PropertyNames = propertyNames;
        }

        public string NameSpaceName { get; }
        public string ClassName { get; }
        public IEnumerable<string> PropertyNames { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ClassToGenerate);
        }

        public bool Equals(ClassToGenerate? other)
        {
            return other is not null &&
                   NameSpaceName == other.NameSpaceName &&
                   ClassName == other.ClassName && PropertyNames.SequenceEqual(other.PropertyNames);
        }

        public override int GetHashCode()
        {
            int hashCode = 1462379383;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NameSpaceName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClassName);
            return hashCode;
        }
    }
}
