using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UserService.Models;


namespace UserService.Repository.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _Context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UnitOfWork(DBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _Context = context;
            _httpContextAccessor = httpContextAccessor;

        }
        public CurrentUser GetCurrentUser()
        {
            CurrentUser CurrentUser = new();
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null)
            {
                CurrentUser = new();
                if (user.HasClaim(x => x.Type == "employeeid")) CurrentUser.EmployeeId = Guid.Parse(user.Claims.First(x => x.Type == "employeeid").Value);
                if (user.HasClaim(x => x.Type == "userid")) CurrentUser.UserId = Guid.Parse(user.Claims.First(x => x.Type == "userid").Value);
                if (user.HasClaim(x => x.Type == "companyid")) CurrentUser.CompanyId = Guid.Parse(user.Claims.First(x => x.Type == "companyid").Value);
                if (user.HasClaim(x => x.Type == "orgid")) CurrentUser.OrgId = Guid.Parse(user.Claims.First(x => x.Type == "orgid").Value);
                if (user.HasClaim(x => x.Type == "displayname")) CurrentUser.UserName = user.Claims.First(x => x.Type == "displayname").Value;
            }
            return CurrentUser;
        }

        private readonly Dictionary<Type, object> _Repositories = new();
        public GenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            // Check to see if we have a constructor for the given type
            if (!_Repositories.ContainsKey(typeof(TEntity)))
            {
                _Repositories.Add(typeof(TEntity), new GenericRepository<TEntity>(_Context));

            }
            return (GenericRepository<TEntity>)_Repositories[typeof(TEntity)];
        }

        public async Task Save()
        {
            await _Context.SaveChangesAsync();
        }

        public void Detach(object obj)
        {
            _Context.Entry(obj).State = EntityState.Detached;
        }

        public DBContext GetContext()
        {
            return _Context;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Context.Dispose();
            }
        }
    }
}
