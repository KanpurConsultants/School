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
using Surya.India.Model.ViewModel;

namespace Surya.India.Service
{
    public interface IMaterialPlanHeaderService : IDisposable
    {
        MaterialPlanHeader Create(MaterialPlanHeader pt);
        void Delete(int id);
        void Delete(MaterialPlanHeader pt);
        MaterialPlanHeader Find(int id);
        IEnumerable<MaterialPlanHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(MaterialPlanHeader pt);
        MaterialPlanHeader Add(MaterialPlanHeader pt);
        IQueryable<MaterialPlanHeaderViewModel> GetMaterialPlanHeaderList(int DocTypeId);        
        Task<IEquatable<MaterialPlanHeader>> GetAsync();
        Task<MaterialPlanHeader> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
        string GetMaxDocNo();
    }

    public class MaterialPlanHeaderService : IMaterialPlanHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<MaterialPlanHeader> _MaterialPlanHeaderRepository;
        RepositoryQuery<MaterialPlanHeader> MaterialPlanHeaderRepository;
        public MaterialPlanHeaderService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _MaterialPlanHeaderRepository = new Repository<MaterialPlanHeader>(db);
            MaterialPlanHeaderRepository = new RepositoryQuery<MaterialPlanHeader>(_MaterialPlanHeaderRepository);
        }


        public MaterialPlanHeader Find(int id)
        {
            return _unitOfWork.Repository<MaterialPlanHeader>().Find(id);
        }

        public MaterialPlanHeader Create(MaterialPlanHeader pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<MaterialPlanHeader>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<MaterialPlanHeader>().Delete(id);
        }

        public void Delete(MaterialPlanHeader pt)
        {
            _unitOfWork.Repository<MaterialPlanHeader>().Delete(pt);
        }

        public void Update(MaterialPlanHeader pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<MaterialPlanHeader>().Update(pt);
        }

        public IEnumerable<MaterialPlanHeader> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<MaterialPlanHeader>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DocNo))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IQueryable<MaterialPlanHeaderViewModel> GetMaterialPlanHeaderList(int DocTypeId)
        {

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            return (from p in db.MaterialPlanHeader
                    orderby p.DocDate descending, p.DocNo descending
                    where p.DocTypeId==DocTypeId && p.SiteId==SiteId && p.DivisionId==DivisionId
                    select new MaterialPlanHeaderViewModel
                    {
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        DocTypeName = p.DocType.DocumentTypeName,
                        DueDate = p.DueDate,
                        MaterialPlanHeaderId = p.MaterialPlanHeaderId,
                        Remark = p.Remark,
                        Status = p.Status
                    }
                        );


        }

        public MaterialPlanHeader Add(MaterialPlanHeader pt)
        {
            _unitOfWork.Repository<MaterialPlanHeader>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.MaterialPlanHeader
                        orderby p.DocDate descending,p.DocNo descending
                        select p.MaterialPlanHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.MaterialPlanHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.MaterialPlanHeaderId).FirstOrDefault();
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

                temp = (from p in db.MaterialPlanHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.MaterialPlanHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.MaterialPlanHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.MaterialPlanHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<MaterialPlanHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<MaterialPlanHeader>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MaterialPlanHeader> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
