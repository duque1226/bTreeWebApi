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

        [HttpPost]
        [Route("createBTree")]
        public IActionResult Post([FromBody] DataFormat value)
        {
            // Validando que el objeto no venga vacio
            if (value != null)
            {                
                // Guardando el arbol en base de datos
                _ibTree.saveBTree(value);
                return Ok();
                
            }
            else
            {
                return BadRequest("Formato erroneo del JSON");
            }

        }



    }
}