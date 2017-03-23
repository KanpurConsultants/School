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
    public interface IRateListHistoryService : IDisposable
    {
        RateListHistory Create(RateListHistory pt);
        void Delete(int id);
        void Delete(RateListHistory s);
        RateListHistory Find(int Id);
        void Update(RateListHistory s);
        IEnumerable<RateListHistory> GetRateListHistory();
        int NextId(int id);
        int PrevId(int id);

    }

    public class RateListHistoryService : IRateListHistoryService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        public RateListHistoryService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public RateListHistory Find(int Id)
        {
            return _unitOfWork.Repository<RateListHistory>().Find(Id);
        }

        public RateListHistory Create(RateListHistory s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<RateListHistory>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<RateListHistory>().Delete(id);
        }

        public void Delete(RateListHistory s)
        {
            _unitOfWork.Repository<RateListHistory>().Delete(s);
        }

        public void Update(RateListHistory s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<RateListHistory>().Update(s);
        }


        public IEnumerable<RateListHistory> GetRateListHistory()
        {
            var pt = (from p in db.RateListHistory
                      orderby p.RateListId
                      select p);            

            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.RateListHistory
                        orderby p.RateListId
                        select p.RateListId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.RateListHistory
                        orderby p.RateListId
                        select p.RateListId).FirstOrDefault();
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

                temp = (from p in db.RateListHistory
                        orderby p.RateListId
                        select p.RateListId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.RateListHistory
                        orderby p.RateListId
                        select p.RateListId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public void Dispose()
        {
        }

    }
}
