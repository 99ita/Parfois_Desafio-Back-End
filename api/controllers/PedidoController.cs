using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.DTOs;

namespace api.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase{
        private readonly PedidoService _pedidoService;

        public PedidoController(PedidoService pedidoService){
            _pedidoService = pedidoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(string id){
            try{
                var pedido = await _pedidoService.GetPedidoAsync(id);
                if (pedido == null) return NotFound();
                return Ok(pedido);
            }
            catch(Exception ex){
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePedido(PedidoDto pedidoDto){
            try{
                var pedido = await _pedidoService.AddPedidoAsync(pedidoDto);
                return CreatedAtAction(nameof(GetPedido), new { id = pedido.PedidoId }, pedido);
            }
            catch(Exception ex){
                return HandleException(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePedido(PedidoDto pedidoDto){
            try{
                await _pedidoService.UpdatePedidoAsync(pedidoDto);
                return NoContent();
            }
            catch(Exception ex){
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(string id){
            try{
                await _pedidoService.DeletePedidoAsync(id);
                return NoContent();
            }
            catch(Exception ex){
                return HandleException(ex);
            }
        }


        private IActionResult HandleException(Exception ex){
            if (ex is InvalidOperationException){
                return NotFound(new { error = "Pedido not found.", message = ex.Message });
            }
            else if (ex is ArgumentNullException || ex is ArgumentException || ex is ArgumentOutOfRangeException){
                return BadRequest(new { error = ex.Message });
            }
            else{
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }


    }
}