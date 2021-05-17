using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace JSONtoXML.Universal
{
    public class UniversalForm
    {
        private ExpandoObject Buffer { get; }

        public UniversalForm(ref ExpandoObject Buffer)
        {
            this.Buffer = Buffer;
        }
    }
}
