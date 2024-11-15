using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    [HttpPost("CreateRoom")]
    public IActionResult CreateRoom([FromBody] CreateRoomRequest request)
    {
        return Ok(new { Success = true, RoomCode = request.RoomCode });
    }
}

public class CreateRoomRequest
{
    public string RoomCode { get; set; }
}
