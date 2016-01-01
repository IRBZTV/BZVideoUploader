using System;
using System.Collections.Generic;
using System.Text;

namespace MyList
{
    public class MyListItem
    {
        private string _name;
        private int _value;

        public MyListItem(string name, int value)
        {
            _name = name;
            _value = value;
        }
        public int value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string  Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
