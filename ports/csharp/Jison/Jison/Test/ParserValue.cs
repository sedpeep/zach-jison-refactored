using System;

namespace jQuerySheet
{
    public class ParserValue
    {
        public string Text { get; set; }
        public int Leng { get; set; }
        public ParserLocation Loc { get; set; }
        public int LineNo { get; set; }
        public bool ValueSet { get; set; }
        public bool BoolValue { get; set; }
        public double DoubleValue { get; set; }

        public ParserValue()
        {
            Loc = new ParserLocation();
            LineNo = 1;
        }

        public ParserValue(ParserValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Text = value.Text;
            Leng = value.Leng;
            Loc = value.Loc;
            LineNo = value.LineNo;
            ValueSet = value.ValueSet;
            BoolValue = value.BoolValue;
            DoubleValue = value.DoubleValue;
        }

        public virtual ParserValue Clone()
        {
            return new ParserValue(this);
        }

        public virtual bool ToBool()
        {
            ValueSet = true;
            BoolValue = Convert.ToBoolean(Text);
            return BoolValue;
        }

        public virtual void Set(bool value)
        {
            BoolValue = value;
            ValueSet = true;
        }

        public virtual double ToDouble()
        {
            ValueSet = true;
            if (!string.IsNullOrEmpty(Text) || DoubleValue != 0)
            {
                if (double.TryParse(Text, out double num))
                {
                    DoubleValue = num;
                }
                else
                {
                    DoubleValue = DoubleValue != 0 ? DoubleValue : 0;
                }
                ValueSet = true;
            }
            else
            {
                DoubleValue = 0;
            }
            return DoubleValue;
        }

        public virtual bool IsNumeric()
        {
            if (ValueSet && DoubleValue > 0)
            {
                return true;
            }

            return double.TryParse(Text, out _);
        }

        public virtual void Set(double value)
        {
            DoubleValue = value;
            ValueSet = true;
        }

        public virtual void Add(ParserValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value.ToDouble();
            DoubleValue += value.DoubleValue;
        }

        public virtual string ToString()
        {
            ValueSet = true;
            return Text;
        }

        public virtual void Set(string value)
        {
            Text = value;
            ValueSet = true;
        }

        public virtual void Concat(ParserValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Text += value.Text;
        }
    }
} 