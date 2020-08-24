using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace ETWTestProvider
{
    [EventSource(Guid = "{3bcf1aae-4f19-49a0-9521-533e43fce039}")]
    public sealed class EventClass : EventSource
    {
        public void Test1(string message)
        {
            WriteEvent(1, message);
        }
    }
}
