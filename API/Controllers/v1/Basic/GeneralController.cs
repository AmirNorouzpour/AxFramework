using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework;
using Entities.MasterSignal;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class GeneralController : BaseController
    {
        private readonly IBaseRepository<ConfigData> _repository;
        private readonly IBaseRepository<UserData> _userDataRepository;

        public GeneralController(IBaseRepository<ConfigData> repository, IBaseRepository<UserData> userDataRepository)
        {
            _repository = repository;
            _userDataRepository = userDataRepository;
        }


        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<IActionResult> GetOrganizationLogo(CancellationToken cancellationToken)
        {
            var data = await _repository.GetFirstAsync(x => x.Active, cancellationToken);

            if (data == null)
                return NotFound();

            return File(data.OrganizationLogo.ToArray(), GeneralUtils.GetContentType("a.png"));
        }

        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]")]
        public async Task<ApiResult<DateTime>> AddApiUser(UserDataDto userDataDto, CancellationToken cancellationToken)
        {
            var userData = await _userDataRepository.GetFirstAsync(x => x.MobileNumber == userDataDto.MobileNumber, cancellationToken);
            if (userData != null)
                return userData.ExpireDate;

            var u = new UserData
            {
                ExpireDate = DateTime.Now.AddDays(7),
                ApiKey = userDataDto.ApiKey,
                MobileNumber = userDataDto.MobileNumber,
                PhrasePassword = userDataDto.PhrasePassword,
                SecretKey = userDataDto.SecretKey,
                InitBalance = 0,
                Balance = 0
            };
            await _userDataRepository.AddAsync(u, cancellationToken);
            return u.ExpireDate;

        }
    }
}