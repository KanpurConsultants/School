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
using Microsoft.AspNet.Identity.EntityFramework;

namespace Surya.India.Service
{
    public interface IUserService : IDisposable
    {
        IEnumerable<IdentityUser> GetUserList(string[] RoleNameStr);
    }

    public class UserService : IUserService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<IdentityUser> GetUserList(string[] RoleNameStr)
        {
            IEnumerable<IdentityUser> temp = from H in db.Users
                                             join L in db.AspNetUserRole on H.Id equals L.UserId into UserRoleTable
                                             from UserRoleTab in UserRoleTable.DefaultIfEmpty()
                                             join R in db.AspNetRole on UserRoleTab.RoleId equals R.Id into RoleTable
                                             from RoleTab in RoleTable.DefaultIfEmpty()
                                             where RoleNameStr.Contains(RoleTab.Name)
                                             select H;
            if (temp != null)
            {
                return temp.ToList();
            }
            else
            {
                return null;
            }
        }


        public void Dispose()
        {
        }
    }
}
