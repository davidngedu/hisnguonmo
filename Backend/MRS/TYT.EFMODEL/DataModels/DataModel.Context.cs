﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TYT.EFMODEL.DataModels
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class TYTEntities : DbContext
    {
        public TYTEntities()
            : base("name=TYTEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<TYT_DEATH> TYT_DEATH { get; set; }
        public DbSet<TYT_FETUS_ABORTION> TYT_FETUS_ABORTION { get; set; }
        public DbSet<TYT_FETUS_BORN> TYT_FETUS_BORN { get; set; }
        public DbSet<TYT_FETUS_EXAM> TYT_FETUS_EXAM { get; set; }
        public DbSet<TYT_GDSK> TYT_GDSK { get; set; }
        public DbSet<TYT_HIV> TYT_HIV { get; set; }
        public DbSet<TYT_KHH> TYT_KHH { get; set; }
        public DbSet<TYT_MALARIA> TYT_MALARIA { get; set; }
        public DbSet<TYT_NERVES> TYT_NERVES { get; set; }
        public DbSet<TYT_TUBERCULOSIS> TYT_TUBERCULOSIS { get; set; }
        public DbSet<TYT_UNINFECT> TYT_UNINFECT { get; set; }
        public DbSet<TYT_UNINFECT_ICD> TYT_UNINFECT_ICD { get; set; }
        public DbSet<TYT_UNINFECT_ICD_GROUP> TYT_UNINFECT_ICD_GROUP { get; set; }
    }
}
