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
    public interface ISaleOrderSettingsService : IDisposable
    {
        SaleOrderSettings Create(SaleOrderSettings pt);
        void Delete(int id);
        void Delete(SaleOrderSettings s);
        SaleOrderSettings Find(int Id);
        void Update(SaleOrderSettings s);
        IEnumerable<SaleOrderSettings> GetSaleOrderSettingsList();
        SaleOrderSettings GetSaleOrderSettings(int DocTypeId,int SiteId, int? DivisionId);

        SaleOrderSettings GetSaleOrderSettingsForExcelImport(int SiteId, int? DivisionId);

    }

    public class SaleOrderSettingsService : ISaleOrderSettingsService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        public SaleOrderSettingsService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SaleOrderSettings Find(int id)
        {
            return _unitOfWork.Repository<SaleOrderSettings>().Find(id);
        }

        public SaleOrderSettings GetSaleOrderSettings(int DocTypeId ,int SiteId, int? DivisionId)
        {
            return _unitOfWork.Repository<SaleOrderSettings>().Query().Get().Where(m=>m.DivisionId==DivisionId&&m.SiteId==SiteId && m.DocTypeId==DocTypeId).FirstOrDefault();
        }

        public SaleOrderSettings GetSaleOrderSettingsForExcelImport(int SiteId, int? DivisionId)
        {
            return _unitOfWork.Repository<SaleOrderSettings>().Query().Get().Where(m => m.DivisionId == DivisionId && m.SiteId == SiteId ).FirstOrDefault();
        }

        public SaleOrderSettings Create(SaleOrderSettings s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SaleOrderSettings>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SaleOrderSettings>().Delete(id);
        }

        public void Delete(SaleOrderSettings s)
        {
            _unitOfWork.Repository<SaleOrderSettings>().Delete(s);
        }

        public void Update(SaleOrderSettings s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SaleOrderSettings>().Update(s);
        }


        public IEnumerable<SaleOrderSettings> GetSaleOrderSettingsList()
        {
            var pt = _unitOfWork.Repository<SaleOrderSettings>().Query().Get();

            return pt;
        }

      

        public void Dispose()
        {
        }

    }
}
