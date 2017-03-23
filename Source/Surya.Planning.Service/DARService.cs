using System.Collections.Generic;
using System.Linq;
using Surya.India.Data;
using Surya.India.Data.Infrastructure;
using Surya.India.Model.Models;

using Surya.India.Core.Common;
using System;
using Surya.India.Model;
using Surya.India.Data.Models;
using System.Threading.Tasks;
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface IDARService : IDisposable
    {
        DAR Create(DAR pt);
        void Delete(int id);
        void Delete(DAR pt);
        DAR Find(int id);
        IEnumerable<DAR> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(DAR pt);
        DAR Add(DAR pt);
        IQueryable<DARViewModel> GetDARList(string UserName);
        Task<IEquatable<DAR>> GetAsync();
        Task<DAR> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class DARService : IDARService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<DAR> _DARRepository;
        RepositoryQuery<DAR> DARRepository;
        public DARService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _DARRepository = new Repository<DAR>(db);
            DARRepository = new RepositoryQuery<DAR>(_DARRepository);
        }

        public DAR Find(int id)
        {
            return _unitOfWork.Repository<DAR>().Find(id);
        }

        public DAR Create(DAR pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<DAR>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<DAR>().Delete(id);
        }

        public void Delete(DAR pt)
        {
            _unitOfWork.Repository<DAR>().Delete(pt);
        }

        public void Update(DAR pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<DAR>().Update(pt);
        }

        public IEnumerable<DAR> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<DAR>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DARId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<DARViewModel> GetDARList(string UserName)
        {
            var pt = (from p in db.DAR
                      where p.ForUser==UserName
                      orderby p.DARDate descending
                      select new DARViewModel
                      {
                          DARDate = p.DARDate,
                          DARId = p.DARId,
                          Description = p.Description,
                          DueDate = p.DueDate,
                          ForUser = p.ForUser,
                          FromTime = p.FromTime,
                          IsActive = p.IsActive,
                          Priority = p.Priority,
                          Status = p.Status,                          
                          TaskName = p.Task.TaskTitle,
                          ToTime = p.ToTime,
                          WorkHours = p.WorkHours
                      }
                );

            return pt;
        }

        public DAR Add(DAR pt)
        {
            _unitOfWork.Repository<DAR>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.DAR
                        orderby p.DARId
                        select p.DARId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.DAR
                        orderby p.DARId
                        select p.DARId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.DAR
                        orderby p.DARId
                        select p.DARId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.DAR
                        orderby p.DARId
                        select p.DARId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public void Dispose()
        {
        }


        public Task<IEquatable<DAR>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DAR> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
