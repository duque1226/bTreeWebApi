using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace binaryTreeWebAPI.Models
{
    public class Node
    {
        public int nodeValue;
        public Node leftBranch;
        public Node rightBranch;
        public Node(int nodeValue)
        {
            this.nodeValue = nodeValue;
            this.leftBranch = null;
            this.rightBranch = null;
        }
    }
}
