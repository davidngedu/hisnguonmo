﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAR.EFMODEL.DataModels
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<SAR_FORM_FIELD> SAR_FORM_FIELD { get; set; }
        public DbSet<SAR_PRINT> SAR_PRINT { get; set; }
        public DbSet<SAR_PRINT_TYPE> SAR_PRINT_TYPE { get; set; }
        public DbSet<SAR_REPORT> SAR_REPORT { get; set; }
        public DbSet<SAR_REPORT_CALENDAR> SAR_REPORT_CALENDAR { get; set; }
        public DbSet<SAR_REPORT_STT> SAR_REPORT_STT { get; set; }
        public DbSet<SAR_REPORT_TEMPLATE> SAR_REPORT_TEMPLATE { get; set; }
        public DbSet<SAR_REPORT_TYPE> SAR_REPORT_TYPE { get; set; }
        public DbSet<SAR_RETY_FOFI> SAR_RETY_FOFI { get; set; }
        public DbSet<SAR_USER_REPORT_TYPE> SAR_USER_REPORT_TYPE { get; set; }
        public DbSet<V_SAR_REPORT> V_SAR_REPORT { get; set; }
        public DbSet<V_SAR_REPORT_TEMPLATE> V_SAR_REPORT_TEMPLATE { get; set; }
        public DbSet<V_SAR_RETY_FOFI> V_SAR_RETY_FOFI { get; set; }
        public DbSet<V_SAR_USER_REPORT_TYPE> V_SAR_USER_REPORT_TYPE { get; set; }
    }
}
