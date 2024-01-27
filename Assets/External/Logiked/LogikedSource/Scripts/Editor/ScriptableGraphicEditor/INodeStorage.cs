using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logiked.source.graphNode
{

    /// <inheritdoc/>
    public interface INodeStorage : INodeStorage<GraphicNode, NodeTransition>  {    }

        /// <summary>
        /// Interface pour récuperer / Set des nodes graphiques dans un scriptable object
        /// </summary>
        /// <typeparam name="N">Type de nodes</typeparam>
        /// <typeparam name="T">Type de transitions</typeparam>
        public interface INodeStorage<N, T> where N : GraphicNode<T> where T : NodeTransition
    {
        public string GetNodeArrayPropertyPath { get; }
        public List<N> GetNodes();
        public void SetNodes(List<N> newNodes);
        /// <summary>
        /// Type des nouveau nodes à instanciés avec 
        /// </summary>
        /// <returns></returns>
       // public Type NewNodeTypes();
        public N GetNodeById(int id);

    }
}
