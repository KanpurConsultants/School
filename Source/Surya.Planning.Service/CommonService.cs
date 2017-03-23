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
using System.Data.SqlClient;
using System.Configuration;

namespace Surya.India.Service
{
    public interface ICommonService : IDisposable
    {
        void ExecuteCustomiseEvents(string EventName, object[] Parameters);
    }

    public class CommonService : ICommonService
    {
        public void ExecuteCustomiseEvents(string EventName, object[] Parameters)
        {
            if (EventName != null)
            {
                string[] FunctionPartArr;
                FunctionPartArr = EventName.Split(new Char[] { '.' });

                string NameSpace = FunctionPartArr[0];
                string ClassName = FunctionPartArr[1];
                string FunctionName = FunctionPartArr[2];

                object obj = (object)Activator.CreateInstance("CustomiseEvents", NameSpace + "." + ClassName).Unwrap();
                Type T = obj.GetType();
                T.GetMethod(FunctionName).Invoke(obj, Parameters);
            }
        }

        public void Dispose()
        {
        }
    }
}
