﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_DATABASE.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BaptistEntities : DbContext
    {
        public BaptistEntities()
            : base("name=BaptistEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CONTRACT> CONTRACTs { get; set; }
        public virtual DbSet<EMPLOYEE> EMPLOYEEs { get; set; }
        public virtual DbSet<OFFEREDCATEGORY> OFFEREDCATEGORies { get; set; }
        public virtual DbSet<PRODUCTCATEGORY> PRODUCTCATEGORies { get; set; }
        public virtual DbSet<RFI> RFIs { get; set; }
        public virtual DbSet<RFIINVITE> RFIINVITEs { get; set; }
        public virtual DbSet<RFP> RFPs { get; set; }
        public virtual DbSet<RFPINVITE> RFPINVITEs { get; set; }
        public virtual DbSet<TEMPLATE> TEMPLATEs { get; set; }
        public virtual DbSet<VENDOR> VENDORs { get; set; }
        public virtual DbSet<VENDORCONTACT> VENDORCONTACTs { get; set; }
    }
}
