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
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface ITasksService : IDisposable
    {
        Tasks Create(Tasks pt);
        void Delete(int id);
        void Delete(Tasks pt);
        Tasks Find(string Name);
        Tasks Find(int id);
        IEnumerable<Tasks> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Tasks pt);
        Tasks Add(Tasks pt);
        IQueryable<TasksViewModel> GetTasksList(string User,string Status);
        IQueryable<TasksViewModel> GetOutBoxTasksList(string User,string Status);
        Task<IEquatable<Tasks>> GetAsync();
        Task<Tasks> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class TasksService : ITasksService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<Tasks> _TasksRepository;
        RepositoryQuery<Tasks> TasksRepository;
        public TasksService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _TasksRepository = new Repository<Tasks>(db);
            TasksRepository = new RepositoryQuery<Tasks>(_TasksRepository);
        }        

        public Tasks Find(string Name)
        {
            return TasksRepository.Get().Where(i => i.TaskTitle == Name).FirstOrDefault();
        }


        public Tasks Find(int id)
        {
            return _unitOfWork.Repository<Tasks>().Find(id);
        }

        public Tasks Create(Tasks pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Tasks>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Tasks>().Delete(id);
        }

        public void Delete(Tasks pt)
        {
            _unitOfWork.Repository<Tasks>().Delete(pt);
        }

        public void Update(Tasks pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Tasks>().Update(pt);
        }

        public IEnumerable<Tasks> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Tasks>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.TaskTitle))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<TasksViewModel> GetTasksList(string User,string Status)
        {
            var pt = (from p in db.Tasks
                      where p.ForUser == User && ((Status == TaskStatusConstants.Close || Status == TaskStatusConstants.Complete) ? (p.Status == TaskStatusConstants.Close || p.Status == TaskStatusConstants.Complete) : (p.Status != TaskStatusConstants.Close && p.Status != TaskStatusConstants.Complete))
                      orderby p.Priority descending, p.DueDate ?? DateTime.MaxValue, p.CreatedDate
                      select new TasksViewModel
                      {

                          DueDate = p.DueDate,
                          ForUser = p.CreatedBy,
                          IsActive = p.IsActive,
                          Priority = p.Priority,
                          ProjectId = p.ProjectId,
                          ProjectName = p.Project.ProjectName,
                          Status = p.Status,
                          TaskDescription = p.TaskDescription,
                          TaskId = p.TaskId,
                          TaskTitle = p.TaskTitle,
                      });

            return pt;
        }

        public IQueryable<TasksViewModel> GetOutBoxTasksList(string User,string Status)
        {
            var pt = (from p in db.Tasks
                      where p.CreatedBy == User && User != p.ForUser && ((Status == TaskStatusConstants.Close || Status == TaskStatusConstants.Complete) ? (p.Status == TaskStatusConstants.Close || p.Status == TaskStatusConstants.Complete) : (p.Status != TaskStatusConstants.Close && p.Status != TaskStatusConstants.Complete))
                      orderby p.Priority descending, p.DueDate ?? DateTime.MaxValue, p.CreatedDate                      
                      select new TasksViewModel
                      {
                          DueDate = p.DueDate,
                          ForUser = p.ForUser,
                          IsActive = p.IsActive,
                          Priority = p.Priority,
                          ProjectId = p.ProjectId,
                          ProjectName = p.Project.ProjectName,
                          Status = p.Status,
                          TaskDescription = p.TaskDescription,
                          TaskId = p.TaskId,
                          TaskTitle = p.TaskTitle,
                      });

            return pt;
        }

        public Tasks Add(Tasks pt)
        {
            _unitOfWork.Repository<Tasks>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.Tasks
                        orderby p.TaskTitle
                        select p.TaskId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.Tasks
                        orderby p.TaskTitle
                        select p.TaskId).FirstOrDefault();
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

                temp = (from p in db.Tasks
                        orderby p.TaskTitle
                        select p.TaskId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.Tasks
                        orderby p.TaskTitle
                        select p.TaskId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<Tasks>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Tasks> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
