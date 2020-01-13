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
            throw new NotImplementedException();
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
                        rootValue = Convert.ToInt32(reader["bTreeName"]);
                    }
                }

                // Cargando los datos del arbol binario deseado
                query = "SELECT * FROM btreeData WHERE idBTree = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", bTreeName);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Almacenando los nodos del arbol binario
                        dataBTree.Add(Convert.ToInt32(reader["bTreeName"]));
                    }
                }

                // Creando el objeto del arbol binario
                BTree bTree = new BTree(rootValue);
                for (int i = 1; i < dataBTree.Count; i++)
                {
                    bTree.addNewNode(dataBTree[i]);
                }

                return bTree;
            }
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



    }
}
