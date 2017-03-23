﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Core.Common
{
    public enum StatusConstants
    {
        Drafted = 0,
        Submitted = 1,
        Approved = 2,
        Modified = 3,
        ModificationSubmitted = 4,
        Closed = 5,
        NocOk = 6,
        Complete = 7,
        Import = 8,
        Locked = 9,
    }

    public enum SaleOrderPriority
    {
        Low = -10,
        Normal = 0,
        High = 10
    }

    public enum TasksPriority
    {
        Low = -10,
        Normal = 0,
        High = 10
    }

    public enum ProductSizeTypeConstants
    {
        StandardSize = 1,
        ManufacturingSize = 2,
        FinishingSize = 3,
        StencilSize = 4,
        MapSize = 5,
    }
    public enum ProductShapeConstants
    {
        Rectangle = 3,
    }

    //public enum UnitConstants
    //{
    //    BOX = "BOX",
    //    SqCms = "CM2",
    //    Feet = "FT",
    //    SqFeet = "FT2",
    //    KG = "KG",
    //    Liter = "Lit",
    //    METER = "MET",
    //    SqMeter = "MT2",
    //    Nos = "Nos",
    //    PCS = "PCS",
    //    PKT = "PKT",
    //    Roll = "Rol",
    //    Yard = "Yar",
    //    SqYard="YD2"

    //}

    public enum UnitConversionFors
    {
        Standard = 1,
        Binding = 2,
        Gachchai = 3,
        Finishing = 4,
        Manufacturing = 5,
    }



    public enum RateTypeConstants
    {
        Rate = 1,
        Percentage = 2,
        NA = 3,
    }

    public enum BaleNoPatternConstants
    {
        ProductInvoiceGroup = 1,
        SaleOrder = 2,
    }

    public enum NotificationSubjectConstants
    {
        SaleOrderSubmitted = 1,
        SaleOrderApproved = 2,

        PurchaseOrderSubmitted = 3,
        PurchaseOrderApproved = 4,

        PurchaseOrderCancelSubmitted = 5,
        PurchaseOrderCancelApproved = 6,

        PurchaseGoodsReceiptSubmitted = 7,
        PurchaseGoodsReceiptApproved = 8,

        TaskCreated = 9,
    }
}