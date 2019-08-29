using Beloning.Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beloning.Services.Contracts
{
    public interface IReferralService
    {
        Task<ResponseDto> Add(ReferralDto user);
        Task<ResponseDto<DashboardDto>> Update(int id, int status);
        Task<ResponseDto<DashboardDto>> GetDashboardInformation();
        Task<ResponseDto<List<PatientInfoDto>>> GetPatients();

    }

}
