using logiked.source.extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace logiked.source.graphNode
{

    /// <summary>
    /// Point d'attache d'une transition entre deux nodes
    /// </summary>
    [Serializable]
    public class NodeTransition
    {
        public NodeTransition Clonetransition()
        {
            return (NodeTransition)this.SerializeToByteArray().DeserializeToObject();
        }

        [SerializeField] private int nextNodeId;

        /// <summary>
        /// Todo : node type connection
        /// </summary>

        public int NextNodeId
        {
            get { return nextNodeId; }
#if UNITY_EDITOR
            set { nextNodeId = value; }
#endif
        }


        public NodeTransition(int nextNodeId)
        {
            this.nextNodeId = nextNodeId;
        }


    }
}
