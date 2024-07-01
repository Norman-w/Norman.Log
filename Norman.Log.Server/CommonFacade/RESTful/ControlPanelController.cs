using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Norman.Log.Server.CommonFacade;
[ApiController]
[Route("[controller]")]
public class ControlPanelController : ControllerBase
{
	private readonly ILogger<ControlPanelController> _logger;

	public ControlPanelController(ILogger<ControlPanelController> logger)
	{
		_logger = logger;
	}

	[HttpGet]
	public string Get()
	{
		return "This is the control panel.";
	}
}