using System;
using System.Linq;
using System.Threading.Tasks;
using Beloning.Services.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Produces("application/json")]
    public class BaseController : Controller
    {
        protected IActionResult GenerateApiResponse<T>(Func<ResponseDto<T>> response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = response();

            if (result.Success)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Error);

        }

        protected async Task<IActionResult> GenerateApiResponse<T>(Func<Task<ResponseDto<T>>> response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await response();

            if (result.Success)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Error);
        }

        protected async Task<IActionResult> GenerateApiResponse(Func<Task<ResponseDto>> response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await response();

            if (result.Success)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Error);

        }

        protected IActionResult GenerateApiResponse<T>(ResponseDto<T> result)
        {
            if (result.Success)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Error);

        }

        protected IActionResult GenerateApiResponse(ResponseDto result)
        {
            if (result.Success)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Error);

        }
    }
}
