using Beloning.Services.Model;
using System.Threading.Tasks;

namespace Beloning.Services.Contracts
{
    public interface IStorageService
    {
        Task<ResponseDto<byte[]>> GetImage(string uri);
        Task<ResponseDto<string>> Post(string photoName, byte[] image);
    }

}
