using static Dextor.API.Models.ViewModel.Dictionary;

namespace Dextor.API.Models.ViewModel
{
    public class Dictionary
    {
        public enum YesNo
        {
            No = 0,
            Yes = 1
        }
        public enum IsActive
        {

            Inactive = 0,
            Active = 1
        }
        public enum OutletEmployeeType
        {
            Manager = 1,
            Executive = 2,
            ShopAssistant = 3
        }
        public enum HrEmployeeStatus
        {
            NotEmployed = 0,
            Contractual = 1,
            Confirmed = 2,
            Suspended = 3,
            Retired = 4,
            Resigned = 5,
            Cancelled = 6,
            Terminated = 7
        }
        /// <summary>
        /// Product Heirarchy
        /// </summary>
        public enum ProductGroupType
        {
            Pg = 1,
            Mag = 2,
            Asg = 3,
            Ag = 4,
            Product = 5
        }

        public enum TicketPriority
        {
            Normal,
            Low,
            High,
            VeryHigh,
            Urgent
        }

        public enum TicketStatus
        {
            Created = 1,
            Resolved = 2,
            Declined = 3
        }

        public enum GV_Redeem_Points
        {
            RetailPOS = 1,
            FranchaisePOS = 2,
            ECOM = 3,
            MarketPlace = 4,
            All = 5
        }


        public enum POStatus
        {
            Created = 0,
            Planned = 1,
            Invoiced = 2,
            Received = 3,
            Canceled = 4,
            Partially_Received = 5,

        }
        public enum CustomerTypeSalesLead
        {
            Retail = 1,
            B2C = 2,
            B2B = 3,
            Hpa = 4,
            Dealer = 5,
            EStore = 6
        }
        public enum LeadConversionPossibility
        {
            Low = 1,
            Medium = 2,
            High = 3
        }

        public enum SalesLeadStatusPos
        {
            Create = 1,
            SendToHo = 2,
            SalesExecuted = 3,
            Cancel = 4,
            InProgress = 5,
            NoAnswer = 6,
            FollowUp = 7,
            ProductNotAvailable = 8
        }
        public enum LeadSource
        {
            General = 0,
            Activation = 1
        }

        public enum SalesType
        {
            Retail = 1,
            B2C = 2,
            B2B = 3,
            Hpa = 4,
            Dealer = 5,
            EStore = 6,
        }
        public enum ProductStockTranSide
        {
            In = 1,
            Out = 2
        }
        public enum ProductStockTranType
        {
            GoodsReceive = 1,
            Transfer = 3,
            Invoice = 5,
            AddStock = 7,
            DeductStock = 8,
            IssueSalesPromotion = 9,
            IssueFixedAsset = 10,
            IssueCeServiceReplacement = 11,
            ReturnDefectiveProduct = 12,
            IssueCompanyConsumption = 13,
            IssueLeServiceReplacenent = 14,
            IssueProductReturnToSupplier = 15,
            IssueForProduction = 16,
            DeliveryBreakageReplacement = 17,
            IssueShortDeliveryProduct = 18,
            IssueScrapSale = 19,
            IssueDefectiveOrScrap = 20,
            ReceiveServiceDefectiveProduct = 21,
            ReturnSalesPromotion = 22,
            ReturnFixedAsset = 23,
            IssueForCannibalize = 24,
            All = -1
        }

        public enum InvoiceType
        {
            Credit = 1,
            Cash = 2,
            Replacement = 3,
            Eps = 4,
            EasyBuy = 5,
            CreditReverse = 6,
            CashReverse = 7,
            ReplacementReverse = 8,
            EpsReverse = 9,
            EasyBuyReverse = 10,
            Reverse = 11,
            ProductReturn = 12,
            IbServiceInvoice = 13,
            CancelInvoice = 14,
            IssueSalesPromotion = 15,
            IssueSalesPromotionReverse = 16,
            BreakageReplacement = 17
        }
        public enum PosInvoiceType
        {
            GeneralInvoice = 0,
            BulkInvoice = 1
        }
        public enum RetailCustomerType
        {
            General = 0,
            Special = 1
        }
        public enum BrandLevel
        {
            MasterBrand = 1,
            SubBrand = 2,
        }
        public enum SecondaryOrderStatus
        {
            Create = 1,
            Invoiced = 2,
            Pending = 3,
            Cancel = 4
        }
        public enum LeaveType
        {
            SickLeave = 0,
            EarnedLeave = 1,
            CasualLeave = 2,
            MaternityLeave = 3,
            PaternityLeave = 4,
            LeaveWithoutPay = 5
        }

        public enum LeaveStatus
        {
            Applied = 1,
            Cancelled = 2,
            Acknowledged = 3,
            ReadyForApproved = 4,
            Approved = 5,
            Reject = 6,

        }
        public enum PartialTypeLeave
        {
            FullDay = 0,
            FirstHalf = 1,
            SecondHalf = 2
        }
        public enum HrOutStationStatus
        {
            Create = 1,
            Approved = 2,
            Reject = 3
        }

        public enum LeaveReason
        {
            Personal,
            Training,
            MarketVisit,
            Others
        }

        public enum LinManagerType
        {
            Leave = 1,
            Assessment = 2,
        }
        public enum ProductSerialStatus
        {
            SendFromHoToOutlet = 0,
            ReceivedAtOutlet = 1,
            SendFromOutletToHo = 2,
            ReceivedAtHo = 3,
            SendFromOutletToOutlet = 4,
            SendFromOutletCsd = 5,
            ReceiveAtCsd = 6,
            SendFromCsdToOutlet = 7,
            SendFromCsdToHo = 8,
            Sold = 9,
            Defective = 10,
            RejectTranByManagement = 11
        }
        public enum ItemCategory
        {
            FinishedProduct = 1,
            RawMaterial = 2,
            SpareParts = 3,
            GiftItem = 4,
            SampleItem = 5,
            AcccessoriesItem = 6
        }
        public enum CreditCardType
        {
            Visa = 1,
            Master = 2,
            Amex = 3,
            Nexus = 4,
            Jcb = 5
        }
        public enum CreditCardCategory
        {
            DebitCard = 1,
            CreditCard = 2
        }
        public enum ServiceType
        {
            Walkin = 1,
            HomeCall = 2,
            Installation = 3
        }
        public enum CsdJobType
        {
            FullWarranty = 1,
            Paid = 2,
            ServiceWarranty = 3,
            ComponentWarranty = 4
        }
        public enum JobStatus
        {
            WalkinJobCreated = 0,
            HomecallJobCreated = 1,
            InstallationJobCreated = 2,
            InterServiceJobCreated = 3,
            AssignedToTechnician = 4,
            Untouched = 5,
            WorkInProgress = 6,
            Estimated = 7,
            EstimateApproved = 8,
            EstimateNotApproved = 9,
            Critical = 10,
            Pending = 11,
            ReadyForTest = 12,
            Repaired = 13,
            Return = 14,
            Replace = 15,
            ReturnFromCustomer = 16,
            ServiceProvided = 17,
            TransportRequired = 18,
            ConvertedFromHomeCall = 19,
            Cancel = 20,

            //Only History Status Start
            ChangeTechnician = 21,
            SentToWorkshop = 22,
            ReceivedAtworkshop = 23,
            SentToFrontDesk = 24,
            ReceivedAtFrontDesk = 25,
            //Only History Status End

            ReadyForDelivery = 26,
            Delivered = 27
        }
        public enum JobStatusSub
        {
            PendingForLocalParts = 1,
            PendingForForeignParts = 2,
            PendingForManagementDecision = 3,
            PendingForCustomerDecision = 4,
            Scheduled = 5,
            ReScheduled = 6
        }
        public enum TechnicianTranspotationStatus
        {
            Create = 1,
            Approved = 2,
            Cancel = 3,
            Reject = 4
        }

        public enum TranspotationMode
        {
            Bus = 1,
            CNG = 2,
            Rickshaw = 3,
            HumanHoller = 4,
            Train = 5,
            MotorCycle = 6,
            Car = 7,
            Launch = 8,
            Microbus = 9,
            Other = 10
        }
        public enum SmsHistoryStatus
        {
            Create = 1,
            Send = 2,
            Problematic = 3,
            Resent = 4,
            Cancel = 5
        }
        public enum AndroidAppId
        {
            CjApps = 1,
            CjDigital = 2,
            CjLighting = 3,
            ECsd = 4,
            EDms = 5,
            ETdFieldForce = 6,
            All = -9
        }
        public enum FfSearchFor
        {
            Self = 1,
            Td = 2,
            Ho = 3
        }
        public enum DayPlanStatus
        {
            Create = 1,
            Approve = 2,
            Reject = 3
        }

        public enum DayPlanDetailsStatus
        {
            Create = 1,
            CheckIn = 2,
            CheckOut = 3,
            NewLeadCreate = 4,
            OldLeadVisit = 5
        }
        public enum DayTrackerFor
        {
            AttendanceCheckIn = 1,
            AttendanceCheckOut = 2,
            DayPlanCheckIn = 3,
            DayPlanCheckOut = 4,
            FreeCheckIn = 5,
            CreateLead = 6
        }

        public enum DayPlanWiseLead
        {
            NewLeadCreate = 1,
            OldLeadVisit = 2
        }

        public enum FfVisitType
        {
            Attendance = 1,
            Other = 2
        }
        public enum DataTransferType
        {
            Add = 1,
            Edit = 2,
            Delete = 3
        }
        public enum IsDownload
        {
            No = 1,
            Yes = 2
        }
        public enum ImageFor
        {
            CsdJob = 1,
            SalesLead = 2,
            DayPlan = 3
        }

        public enum CouponStatus
        {
            Unused = 1,
            Used = 2
        }


    }

    public class GV_Prefix
    {
        public const string R = "RetailPOS";
        public const string F = "FranchaisePOS";
        public const string A = "ECOM";
        public const string M = "MarketPlace";
        public const string AA = "ALL";
    }

    public static class GV_PrefixMap
    {
        public static readonly Dictionary<int, string> Map = new Dictionary<int, string>()
    {
        { (int)GV_Redeem_Points.RetailPOS, GV_Prefix.R },
        { (int)GV_Redeem_Points.FranchaisePOS, GV_Prefix.F },
        { (int)GV_Redeem_Points.ECOM, GV_Prefix.A },
        { (int)GV_Redeem_Points.MarketPlace, GV_Prefix.M },
        { (int)GV_Redeem_Points.All, GV_Prefix.AA}
    };
    }
}
