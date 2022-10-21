using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Data.Repositories;
using Entities.Framework;

namespace Services.Services
{
    public interface IUserConnectionService
    {
        List<string> GetActiveConnections();
        List<string> GetActiveConnections(int userId);
    }

    public class UserConnectionService: IUserConnectionService, IScopedDependency
    {
        private readonly IBaseRepository<UserConnection> _repository;

        public UserConnectionService(IBaseRepository<UserConnection> repository)
        {
            _repository = repository;
        }

        public List<string> GetActiveConnections()
        {
            var connections = _repository.GetAll(x => x.UserToken.ExpireDateTime > DateTime.Now).Select(x => x.ConnectionId).ToList();
            return connections;
        }

        public List<string> GetActiveConnections(string symbol)
        {
            var connections = _repository.GetAll(x => x.UserToken.ExpireDateTime > DateTime.Now).Select(x => x.ConnectionId).ToList();
            return connections;
        }

        public List<string> GetActiveConnections(int userId)
        {
            var connections = _repository.GetAll(x => x.UserToken.ExpireDateTime > DateTime.Now &&x.UserId == userId).Select(x => x.ConnectionId).ToList();
            return connections;
        }
    }

   
}
