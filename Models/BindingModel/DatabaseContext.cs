using Microsoft.EntityFrameworkCore; // The modern EF Core namespace
using Dextor.API.Models.Core;

namespace Dextor.API.Models.BindingModel
{
    // 1. POS DATABASE CONTEXT
    public class POSDBContext : DbContext
    {
        public POSDBContext(DbContextOptions<POSDBContext> options) : base(options)
        {
            // ProxyCreation and LazyLoading are false by default in EF Core.
        }

        //public DbSet<POS.Customer> POSCustomers { get; set; }
        //public DbSet<POSChallan> POSChallans { get; set; }
        //public DbSet<SalesInvoiceDiscountChargeCoupon> SalesInvoiceDiscountChargeCoupons { get; set; }
    }

    // 2. DATA WAREHOUSE CONTEXT
    public class DWDBContext : DbContext
    {
        public DWDBContext(DbContextOptions<DWDBContext> options) : base(options)
        {

        }

        //    // public DbSet<POS.Customer> POSCustomers { get; set; }
    }

    // 3. ORACLE CONTEXT (Removed the old EF6 [DbConfigurationType])
    public class OracleDBContext : DbContext
    {
        public OracleDBContext(DbContextOptions<OracleDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EF Core 8 Pluralization Fix (Bulletproof Method)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType != null)
                {
                    // Directly write the table name to the EF Core metadata
                    // This bypasses the need for ToTable() or SetTableName() entirely
                    entityType.SetAnnotation("Relational:TableName", entityType.ClrType.Name);
                }
            }

            // ... your precision configurations (HasPrecision, etc.) go below ...
        }

        // Define your Oracle tables here
        // public DbSet<OracleCustomer> OracleCustomers { get; set; }
    }

    // 4. MAIN WEB DATABASE CONTEXT
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Changed from DbModelBuilder
        {
            base.OnModelCreating(modelBuilder);

            // Precision works exactly the same in EF Core!
            //modelBuilder.Entity<Dextor.API.iclass.ApiClient.Models.TestLab.ParameterCostRate>().Property(p => p.ParameterPrice).HasPrecision(10, 2);
            //modelBuilder.Entity<icures.iclass.ApiClient.Models.TestLab.ParameterMethod>().Property(p => p.MinValue).HasPrecision(10, 2);
            //modelBuilder.Entity<icures.iclass.ApiClient.Models.TestLab.ParameterMethod>().Property(p => p.MaxValue).HasPrecision(10, 2);
            //modelBuilder.Entity<icures.iclass.ApiClient.Models.TestLab.ParameterMethod>().Property(p => p.FixedValue).HasPrecision(10, 2);
            //modelBuilder.Entity<icures.iclass.ApiClient.Models.TestLab.ParameterMethod>().Property(p => p.StandardValue).HasPrecision(10, 2);
        }

        // --- ALL YOUR DB SETS REMAIN EXACTLY THE SAME ---
        //public DbSet<icures.iclass.ApiClient.Models.TestLab.ApplicableMaterial> TestLabMaterials { get; set; }
        //public DbSet<icures.iclass.ApiClient.Models.TestLab.ParameterDirection> TestLabDirections { get; set; }
        //public DbSet<icures.iclass.ApiClient.Models.TestLab.ParameterCostRate> TestLabParameterCosts { get; set; }
        //public DbSet<icures.iclass.ApiClient.Models.TestLab.ParameterMethod> TestLabParameterMethods { get; set; }

        //public DbSet<PurchaseOrderERPSyncLog> PurchaseOrderERPSyncLogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRegistration> UserRegistration { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }



        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<EmployeeLineManager> EmployeeLineManagers { get; set; }
        //public DbSet<EmployeesBasicDataGeneric> EmployeesBasicDataGeniric { get; set; }
        //public DbSet<UserRole> UserRole { get; set; }
        //public DbSet<Role> Role { get; set; }
        //public DbSet<Core.Basic.Customer> Customers { get; set; }
        //public DbSet<UserPermission> UserPermissions { get; set; }
        //public DbSet<ReportLog> ReportLog { get; set; }
        //public DbSet<CoASetup> CoASetup { get; set; }
        //public DbSet<CoATierConfig> CoATierConfig { get; set; }
        //public DbSet<AccJournalTransaction> AccJournalTransaction { get; set; }
        //public DbSet<AccJournalTransactionDetail> AccJournalTransactionDetail { get; set; }
        //public DbSet<AccGroup> AccGroup { get; set; }
        //public DbSet<AccCostProfitCenter> AccCostProfitCenter { get; set; }
        //public DbSet<AccChartOfAccount> AccChartOfAccount { get; set; }
        //public DbSet<AccFiscalYearPeriodSetup> AccFiscalYearPeriodSetup { get; set; }
        //public DbSet<AccFiscalYearPeriodSetupDetail> AccFiscalYearPeriodSetupDetail { get; set; }
        //public DbSet<ProductItemCategory> ProductItemCategory { get; set; }
        //public DbSet<ProductType> ProductType { get; set; }
        //public DbSet<ProductVATType> ProductVATType { get; set; }
        //public DbSet<ProductSupplyType> ProductSupplyType { get; set; }
        //public DbSet<ProductInventoryCategory> ProductInventoryCategory { get; set; }
        //public DbSet<HSCodeInfo> HSCode { get; set; }
        //public DbSet<Brand> Brands { get; set; }
        //public DbSet<ProductGroupType> ProductGroupTypes { get; set; }
        //public DbSet<ProductGroup> ProductGroups { get; set; }
        //public DbSet<Product> Products { get; set; }
        //public DbSet<Bank> Banks { get; set; }
        //public DbSet<BankAccount> BankAccounts { get; set; }
        //public DbSet<BankBranch> BankBranchs { get; set; }
        //public DbSet<Department> Departments { get; set; }
        //public DbSet<B2BCustomerOrders> B2BCustomerOrders { get; set; }
        //public DbSet<B2BMboCustomer> B2BMboCustomers { get; set; }
        //public DbSet<B2BSalesInfos> B2BSalesInfos { get; set; }
        //public DbSet<Designation> Designations { get; set; }
        //public DbSet<StkReoderDeliverySchedule> StkReoderDeliverySchedules { get; set; }
        //public DbSet<StkReoderInventoryPlanning> StkReoderInventoryPlannings { get; set; }
        //public DbSet<JobGrade> JobGrades { get; set; }
        //public DbSet<JobGradeGroup> JobGradeGroups { get; set; }
        //public DbSet<CustomerType> CustomerTypes { get; set; }
        //public DbSet<MarketGroup> MarketGroups { get; set; }
        //public DbSet<SBU> SBUs { get; set; }
        //public DbSet<SalesPromoType> SalesPromoTypes { get; set; }
        //public DbSet<Core.Basic.IPAddress> IPAddress { get; set; }
        //public DbSet<JobLocation> JobLocations { get; set; }
        //public DbSet<Site> Sites { get; set; }
        //public DbSet<ProjectTask> ProjectTasks { get; set; }
        //public DbSet<ProjectTaskHistory> ProjectTaskHistorys { get; set; }
        //public DbSet<ProjectTaskNotification> ProjectTaskNotifications { get; set; }
        //public DbSet<HRShift> HRShifts { get; set; }
        //public DbSet<Channel> Channels { get; set; }
        public DbSet<FirebaseTokenByUser> FirebaseTokenByUsers { get; set; }
        //public DbSet<HRPosition> HRPositions { get; set; }
        //public DbSet<HRCalander> HRCalanders { get; set; }
        //public DbSet<HRCalanderDetail> HRCalanderDetails { get; set; }
        //public DbSet<CanteenMealMenu> CanteenMealMenus { get; set; }
        //public DbSet<Attendance> Attendances { get; set; }
        //public DbSet<Asset> Assets { get; set; }
        //public DbSet<AssetGroup> AssetGroups { get; set; }
        //public DbSet<AssetType> AssetTypes { get; set; }
        //public DbSet<AssetTran> AssetTrans { get; set; }
        //public DbSet<AssetTranItem> AssetTranItems { get; set; }
        //public DbSet<AssetStock> AssetStocks { get; set; }
        //public DbSet<CanteenMealActivation> CanteenMealActivations { get; set; }
        //public DbSet<ProductPrice> ProductPrices { get; set; }
        //public DbSet<CanteenMealRate> CanteenMealRates { get; set; }
        //public DbSet<FieldActivityTrack> FieldActivityTracks { get; set; }
        //public DbSet<FieldActivityLocationData> LocationData { get; set; }
        //public DbSet<Leave> Leave { get; set; }
        //public DbSet<EmployeeLeaveApproved> LeaveApproved { get; set; }
        //public DbSet<LeaveType> LeaveType { get; set; }
        //public DbSet<HRShiftMapping> HRShiftMappings { get; set; }
        //public DbSet<SalesPromo> SalesPromos { get; set; }
        //public DbSet<SalesPromoCustomerMapping> SalesPromoCustomerMappings { get; set; }
        //public DbSet<SalesPromoProductMapping> SalesPromoProductMappings { get; set; }
        //public DbSet<SalesPromoSiteMapping> SalesPromoSiteMappings { get; set; }
        //public DbSet<SalesPromoSlab> SalesPromoSlabs { get; set; }
        //public DbSet<SalesPromoSlabRatio> SalesPromoSlabRatios { get; set; }
        //public DbSet<SalesPromoOffer> SalesPromoOffers { get; set; }
        //public DbSet<SalesPromoOfferDetail> SalesPromoOfferDetails { get; set; }
        //public DbSet<Ticket> Tickets { get; set; }
        //public DbSet<TicketHistory> TicketHistories { get; set; }
        //public DbSet<TicketCategory> TicketCategories { get; set; }
        //public DbSet<TicketSource> TicketSources { get; set; }
        //public DbSet<TicketFiles> TicketFiles { get; set; }
        //public DbSet<CXTicketActualCategory> CXTicketActualCategorys { get; set; }
        //public DbSet<DCS> DCS { get; set; }
        //public DbSet<DCSDetail> DCSDetailList { get; set; }
        //public DbSet<SpecialDiscount> SpecialDiscount { get; set; }
        //public DbSet<SpecialDiscountRedeem> SpecialDiscountRedeemList { get; set; }
        //public DbSet<UserPOSTerminalMapping> UserPOSTerminalMappings { get; set; }
        //public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        //public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        //public DbSet<PurchaseOrderLotBatchItem> PurchaseOrderLotBatchItems { get; set; }
        //public DbSet<PurchaseOrderReceiptHistory> PurchaseOrderReceiptHistorys { get; set; }
        //public DbSet<PurchaseOrderReceiptHistory_Temp> PurchaseOrderReceiptHistory_Temps { get; set; }
        //public DbSet<ProvisionType> ProvisionTypes { get; set; }
        //public DbSet<Provision> Provisions { get; set; }
        //public DbSet<SalesOrder> SalesOrders { get; set; }
        //public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        //public DbSet<ProductImage> ProductImages { get; set; }
        //public DbSet<PaymentType> PaymentTypes { get; set; }
        //public DbSet<PaymentTypeMapping> PaymentTypeMappings { get; set; }
        //public DbSet<VoucherCategory> VoucherCategorys { get; set; }
        //public DbSet<PaymentDiscountCoupon> PaymentDiscountCoupons { get; set; }
        //public DbSet<PaymentDiscountCouponHeader> PaymentDiscountCouponHeaders { get; set; }
        //public DbSet<PaymentDiscountCouponItem> PaymentDiscountCouponItems { get; set; }
        //public DbSet<PaymentDiscountCouponPay> PaymentDiscountCouponPays { get; set; }
        //public DbSet<PaymentDiscountCouponSeller> PaymentDiscountCouponSellers { get; set; }
        //public DbSet<PaymentDiscountCouponSMSLog> PaymentDiscountCouponSMSLogs { get; set; }
        //public DbSet<CustomerBalance> CustomerBalances { get; set; }
        //public DbSet<MRPMaterialReadiness> MRPMaterialReadiness { get; set; }
        //public DbSet<HRRight> HRRights { get; set; }
        //public DbSet<HR_EMPLOYEE_RIGHT_FILES> HR_EMPLOYEE_RIGHT_FILESs { get; set; }
        //public DbSet<PurchaseOrderERPSync> PurchaseOrderERPSyncs { get; set; }
        //public DbSet<DemandScheduling> DemandSchedulings { get; set; }
        //public DbSet<CXSolutionType> CXSolutionTypes { get; set; }
        //public DbSet<HREmployeeTravel> HREmployeeTravels { get; set; }
        //public DbSet<DayWiseProductStock> DayWiseProductStocks { get; set; }
        //public DbSet<EmployeeEducation> EmployeeEducations { get; set; }
        //public DbSet<EmpWorkExperience> EmpWorkExperiences { get; set; }
        //public DbSet<EmployeeTraining> EmployeeTrainings { get; set; }
        //public DbSet<ApparelTarget> ApparelTargets { get; set; }
        //public DbSet<EmpTarget> EmpTargets { get; set; }
        //public DbSet<FootwearTarget> FootwearTargets { get; set; }
        //public DbSet<IBTarget> IBTargets { get; set; }
        //public DbSet<CurentStock> CurentStocks { get; set; }
        //public DbSet<POSReceiveConfirmLog> POSReceiveConfirmLogs { get; set; }
        //public DbSet<PMSMaster> PMSMaster { get; set; }
        //public DbSet<PMSConfiguration> PMSConfiguration { get; set; }
        //public DbSet<PMSDevelopmentPlan> PMSDevelopmentPlan { get; set; }
        //public DbSet<PMSAttributeRating> PMSAttributeRating { get; set; }
        //public DbSet<PMSObjectiveAssignment> PMSObjectiveAssignment { get; set; }
        //public DbSet<PMSObjectiveReview> PMSObjectiveReview { get; set; }
        //public DbSet<EmployeeTicket> EmployeeTickets { get; set; }
        //public DbSet<EmployeeTicketCategory> EmployeeTicketCategorys { get; set; }
        //public DbSet<CasualEmployee> CasualEmployees { get; set; }
        //public DbSet<CasualEmployeeDept> CasualEmployeeDepts { get; set; }
        //public DbSet<SecondaryCustomer> SecondaryCustomer { get; set; }

    }
}