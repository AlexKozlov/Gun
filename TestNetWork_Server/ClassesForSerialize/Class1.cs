using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassesForSerialize
{
    [Serializable]
    public class SomeText
    {
        public int year;
        public string message;
        public double pi;

        public SomeText()
        {
            message = "";
        }

        public SomeText(int year, string message, double pi)
        {
            this.year = year;
            this.message = message;
            this.pi = pi;
        }
    }
}
