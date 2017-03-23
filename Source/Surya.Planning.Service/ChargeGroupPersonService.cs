﻿using System.Collections.Generic;
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
    public interface IChargeGroupPersonService : IDisposable
    {
        ChargeGroupPerson Create(ChargeGroupPerson pt);
        void Delete(int id);
        void Delete(ChargeGroupPerson pt);
        ChargeGroupPerson Find(string Name);
        ChargeGroupPerson Find(int id);
        IEnumerable<ChargeGroupPerson> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(ChargeGroupPerson pt);
        ChargeGroupPerson Add(ChargeGroupPerson pt);
        IQueryable<ChargeGroupPerson> GetChargeGroupPersonList(int ChargeTypeId);
        IEnumerable<ChargeGroupPerson> GetChargeGroupPersonList(string chargetypename);        
        Task<IEquatable<ChargeGroupPerson>> GetAsync();
        Task<ChargeGroupPerson> FindAsync(int id);
        int NextId(int id,int ctypeid);
        int PrevId(int id,int ctypeid);
    }

    public class ChargeGroupPersonService : IChargeGroupPersonService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<ChargeGroupPerson> _ChargeGroupPersonRepository;
        RepositoryQuery<ChargeGroupPerson> ChargeGroupPersonRepository;
        public ChargeGroupPersonService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ChargeGroupPersonRepository = new Repository<ChargeGroupPerson>(db);
            ChargeGroupPersonRepository = new RepositoryQuery<ChargeGroupPerson>(_ChargeGroupPersonRepository);
        }

        public ChargeGroupPerson Find(string Name)
        {
            return (from p in db.ChargeGroupPerson
                    where p.ChargeGroupPersonName == Name
                    select p
                        ).FirstOrDefault();
        }

        public ChargeGroupPerson Find(int id)
        {
            return _unitOfWork.Repository<ChargeGroupPerson>().Find(id);
        }

        public ChargeGroupPerson Create(ChargeGroupPerson pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<ChargeGroupPerson>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<ChargeGroupPerson>().Delete(id);
        }

        public void Delete(ChargeGroupPerson pt)
        {
            _unitOfWork.Repository<ChargeGroupPerson>().Delete(pt);
        }

        public void Update(ChargeGroupPerson pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<ChargeGroupPerson>().Update(pt);
        }

        public IEnumerable<ChargeGroupPerson> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<ChargeGroupPerson>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.ChargeGroupPersonName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<ChargeGroupPerson> GetChargeGroupPersonList(int ChargeTypeId)
        {
            var pt = (from p in db.ChargeGroupPerson
                      where p.ChargeTypeId == ChargeTypeId
                          orderby p.ChargeGroupPersonName
                          select p
                          );

            return pt;
        }

        public IEnumerable<ChargeGroupPerson> GetChargeGroupPersonList(string chargetypename)
        {
            var pt = (from p in db.ChargeGroupPerson
                      where p.ChargeType.ChargeTypeName == chargetypename
                      orderby p.ChargeGroupPersonName
                      select p
                          );

            return pt;
        }


        public ChargeGroupPerson Add(ChargeGroupPerson pt)
        {
            _unitOfWork.Repository<ChargeGroupPerson>().Insert(pt);
            return pt;
        }

        public int NextId(int id,int ctypeid)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.ChargeGroupPerson
                        where p.ChargeTypeId==ctypeid
                        orderby p.ChargeGroupPersonName
                        select p.ChargeGroupPersonId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.ChargeGroupPerson
                        where p.ChargeTypeId==ctypeid
                        orderby p.ChargeGroupPersonName
                        select p.ChargeGroupPersonId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id,int ctypeid)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.ChargeGroupPerson
                        where p.ChargeTypeId==ctypeid
                        orderby p.ChargeGroupPersonName
                        select p.ChargeGroupPersonId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.ChargeGroupPerson
                        where p.ChargeTypeId==ctypeid
                        orderby p.ChargeGroupPersonName
                        select p.ChargeGroupPersonId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<ChargeGroupPerson>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ChargeGroupPerson> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
