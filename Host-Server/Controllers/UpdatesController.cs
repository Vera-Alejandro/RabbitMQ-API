using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MessageType;
using Newtonsoft.Json;

namespace Host_Server.Controllers
{
	[Produces("application/json")]
	[Route("updates")]
	[ApiController]
	public class UpdatesController : ControllerBase
	{
		[HttpGet("{applicationId}/download")]
		public IActionResult Download(string applicationId)
		{
			string location = @$"C:\Updates\{applicationId}";
			string executable = Directory.GetFiles(location, "*.exe").FirstOrDefault();

			FileStream stream = new FileStream(executable, FileMode.Open);

			return File(stream, "application/octet-stream");
		}

		[HttpGet("{applicationId}/information")]
		public IActionResult Information(string applicationId)
		{
			try
			{
				string location = @$"C:\Updates\{applicationId}";
				string jsonFile = Directory.GetFiles(location, "*.json").FirstOrDefault();

				var fileInfo = JsonConvert.DeserializeObject<FIleInformation>(System.IO.File.ReadAllText(jsonFile));

				return Ok(fileInfo);
			}
			catch (System.Exception)
			{

				return NoContent();
			}
		}
	}
}