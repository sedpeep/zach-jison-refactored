using System;

namespace jQuerySheet
{
    public class ParserSymbol
    {
        public string Name { get; }
        public int Index { get; }
        public object Value { get; set; }
        public ParserLocation Location { get; set; }

        public ParserSymbol(string name, int index)
        {
            Name = name;
            Index = index;
            Location = new ParserLocation();
        }

        public ParserSymbol(string name, int index, object value)
        {
            Name = name;
            Index = index;
            Value = value;
            Location = new ParserLocation();
        }

        public override string ToString()
        {
            return $"{Name} ({Index})";
        }

        public override bool Equals(object obj)
        {
            if (obj is ParserSymbol other)
            {
                return Name == other.Name && Index == other.Index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Index);
        }
    }
} 