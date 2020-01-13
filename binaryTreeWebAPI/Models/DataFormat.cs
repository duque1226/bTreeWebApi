using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace binaryTreeWebAPI.Models
{
    public class DataFormat
    {
        public string bTreeName { get; set; }
        public int rootValue { get; set; }
        public int[] nodeValues { get; set; }
    }
}
