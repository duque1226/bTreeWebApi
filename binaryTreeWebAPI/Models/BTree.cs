using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace binaryTreeWebAPI.Models
{
    public class BTree
    {
        public Node root;
        public BTree()
        {
            root = null;
        }

        // Constructor sobrecargado cuando se enviar un valor para inicializar
        public BTree(int nodeValue)
        {
            root = new Node(nodeValue);
        }

        // Método para agregar un nuevo nodo
        public void addNewNode(int nodeValue)
        {
            this.addNewNodeMethod(ref root, nodeValue);
        }

        // Método para agregar un nuevo nodo de forma recursiva
        private void addNewNodeMethod(ref Node actualNode, int nodeValue)
        {
            // Verificando si el nodo raiz esta vacio
            if (actualNode == null)
            {
                // Creando el nuevo nodo
                actualNode = new Node(nodeValue);
                return;
            }
            // Verificando si se debe almacenar el nodo en la rama izquierda
            else if (nodeValue < actualNode.nodeValue)
            {
                // Llamando recursivamente el metodo de agregar con el puntero de la rama izquierda
                addNewNodeMethod(ref actualNode.leftBranch, nodeValue);
                return;
            }
            // Verificando si se debe almacenar el nodo en la rama derecha
            else if (nodeValue >= actualNode.nodeValue)
            {
                // Llamando recursivamente el metodo de agregar con el puntero de la rama derecha
                addNewNodeMethod(ref actualNode.rightBranch, nodeValue);
                return;
            }
        }
    }
}
