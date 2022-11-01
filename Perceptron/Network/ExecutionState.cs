using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public enum ExecutionStateGroup
    {
        Normal,
        SumMember,
        Sum,
        Activation,
        UpdateBias,
        UpdateWeight
    }

    public class ExecutionState
    {
        public ExecutionStateGroup Group { get; set; }
        public int Index { get; set; } = 0;
    }
}
