using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpersLib
{
    public abstract class CodeMenuEntry
    {
        protected readonly String _value, _description;

        public CodeMenuEntry(string value, string description)
        {
            _value = value;
            _description = description;
        }

        public String Value { get { return _value; } }
        public String Description { get { return _description; } }
        
        public abstract string ToPrefixString();
    }
}
