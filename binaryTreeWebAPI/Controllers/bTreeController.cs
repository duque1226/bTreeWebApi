using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using binaryTreeWebAPI.Interfaces;
using binaryTreeWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace binaryTreeWebAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class bTreeController : ControllerBase
    {
        private readonly IbTree _ibTree;

        public bTreeController(IbTree ibTree)
        {
            // Inyección de dependencias de los metodos del árbol binario
            _ibTree = ibTree;
        }

        [HttpGet]
        [Route("lca")]
        public IActionResult calcLCA(string bTreeName, int node1, int node2)
        {
            // Obteniendo la lista de los arboles creados
            List<string> bTreeNamesCreated = _ibTree.getBTreeNames();
            // Validando si el nombre del árbol es válido
            if (bTreeNamesCreated.IndexOf(bTreeName) != -1)
            {
                // Validando si los nodos pertenecen al árbol binario
                Error error = _ibTree.validateNodes(bTreeName, node1, node2);
                if (error.isError)
                {
                    return BadRequest(error.msjError);
                }
                // Creando el árbol binario
                BTree bTree = _ibTree.createBTree(bTreeName);
                // Calculando el LCA
                int lca = _ibTree.calculateLCA(ref bTree.root, node1, node2, ref bTree.root);
                return Ok(lca);
            }
            return BadRequest("El nombre del árbol no se encuentra en la base de datos");
        }

        [HttpPost]
        [Route("createBTree")]
        public IActionResult Post([FromBody] DataFormat value)
        {
            // Validando que el objeto no venga vacio
            if (value != null)
            {
                // Verificando la estructura del objeto JSON
                Error error = _ibTree.validateBTree(value);
                if (error.isError)
                {
                    return BadRequest(error.msjError);
                }
                else
                {
                    // Guardando el arbol en base de datos
                    _ibTree.saveBTree(value);
                    return Ok();
                }
            }
            else
            {
                return BadRequest("Formato erroneo del JSON");
            }

        }
    }
}