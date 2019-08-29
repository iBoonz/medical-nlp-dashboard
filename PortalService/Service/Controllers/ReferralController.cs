using System.Threading.Tasks;
using Beloning.Services.Contracts;
using Beloning.Services.Model;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferralController : BaseController
    {
        private readonly IReferralService _referralService;

        public ReferralController(IReferralService referralService)
        {
            _referralService = referralService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReferralInformation(ReferralDto referral)
        {
            return await GenerateApiResponse(() => _referralService.Add(referral));
        }

        [HttpPut("{id}/status/{status}")]
        public async Task<IActionResult> UpdateReferralInformation(int id, int status)
        {
            return await GenerateApiResponse(() => _referralService.Update(id, status));
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardInformation()
        {
            return await GenerateApiResponse(() => _referralService.GetDashboardInformation());
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            return await GenerateApiResponse(() => _referralService.GetPatients());
        }
    }
}
