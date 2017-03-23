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
    public interface IModuleService : IDisposable
    {
        MenuModule Create(MenuModule pt);
        void Delete(int id);
        void Delete(MenuModule pt);
        MenuModule Find(string Name);
        MenuModule Find(int id);      
        void Update(MenuModule pt);
        MenuModule Add(MenuModule pt);
        IEnumerable<MenuModule> GetModuleList();
        IEnumerable<MenuModule> GetModuleListForUser(List<string> Roles, int SiteId, int DivisionId);
        MenuModule GetModuleByName(string terms);
    }

    public class ModuleService : IModuleService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<MenuModule> _ModuleRepository;
        RepositoryQuery<MenuModule> ModuleRepository;
        public ModuleService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ModuleRepository = new Repository<MenuModule>(db);
            ModuleRepository = new RepositoryQuery<MenuModule>(_ModuleRepository);
        }
        public MenuModule GetModuleByName(string terms)
        {
            return (from p in db.MenuModule
                    where p.ModuleName == terms
                    select p).FirstOrDefault();
        }

        public MenuModule Find(string Name)
        {
            return ModuleRepository.Get().Where(i => i.ModuleName == Name).FirstOrDefault();
        }

        //public Module GetModuleByRoles()
        //{

           

        //}


        public MenuModule Find(int id)
        {
            return _unitOfWork.Repository<MenuModule>().Find(id);
        }

        public MenuModule Create(MenuModule pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<MenuModule>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<MenuModule>().Delete(id);
        }

        public void Delete(MenuModule pt)
        {
            _unitOfWork.Repository<MenuModule>().Delete(pt);
        }

        public void Update(MenuModule pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<MenuModule>().Update(pt);
        }

        public IEnumerable<MenuModule> GetModuleList()
        {
            var pt = (from p in db.MenuModule
                      orderby p.Srl
                      select p
                          );

            return pt;
        }

        public IEnumerable<MenuModule> GetModuleListForUser(List<string> Role,int SiteId,int DivisionId)
        {

            //Testing Block

            var Roles = (from p in db.Roles
                         select p).ToList();

            var RoleId = string.Join(",", from p in Roles
                                          where Role.Contains(p.Name)
                                          select p.Id.ToString());
            //End


            var pt = (from  p in db.RolesMenu
                      join t in db.Menu on p.MenuId equals t.MenuId
                      join t2 in db.MenuModule on t.ModuleId equals t2.ModuleId
                      where p.SiteId==SiteId && p.DivisionId==DivisionId && RoleId.Contains(p.RoleId)
                      group t2 by t2.ModuleId into g
                      select g.FirstOrDefault()
                          );

            return pt;
        }

        public MenuModule Add(MenuModule pt)
        {
            _unitOfWork.Repository<MenuModule>().Insert(pt);
            return pt;
        }

        public void Dispose()
        {
        }

    }
}
