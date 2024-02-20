using System.ComponentModel.DataAnnotations;
using System.Net;
using Infrastructure.Common.Models;
using Inventory.API.Entities;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Shared.DTOs.Inventory;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("items/{itemNo}", Name = "GetAllByItemNo")]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo)
    {
        var result = await _inventoryService.GetAllByItemNoAsync(itemNo);
        return Ok(result);
    }

    [HttpGet("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
    [ProducesResponseType(typeof(PagedList<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedList<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo,
        [FromQuery] GetInventoryPagingQuery query)
    {
        query.SetItemNo(itemNo);
        var result = await _inventoryService.GetAllByItemNoPagingAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}", Name = "GetInventoryById")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> GetInventoryById([Required] string id)
    {
        var result = await _inventoryService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost("purchase/{itemNo}", Name = "PurchaseOrder")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder([Required] string itemNo,
        [FromBody] PurchaseProductDto model)
    {
        model.SetItemNo(itemNo);
        var result = await _inventoryService.PurchaseItemAsync(model);
        return Ok(result);
    }

    [HttpPost("sales/{itemNo}", Name = "SalesItem")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> SalesItem([Required] string itemNo,
        [FromBody] SalesProductDto model)
    {
        model.SetItemNo(itemNo);
        var result = await _inventoryService.SalesItemAsync(itemNo, model);
        return Ok(result);
    }

    [HttpPost("sales/order-no/{orderNo}", Name = "SalesOrder")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> SalesOrder([Required] string orderNo,
        [FromBody] SalesOrderDto model)
    {
        model.OrderNo = orderNo;
        var documentNo = await _inventoryService.SalesOrderAsync(model);
        var result = new CreatedSalesOrderSuccessDto(documentNo);
        return Ok(result);
    }

    [HttpDelete("{id}", Name = "DeleteInventoryById")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> DeleteInventoryById([Required] string id)
    {
        var entity = await _inventoryService.GetByIdAsync(id);
        if (entity == null) return NotFound();
        await _inventoryService.DeleteAsync(id);
        return NoContent();
    }
}