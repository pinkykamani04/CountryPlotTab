using System;
using System.Collections.Generic;
using System.Text;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class WasteStream
    {
        public int WasteStreamId { get; set; }

        public string WasteStreamName { get; set; }

        public decimal TargetValue { get; set; }
    }
}
