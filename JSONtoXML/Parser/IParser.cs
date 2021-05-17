using JSONtoXML.Universal;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace JSONtoXML.Parser
{
    public interface IParser
    {
        public void loadJSON(string path);
        public ExpandoObject GetUniversal();
    }
}
