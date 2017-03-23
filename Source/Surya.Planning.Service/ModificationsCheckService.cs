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
using System.Xml.Linq;
using Surya.India.Model.ViewModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Collections;

namespace Surya.India.Service
{
    public interface IModificationsCheckService : IDisposable
    {
       
    }

    public class ModificationsCheckService : IModificationsCheckService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        
        public ModificationsCheckService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;          
        }

        public ModificationsCheckService()
        {

        }


        public XElement CheckChanges(List<LogTypeViewModel> LogList)
        {
            XElement s = new XElement("Log");

            foreach (var Rec in LogList)
            {
                if (Rec.Obj != null && Rec.ExObj != null)
                {
                    Type T = Rec.ExObj.GetType();
                    Type T2 = Rec.Obj.GetType();

                    var ClassProp = T.GetProperties();
                    var ClassProp2 = T2.GetProperties();
                    var Pk = ClassProp.FirstOrDefault(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)));
                    XElement Name = new XElement(T.Name, Pk!=null? new XElement(Pk.Name, new XElement("Old", Convert.ToString(Pk.GetValue(Rec.ExObj))), new XElement("New", Convert.ToString(Pk.GetValue(Rec.ExObj)))) : new XElement("ViewModel"));

                    foreach (PropertyInfo item in ClassProp)
                    {

                        if (!object.Equals((item.GetValue(Rec.ExObj)), (item.GetValue(Rec.Obj))) && item.Name != "ModifiedBy" && item.Name != "ModifiedDate" && item.Name != "CreatedBy" && item.Name != "CreatedDate" && item.Name != "ObjectState" && !item.GetMethod.IsVirtual && (item.PropertyType.IsGenericType ? item.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>) : 1 == 1))
                        {
                            Name.Add(new XElement(item.Name, new XElement("Old", item.GetValue(Rec.ExObj) == null ? "Null" : (item.PropertyType == typeof(DateTime?)) ? ((DateTime)item.GetValue(Rec.ExObj)).ToString("dd/MMM/yyyy") : item.GetValue(Rec.ExObj)), new XElement("New", item.GetValue(Rec.Obj) == null ? "Null" : (item.PropertyType == typeof(DateTime?) ? ((DateTime)item.GetValue(Rec.Obj)).ToString("dd/MMM/yyyy") : item.GetValue(Rec.Obj)))));
                        }

                    }
                    s.Add(Name);
                }
                else if (Rec.ExObj != null)
                {
                    Type T = Rec.ExObj.GetType();
                    var ClassProp = T.GetProperties();

                    var Pk = ClassProp.FirstOrDefault(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)));
                    XElement Name = new XElement(T.Name, Pk!=null ? new XElement(Pk.Name, Convert.ToString(Pk.GetValue(Rec.ExObj))) : new XElement("ViewModel"));

                    foreach (PropertyInfo item in ClassProp)
                    {

                        if (item.Name != "ObjectState" && !item.GetMethod.IsVirtual && (item.PropertyType.IsGenericType ? item.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>) : 1 == 1))
                        {
                            Name.Add(new XElement(item.Name, item.GetValue(Rec.ExObj) == null ? "Null" : (item.PropertyType == typeof(DateTime?)) ? ((DateTime)item.GetValue(Rec.ExObj)).ToString("dd/MMM/yyyy") : item.GetValue(Rec.ExObj)));
                        }

                    }
                    s.Add(Name);
                }
                else
                {
                    Type T = Rec.Obj.GetType();
                    var ClassProp = T.GetProperties();

                    var Pk = ClassProp.FirstOrDefault(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)));
                    XElement Name = new XElement(T.Name, Pk!=null ? new XElement(Pk.Name, Convert.ToString(Pk.GetValue(Rec.Obj))) : new XElement("ViewModel"));

                    foreach (PropertyInfo item in ClassProp)
                    {

                        if (item.Name != "ObjectState" && !item.GetMethod.IsVirtual && (item.PropertyType.IsGenericType ? item.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>) : 1 == 1))
                        {
                            Name.Add(new XElement(item.Name, item.GetValue(Rec.Obj) == null ? "Null" : (item.PropertyType == typeof(DateTime?)) ? ((DateTime)item.GetValue(Rec.Obj)).ToString("dd/MMM/yyyy") : item.GetValue(Rec.Obj)));
                        }

                    }
                    s.Add(Name);
                }

            }

            return s;
        }

        
        public void Dispose()
        {
        }


     
    }
}
