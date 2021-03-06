﻿using binaryTreeWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace binaryTreeWebAPI.Interfaces
{
    public interface IbTree
    {
        // Método para crear un arbol binario
        BTree createBTree(string bTreeName);

        // Método para calcular el LCA
        int calculateLCA(ref Node actualNode, int value1, int value2, ref Node originNode);

        // Método para validar la estructura del JSON del arbol binario
        Error validateBTree(DataFormat bTreeData);

        // Método para crear un arbol binario en base de datos
        void saveBTree(DataFormat bTreeJSON);

        // Método para obtene los nombres de arboles creados en base de datos
        List<string> getBTreeNames();
        // Método para validar que los nodos pertenezcan al arbol cargado
        Error validateNodes(string bTreeName, int n1, int n2);
    }
}
