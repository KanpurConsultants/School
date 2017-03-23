using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Surya.India.Model.Models;
using Surya.India.Model.ViewModels;
using Surya.India.Data.Models;
using Surya.India.Data.Infrastructure;
using Surya.India.Service;
using Surya.India.Web;
using AutoMapper;
using Surya.India.Presentation.ViewModels;
using Surya.India.Presentation;
using Surya.India.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;
using System.Configuration;
using System.IO;
using ImageResizer;
using Surya.India.Model.ViewModel;
using Surya.India.Presentation.Helper;

namespace Surya.India.Web.Controllers
{
    [Authorize]
    public class AdmissionController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IAdmissionService _AdmissionService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public AdmissionController(IAdmissionService AdmissionService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _AdmissionService = AdmissionService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Boms = _AdmissionService.GetAdmissionListForIndex().ToList();
            return View(Boms);
        }

        [HttpGet]
        public JsonResult FacilityIndex(int id)
        {
            var p = (from FEnr in db.Sch_FacilityEnrollment
                     join A in db.Sch_Admission on FEnr.AdmissionId equals A.AdmissionId into AdmissionTable
                     from AdmissionTab in AdmissionTable.DefaultIfEmpty()
                     join FStop in db.Sch_FacilityStop on FEnr.FacilityEnrollmentId equals FStop.FacilityEnrollmentId into FacilityStopTable
                     from FacilityStopTab in FacilityStopTable.DefaultIfEmpty()
                     join F in db.Sch_FacilitySubCategory on FEnr.FacilitySubCategoryId equals F.FacilitySubCategoryId into FacilitySubCategoryTable
                     from FacilitySubCategoryTab in FacilitySubCategoryTable.DefaultIfEmpty()
                     where AdmissionTab.AdmissionId == id
                     select new
                     {
                         FacilityEnrollmentId = FEnr.FacilityEnrollmentId,
                         FacilityName = FacilitySubCategoryTab.Facility.FacilityName,
                         StartDate = FEnr.StartDate.ToString(),
                         EndDate = FacilityStopTab.FacilityStopHeader.DocDate.ToString()
                     }).ToList();


            return Json(p, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult NextPage(int id, string name)//BomId
        {
            var nextId = _AdmissionService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//BomId
        {
            var nextId = _AdmissionService.PrevId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Print()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Report()
        {

            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.DesignConsumption);

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }

        [HttpGet]
        public ActionResult Remove()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }




        public void PrepareViewBag(Sch_AdmissionViewModel svm)
        {
            List<SelectListItem> GenderList = new List<SelectListItem>();
            GenderList.Add(new SelectListItem { Text = "Male", Value = "Male"});
            GenderList.Add(new SelectListItem { Text = "Female", Value = "Female" });
            
            if (svm == null)
                ViewBag.Gender = new SelectList(GenderList, "Value", "Text");
            else
                ViewBag.Gender = new SelectList(GenderList, "Value", "Text", svm.Gender);


            List<SelectListItem> CastCategoryList = new List<SelectListItem>();
            CastCategoryList.Add(new SelectListItem { Text = "General", Value = "General" });
            CastCategoryList.Add(new SelectListItem { Text = "OBC", Value = "OBC" });
            CastCategoryList.Add(new SelectListItem { Text = "SC", Value = "SC" });
            CastCategoryList.Add(new SelectListItem { Text = "ST", Value = "ST" });
            CastCategoryList.Add(new SelectListItem { Text = "Other", Value = "Other" });

            if (svm == null)
                ViewBag.CastCategory = new SelectList(CastCategoryList, "Value", "Text");
            else
                ViewBag.CastCategory = new SelectList(CastCategoryList, "Value", "Text", svm.CastCategory);


            List<SelectListItem> ReligionList = new List<SelectListItem>();
            ReligionList.Add(new SelectListItem { Text = "Hindu", Value = "Hindu" });
            ReligionList.Add(new SelectListItem { Text = "Muslim", Value = "Muslim" });
            ReligionList.Add(new SelectListItem { Text = "Sikh", Value = "Sikh" });
            ReligionList.Add(new SelectListItem { Text = "Christian", Value = "Christian" });
            ReligionList.Add(new SelectListItem { Text = "Other", Value = "Other" });

            if (svm == null)
                ViewBag.Religion = new SelectList(ReligionList, "Value", "Text");
            else
                ViewBag.Religion = new SelectList(ReligionList, "Value", "Text", svm.Religion);
        }



        public ActionResult ChooseType()
        {
            return PartialView("ChooseType");
        }
        [HttpGet]
        public ActionResult CopyFromExisting()
        {
            return PartialView("CopyFromExisting");
        }


        public ActionResult Create()
        {
            Sch_AdmissionViewModel p = new Sch_AdmissionViewModel();
            p.DocDate = DateTime.Now;
            p.DOB = DateTime.Now;
            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".Sch_FacilityEnrollment", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            p.IsActive = true;
            PrepareViewBag(p);
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sch_AdmissionViewModel svm)
        {


            if (ModelState.IsValid)
            {
                
                if (svm.AdmissionId <= 0)
                {
                    if (svm.ExistingStudentId == null || svm.ExistingStudentId == 0)
                    {
                        Person Person = new Person();
                        Person.Name = svm.StudentName;
                        Person.Suffix = svm.Suffix;
                        Person.Code = svm.Code;
                        Person.Mobile = svm.Mobile;
                        Person.Email = svm.EMail;
                        Person.CreatedDate = DateTime.Now;
                        Person.ModifiedDate = DateTime.Now;
                        Person.CreatedBy = User.Identity.Name;
                        Person.ModifiedBy = User.Identity.Name;
                        Person.ObjectState = Model.ObjectState.Added;
                        new PersonService(_unitOfWork).Create(Person);

                        Sch_Student Student = new Sch_Student();
                        Student.FatherName = svm.FatherName;
                        Student.MotherName = svm.MotherName;
                        Student.Gender = svm.Gender;
                        Student.DOB = svm.DOB;
                        Student.CastCategory = svm.CastCategory;
                        Student.Religion = svm.Religion;
                        Student.ObjectState = Model.ObjectState.Added;
                        new StudentService(_unitOfWork).Create(Student);


                        PersonAddress PersonAddress = new PersonAddress();
                        PersonAddress.AddressType = AddressTypeConstants.Work;
                        PersonAddress.Address = svm.Address;
                        PersonAddress.CityId = svm.CityId;
                        PersonAddress.Zipcode = svm.Zipcode;
                        PersonAddress.CreatedDate = DateTime.Now;
                        PersonAddress.ModifiedDate = DateTime.Now;
                        PersonAddress.CreatedBy = User.Identity.Name;
                        PersonAddress.ModifiedBy = User.Identity.Name;
                        PersonAddress.ObjectState = Model.ObjectState.Added;
                        new PersonAddressService(_unitOfWork).Create(PersonAddress);

                        if (svm.GuardianName != null)
                        {
                            Person Guardian = new Person();
                            Guardian.PersonID = -1;
                            Guardian.Suffix = "G" + svm.Suffix;
                            Guardian.Code = "G" + svm.Code;
                            Guardian.Name = svm.GuardianName;
                            Guardian.Mobile = svm.GuardianMobile;
                            Guardian.Email = svm.GuardianEMail;
                            Guardian.CreatedDate = DateTime.Now;
                            Guardian.ModifiedDate = DateTime.Now;
                            Guardian.CreatedBy = User.Identity.Name;
                            Guardian.ModifiedBy = User.Identity.Name;
                            Guardian.ObjectState = Model.ObjectState.Added;
                            new PersonService(_unitOfWork).Create(Guardian);

                            PersonContact PersonContact = new PersonContact();
                            PersonContact.PersonId = Person.PersonID;
                            PersonContact.ContactId = Guardian.PersonID;
                            PersonContact.PersonContactTypeId = new PersonContactTypeService(_unitOfWork).Find("Residence").PersonContactTypeId;
                            PersonContact.CreatedDate = DateTime.Now;
                            PersonContact.ModifiedDate = DateTime.Now;
                            PersonContact.CreatedBy = User.Identity.Name;
                            PersonContact.ModifiedBy = User.Identity.Name;
                            PersonContact.ObjectState = Model.ObjectState.Added;
                            new PersonContactService(_unitOfWork).Create(PersonContact);

                        }
                        svm.ExistingStudentId = Student.PersonID;
                    }



                    Sch_Admission Admission = new Sch_Admission();
                    Admission.DocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Admission).DocumentTypeId;
                    Admission.DocNo = svm.DocNo;
                    Admission.DocDate = svm.DocDate;
                    Admission.StudentId = svm.ExistingStudentId;
                    Admission.AdmissionQuotaId = svm.AdmissionQuotaId;
                    Admission.ClassSectionId = svm.ClassSectionId;
                    Admission.RollNo = svm.RollNo;
                    Admission.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                    Admission.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                    Admission.IsActive = true;
                    Admission.CreatedDate = DateTime.Now;
                    Admission.ModifiedDate = DateTime.Now;
                    Admission.CreatedBy = User.Identity.Name;
                    Admission.ModifiedBy = User.Identity.Name;
                    Admission.ObjectState = Model.ObjectState.Added;
                    _AdmissionService.Create(Admission);


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        PrepareViewBag(svm);
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View(svm);

                    }

                    return RedirectToAction("Edit", new { id = Admission.AdmissionId }).Success("Data saved Successfully");
                }
                else
                {
                    Person Person = new PersonService(_unitOfWork).Find(svm.PersonId);
                    Person.Name = svm.StudentName;
                    Person.Suffix = svm.Suffix;
                    Person.Code = svm.Code;
                    Person.Mobile = svm.Mobile;
                    Person.Email = svm.EMail;
                    Person.ModifiedDate = DateTime.Now;
                    Person.ModifiedBy = User.Identity.Name;
                    new PersonService(_unitOfWork).Update(Person);

                    Sch_Student Student = new StudentService(_unitOfWork).Find(svm.PersonId);
                    Student.Gender = svm.Gender;
                    Student.DOB = svm.DOB;
                    Student.CastCategory = svm.CastCategory;
                    Student.Religion = svm.Religion;
                    Person.ModifiedDate = DateTime.Now;
                    Person.ModifiedBy = User.Identity.Name;
                    new StudentService(_unitOfWork).Update(Student);


                    Sch_Admission Admission = _AdmissionService.Find(svm.AdmissionId);
                    Admission.DocNo = svm.DocNo;
                    Admission.DocDate = svm.DocDate;
                    Admission.AdmissionQuotaId = svm.AdmissionQuotaId;
                    Admission.ClassSectionId = svm.ClassSectionId;
                    Admission.RollNo = svm.RollNo;
                    Admission.ModifiedBy = User.Identity.Name;
                    Admission.ModifiedDate = DateTime.Now;
                    _AdmissionService.Update(Admission);


                    PersonAddress PersonAddress = (from P in db.PersonAddress where P.PersonId == svm.PersonId select P).FirstOrDefault();
                    if (PersonAddress != null)
                    {
                        PersonAddress.AddressType = AddressTypeConstants.Work;
                        PersonAddress.Address = svm.Address;
                        PersonAddress.CityId = svm.CityId;
                        PersonAddress.Zipcode = svm.Zipcode;
                        PersonAddress.ModifiedDate = DateTime.Now;
                        PersonAddress.ModifiedBy = User.Identity.Name;
                        new PersonAddressService(_unitOfWork).Update(PersonAddress);
                    }



                    Person Guardian = (from Pc in db.PersonContacts  
                                       join P in db.Persons on Pc.ContactId equals P.PersonID into PersonTable from PersonTab in PersonTable.DefaultIfEmpty()
                                       where Pc.PersonId == svm.PersonId select PersonTab).FirstOrDefault();
                    if (Guardian != null)
                    {
                        Guardian.Name = svm.GuardianName;
                        Guardian.Mobile = svm.GuardianMobile;
                        Guardian.Email = svm.GuardianEMail;
                        Guardian.ModifiedDate = DateTime.Now;
                        Guardian.ModifiedBy = User.Identity.Name;
                        new PersonService(_unitOfWork).Update(Guardian);
                    }
                    else
                    {
                        if (svm.GuardianName != null)
                        {
                            Person NewGuardian = new Person();
                            NewGuardian.PersonID = -1;
                            NewGuardian.Suffix = "G" + svm.Suffix;
                            NewGuardian.Code = "G" + svm.Code;
                            NewGuardian.Name = svm.GuardianName;
                            NewGuardian.Mobile = svm.Mobile;
                            NewGuardian.Email = svm.EMail;
                            NewGuardian.CreatedDate = DateTime.Now;
                            NewGuardian.ModifiedDate = DateTime.Now;
                            NewGuardian.CreatedBy = User.Identity.Name;
                            NewGuardian.ModifiedBy = User.Identity.Name;
                            NewGuardian.ObjectState = Model.ObjectState.Added;
                            new PersonService(_unitOfWork).Create(NewGuardian);

                            PersonContact PersonContact = new PersonContact();
                            PersonContact.PersonId = Person.PersonID;
                            PersonContact.ContactId = NewGuardian.PersonID;
                            PersonContact.PersonContactTypeId = new PersonContactTypeService(_unitOfWork).Find("Residence").PersonContactTypeId;
                            PersonContact.CreatedDate = DateTime.Now;
                            PersonContact.ModifiedDate = DateTime.Now;
                            PersonContact.CreatedBy = User.Identity.Name;
                            PersonContact.ModifiedBy = User.Identity.Name;
                            PersonContact.ObjectState = Model.ObjectState.Added;
                            new PersonContactService(_unitOfWork).Create(PersonContact);
                        }
                    }

                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = svm.AdmissionId,
                        Narration = "",
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Admission).DocumentTypeId,

                    };
                    new ActivityLogService(_unitOfWork).Create(al);
                    //End of Saving ActivityLog

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        PrepareViewBag(svm);
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", svm);
                    }
                    return RedirectToAction("Index").Success("Data saved successfully");
                }
            }
            PrepareViewBag(svm);
            return View(svm);
        }


        public ActionResult Edit(int id)
        {
            Sch_AdmissionViewModel bvm = _AdmissionService.GetAdmissionForEdit(id);
            PrepareViewBag(bvm);
            if (bvm == null)
            {
                return HttpNotFound();
            }
            return View("Create", bvm);
        }


        // GET: /ProductMaster/Delete/5

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            
            Sch_Admission Admission = _AdmissionService.Find(id);
            if (Admission == null)
            {
                return HttpNotFound();
            }

            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Sch_Admission Admission = _AdmissionService.Find(vm.id);
                Person Person = new PersonService(_unitOfWork).Find(Admission.StudentId);
                Sch_Student Studemt = new StudentService(_unitOfWork).Find(Admission.StudentId);
                PersonAddress PersonAddress = (from P in db.PersonAddress where P.PersonId == Person.PersonID select P).FirstOrDefault();
                PersonContact PersonContact = (from P in db.PersonContacts where P.PersonId == Person.PersonID select P).FirstOrDefault();
                Person Guardian = new PersonService(_unitOfWork).Find(PersonContact.ContactId);


                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = vm.Reason,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Admission).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);


                //IEnumerable<Sch_AdmissionLine> AdmissionLine = new AdmissionLineService(_unitOfWork).GetAdmissionLineList(vm.id);
                ////Mark ObjectState.Delete to all the Bom Detail For Above Bom. 
                //foreach (Sch_AdmissionLine item in AdmissionLine)
                //{
                //    new AdmissionLineService(_unitOfWork).Delete(item.AdmissionLineId);
                //}

                new PersonContactService(_unitOfWork).Delete(PersonContact);
                new PersonAddressService(_unitOfWork).Delete(PersonAddress);
                _AdmissionService.Delete(vm.id);
                new StudentService(_unitOfWork).Delete(Studemt);
                new PersonService(_unitOfWork).Delete(Guardian);
                new PersonService(_unitOfWork).Delete(Person);

                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return PartialView("_Reason", vm);
                }
                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }

        public JsonResult GetStudentDetailJson(int StudentId)
        {
            Sch_StudentViewModel StudentDetail = new StudentService(_unitOfWork).GetStudentDetail(StudentId);

            List<Sch_StudentViewModel> StudentDetailJson = new List<Sch_StudentViewModel>();

            if (StudentDetail != null)
            {
                StudentDetailJson.Add(new Sch_StudentViewModel()
                {
                    Name = StudentDetail.Name,
                    Code = StudentDetail.Code,
                    Suffix = StudentDetail.Suffix,
                    Gender = StudentDetail.Gender,
                    DOB = StudentDetail.DOB,
                    Religion = StudentDetail.Religion,
                    CastCategory = StudentDetail.CastCategory,
                    Address = StudentDetail.Address,
                    CityId = StudentDetail.CityId,
                    CityName = StudentDetail.CityName,
                    Zipcode = StudentDetail.Zipcode,
                    Mobile = StudentDetail.Mobile,
                    Email = StudentDetail.Email,
                    FatherName = StudentDetail.FatherName,
                    MotherName = StudentDetail.MotherName,
                    GuardianName = StudentDetail.GuardianName,
                    GuardianMobile = StudentDetail.GuardianMobile,
                    GuardianEMail = StudentDetail.GuardianEMail
                });
            }


            return Json(StudentDetailJson);
        }

      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
