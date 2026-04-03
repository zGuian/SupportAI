using Microsoft.AspNetCore.Mvc;
using SupportAI.Core.Interface.Services;

namespace SupportAI.WebApi.Controllers
{
    public class IngestionController : ControllerBase
    {
        [HttpPost("upload-file")]
        public async Task<IActionResult> ReceveidFilePdf([FromServices] IIngestionService ingestionService, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Arquivo inválido");
            }

            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                if (file.FileName.EndsWith(".pdf"))
                {
                    await ingestionService.ProcessPdfAsync(filePath);
                }
                if (file.FileName.EndsWith(".md"))
                {
                    await ingestionService.ProcessMarkdownAsync(filePath);
                }
                else
                {
                    return BadRequest("Formato de ficheiro não suportado no momento.");
                }

                return Ok(new { message = $"Ficheiro {file.FileName} processado e enviado para o banco de dados!" });
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJsonAsync([FromServices] IIngestionService ingestionService)
        {
            return BadRequest("Loading...");
        }
    }
}
