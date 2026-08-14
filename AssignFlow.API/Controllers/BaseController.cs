using AssignFlow.API.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid CurrentUserId => User.GetUserId();
    protected string CurrentRole => User.GetRole();
}
