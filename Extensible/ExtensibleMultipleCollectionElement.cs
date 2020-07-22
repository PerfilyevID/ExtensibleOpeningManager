using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Extensible
{
    public class ExtensibleMultipleCollectionElement
    {
        public ExtensibleParameter Parameter { get; }
        public string Value { get; }
        public ExtensibleMultipleCollectionElement(ExtensibleParameter parameter, string value)
        {
            Parameter = parameter;
            Value = value;
        }
    }
}
