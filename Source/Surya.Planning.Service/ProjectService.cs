using System.Collections.Generic;
using System.Linq;
using Surya.India.Data;
using Surya.India.Data.Infrastructure;
using Surya.India.Model.Models;

using Surya.India.Core.Common;
using System;
using Surya.India.Model;
using System.Threading.Tasks;
using Surya.India.Data.Models;

namespace Surya.India.Service
{
    public interface IProjectService : IDisposable
    {
        Project Create(Project pt);
        void Delete(int id);
        void Delete(Project pt);
        Project Find(string Name);
        Project Find(int id);
        IEnumerable<Project> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Project pt);
        Project Add(Project pt);
        IQueryable<Project> GetProjectList();
        Task<IEquatable<Project>> GetAsync();
        Task<Project> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class ProjectService : IProjectService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Project> _ProjectRepository;
        RepositoryQuery<Project> ProjectRepository;
        public ProjectService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ProjectRepository = new Repository<Project>(db);
            ProjectRepository = new RepositoryQuery<Project>(_ProjectRepository);
        }        

        public Project Find(string Name)
        {
            return ProjectRepository.Get().Where(i => i.ProjectName == Name).FirstOrDefault();
        }


        public Project Find(int id)
        {
            return _unitOfWork.Repository<Project>().Find(id);
        }

        public Project Create(Project pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Project>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Project>().Delete(id);
        }

        public void Delete(Project pt)
        {
            _unitOfWork.Repository<Project>().Delete(pt);
        }

        public void Update(Project pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Project>().Update(pt);
        }

        public IEnumerable<Project> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Project>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.ProjectName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<Project> GetProjectList()
        {
            var pt = _unitOfWork.Repository<Project>().Query().Get().OrderBy(m=>m.ProjectName);

            return pt;
        }

        public Project Add(Project pt)
        {
            _unitOfWork.Repository<Project>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Project
                        orderby p.ProjectName
                        select p.ProjectId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Project
                        orderby p.ProjectName
                        select p.ProjectId).FirstOrDefault();
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

                temp = (from p in db.Project
                        orderby p.ProjectName
                        select p.ProjectId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Project
                        orderby p.ProjectName
                        select p.ProjectId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }
        public void Dispose()
        {
        }


        public Task<IEquatable<Project>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Project> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
