using binaryTreeWebAPI.Interfaces;
using binaryTreeWebAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace binaryTreeWebAPI.ServicesCore
{
    public class BTreeServiceCore : IbTree
    {
        private readonly IConfiguration _configuration;
        private string connString;

        public BTreeServiceCore(IConfiguration configuration)
        {
            _configuration = configuration;
            connString = _configuration.GetSection("SQL:connectionString").Value;

        }

        public int calculateLCA(ref Node actualNode, int value1, int value2, ref Node originNode)
        {
            int dMax = value1;
            int dMin = value2;
            // Verificando si value1 es mayor a value 2
            if (value2 > value1)
            {
                dMax = value2;
                dMin = value1;
            }
            if ((actualNode.nodeValue < dMax) && (actualNode.nodeValue > dMin))
            {
                return actualNode.nodeValue;
            }
            else
            {
                if (dMax < actualNode.nodeValue)
                {
                    return (calculateLCA(ref actualNode.leftBranch, dMax, dMin, ref actualNode));
                }
                else
                {
                    return (calculateLCA(ref actualNode.rightBranch, dMax, dMin, ref actualNode));
                }
            }
        }

        public BTree createBTree(string bTreeName)
        {

            using (SqlConnection conn = new SqlConnection(this.connString))
            {
                int rootValue = 0;
                List<int> dataBTree = new List<int>();

                conn.Open();
                string query = "";

                // Cargando el nodo raiz del arbol binario deseado
                query = "SELECT * FROM bTreeInfo WHERE bTreeName = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", bTreeName);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        rootValue = Convert.ToInt32(reader["bTreeRoot"]);
                    }
                }
                conn.Close();

                // Obteniendo todos los nodos del árbol binario
                dataBTree = this.getNodes(bTreeName);

                // Creando el objeto del arbol binario
                BTree bTree = new BTree(rootValue);
                for (int i = 1; i < dataBTree.Count; i++)
                {
                    bTree.addNewNode(dataBTree[i]);
                }
                return bTree;
            }
        }

        // Método para validar que la estructura del arbol binario venga correcta
        public Error validateBTree(DataFormat bTreeData)
        {
            Error error = new Error();
            error.isError = false;
            error.msjError = "";
            List<string> bTreeNames = new List<string>();
            // Obteniendo los nombres de los arbiles almacenados en base de datos
            bTreeNames = this.getBTreeNames();
            // Verificando si ya existe un arbol con el nombre actual
            if (bTreeNames.IndexOf(bTreeData.bTreeName) != -1)
            {
                error.msjError = "El árbol con nombre " + bTreeData.bTreeName + " ya existe, cambie el nombre";
            }
            else if (bTreeData.nodeValues.Length == 0)
            {
                error.msjError = "Debe ingresar los nodos del arbol binario";
            }
            else if (string.IsNullOrEmpty(bTreeData.bTreeName))
            {
                error.msjError = "Debe ingresar un nombre para el arbol binario";
            }

            // Verificando si hay un mensaje de error
            if (error.msjError.Length > 0)
            {
                error.isError = true;
            }
            return error;

        }

        public void saveBTree(DataFormat bTreeJSON)
        {
            // Obteniendo la cadena de conexion desde el appsettings            
            using (SqlConnection conn = new SqlConnection(this.connString))
            {
                conn.Open();
                string query = "";
                using (SqlTransaction transact = conn.BeginTransaction())
                {
                    try
                    {
                        // Agregando los nodos 
                        foreach (int nValue in bTreeJSON.nodeValues)
                        {
                            query = "INSERT INTO dbo.bTreeData (bTreeValue, idBTree) VALUES (@valueNode, @bTreeName)";
                            using (SqlCommand cmd = new SqlCommand(query, conn, transact))
                            {
                                cmd.Parameters.AddWithValue("@valueNode", nValue);
                                cmd.Parameters.AddWithValue("@bTreeName", bTreeJSON.bTreeName);
                                cmd.ExecuteNonQuery();
                            }
                            query = "";
                        }

                        // Agregando el arbol binario
                        query = "INSERT INTO dbo.bTreeInfo (bTreeName, bTreeRoot) VALUES (@name, @root)";
                        using (SqlCommand cmd = new SqlCommand(query, conn, transact))
                        {
                            cmd.Parameters.AddWithValue("@name", bTreeJSON.bTreeName);
                            cmd.Parameters.AddWithValue("@root", bTreeJSON.rootValue);
                            cmd.ExecuteNonQuery();
                        }

                        transact.Commit();
                        conn.Close();

                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        // Devolviendo la transaccion porque se presento un error en el proceso
                        transact.Rollback();
                        throw new Exception(ex.Message);

                    }
                }
            }
        }

        // Método para obtene los nombres de arboles creados en base de datos
        public List<string> getBTreeNames()
        {
            List<string> bTreeNames = new List<string>();
            using (SqlConnection conn = new SqlConnection(this.connString))
            {
                conn.Open();
                string query = "";
                query = "SELECT DISTINCT(bTreeName) from bTreeInfo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Almacenando los nombres de arboles existentes
                        bTreeNames.Add(reader["bTreeName"].ToString());
                    }
                }
                conn.Close();
            }
            return bTreeNames;
        }

        // Método para validar que los nodos pertenezcan al árbol binario
        public Error validateNodes(string bTreeName, int n1, int n2)
        {
            Error error = new Error();
            error.isError = false;
            error.msjError = "";
            List<int> bTreeNodes = new List<int>();
            bTreeNodes = this.getNodes(bTreeName);
            if ((bTreeNodes.IndexOf(n1) == -1) || (bTreeNodes.IndexOf(n2) == -1))
            {
                error.isError = true;
                error.msjError = "Los nodos deben existir en el árbol binario";
            }
            return error;
        }

        private List<int> getNodes(string bTreeName)
        {
            try
            {
                List<int> dataBTree = new List<int>();
                using (SqlConnection conn = new SqlConnection(this.connString))
                {
                    conn.Open();
                    string query = "";
                    // Cargando los datos del arbol binario deseado
                    query = "SELECT * FROM btreeData WHERE idBTree = @name";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", bTreeName);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            // Almacenando los nodos del arbol binario
                            dataBTree.Add(Convert.ToInt32(reader["bTreeValue"]));
                        }
                    }
                    conn.Close();
                }
                return dataBTree;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
