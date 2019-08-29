using AutoMapper;
using Beloning.Data.UnitOfWork;
using Beloning.Services.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace Beloning.Services.Base
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IMapper Mapper;
        protected readonly ILogger<BaseService> Logger;
        internal BaseService(IUnitOfWork unitOfWork, ILogger<BaseService> logger, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            Logger = logger;
        }

        protected string GenerateErrorMessage([CallerMemberName] string memberName = "")
        {
            return $"Something went wrong when executing  method: {memberName}";
        }

        protected string NotAllFieldsAreFilledIn()
        {
            return $"Not All Fields are filled in";
        }

        protected string GenerateNotAllowedMessage()
        {
            return $"Your are not allowed to perform this action";
        }

        protected string GenerateErrorMessage<T>(T dto, [CallerMemberName] string memberName = "") where T : EntityDto
        {
            if (dto != null)
            {
                return GenerateErrorMessage(dto.Id, memberName);
            }

            return GenerateErrorMessage(memberName);
        }

        protected string GenerateErrorMessage(int id, [CallerMemberName] string memberName = "")
        {
            return $"Something went wrong when executing the method: {memberName} (Id: {id})";
        }

        protected string GenerateItemNotFoundMessage<T>(object id)
        {
            return $"Could not find {typeof(T).Name} with id {id}";
        }

        protected void CheckForValidDto<T>(T dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException($"{typeof(T).Name}");
            }
        }

    }
}
