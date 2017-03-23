﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Surya.India.Model.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Surya.India.Data;
using Surya.India.Data.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;
using System.Data.SqlClient;

namespace Surya.India.Data.Models
{
    public class TargetAchievement
    {
        public string Head { get; set; }
        public decimal Target { get; set; }
        public decimal Acheived { get; set; }
    }

    public class PurchaseOrderBalanceTimeLineRisk
    {
        public string Head { get; set; }
        public decimal DeepRisk { get; set; }
        public decimal OnRisk { get; set; }
        public decimal OnTime { get; set; }
    }
    public class PurchaseOrderBalanceCityTimeLine
    {
        public string CityName { get; set; }
        public decimal DeepRisk { get; set; }
        public decimal OnRisk { get; set; }
        public decimal OnTime { get; set; }
    }
    public interface IPlanningStoredProcedures
    {
        IEnumerable<TargetAchievement> DashBoardTargetAchievement();
        IEnumerable<TargetAchievement> DashBoardTargetAchievementPercentage();
        IEnumerable<PurchaseOrderBalanceTimeLineRisk> DashBoardPurchaseOrderBalanceTimeLineRisk();
        IEnumerable<PurchaseOrderBalanceTimeLineRisk> DashBoardPurchaseOrderBalanceTimeLineRiskPercentage();
        IEnumerable<PurchaseOrderBalanceCityTimeLine> DashBoardPurchaseOrderBalanceCityTimeLineMonthly();
        IEnumerable<PurchaseOrderBalanceCityTimeLine> DashBoardPurchaseOrderBalanceCityTimeLineQarterly();
    }

    public partial class ApplicationDbContext : IPlanningStoredProcedures
    {
        public IEnumerable<TargetAchievement> DashBoardTargetAchievement()
        {
            return this.Database.SqlQuery<TargetAchievement>("sp_ViewManagerDashBoardTargetAchievement");
        }

        public IEnumerable<TargetAchievement> DashBoardTargetAchievementPercentage()
        {
            return this.Database.SqlQuery<TargetAchievement>("sp_ViewManagerDashBoardTargetAchievementPercentage");
        }
        public IEnumerable<PurchaseOrderBalanceTimeLineRisk> DashBoardPurchaseOrderBalanceTimeLineRisk()
        {
            return this.Database.SqlQuery<PurchaseOrderBalanceTimeLineRisk>("sp_ViewManagerDashBoardPurchaseOrderBalanceTimeLineRisk");
        }
        public IEnumerable<PurchaseOrderBalanceTimeLineRisk> DashBoardPurchaseOrderBalanceTimeLineRiskPercentage()
        {
            return this.Database.SqlQuery<PurchaseOrderBalanceTimeLineRisk>("sp_ViewManagerDashBoardPurchaseOrderBalanceTimeLineRiskPercentage");
        }
        public IEnumerable<PurchaseOrderBalanceCityTimeLine> DashBoardPurchaseOrderBalanceCityTimeLineMonthly()
        {
            return this.Database.SqlQuery<PurchaseOrderBalanceCityTimeLine>("sp_ViewManagerDashBoardPurchaseOrderBalanceCityTimeLineMonthly");
        }
        public IEnumerable<PurchaseOrderBalanceCityTimeLine> DashBoardPurchaseOrderBalanceCityTimeLineQarterly()
        {
            return this.Database.SqlQuery<PurchaseOrderBalanceCityTimeLine>("sp_ViewManagerDashBoardPurchaseOrderBalanceCityTimeLineQarterly");
        }
    }
   
}