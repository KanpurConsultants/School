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
    public interface ITrialBalanceSettingService : IDisposable
    {
        TrialBalanceSetting Create(TrialBalanceSetting pt);
        void Delete(int id);
        void Delete(TrialBalanceSetting pt);
        TrialBalanceSetting Find(int id);      
        void Update(TrialBalanceSetting pt);
        TrialBalanceSetting Add(TrialBalanceSetting pt);
        TrialBalanceSetting GetTrailBalanceSetting(string UserName);
    }

    public class TrialBalanceSettingService : ITrialBalanceSettingService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<TrialBalanceSetting> _TrialBalanceSettingRepository;
        RepositoryQuery<TrialBalanceSetting> TrialBalanceSettingRepository;
        public TrialBalanceSettingService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _TrialBalanceSettingRepository = new Repository<TrialBalanceSetting>(db);
            TrialBalanceSettingRepository = new RepositoryQuery<TrialBalanceSetting>(_TrialBalanceSettingRepository);
        }

        public TrialBalanceSetting Find(int id)
        {
            return _unitOfWork.Repository<TrialBalanceSetting>().Find(id);
        }

        public TrialBalanceSetting Create(TrialBalanceSetting pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<TrialBalanceSetting>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<TrialBalanceSetting>().Delete(id);
        }

        public void Delete(TrialBalanceSetting pt)
        {
            _unitOfWork.Repository<TrialBalanceSetting>().Delete(pt);
        }

        public void Update(TrialBalanceSetting pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<TrialBalanceSetting>().Update(pt);
        }

        public TrialBalanceSetting Add(TrialBalanceSetting pt)
        {
            _unitOfWork.Repository<TrialBalanceSetting>().Insert(pt);
            return pt;
        }

        public TrialBalanceSetting GetTrailBalanceSetting(string UserName)
        {

            return (from p in db.TrialBalanceSetting
                    where p.UserName == UserName
                    select p
                        ).FirstOrDefault();

        }

        public void Dispose()
        {
        }

    }
}
