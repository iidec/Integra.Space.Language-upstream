﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Integra.Space.LanguageUnitTests.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SpaceTestsEntities : DbContext
    {
        public SpaceTestsEntities()
            : base("name=SpaceTestsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<ActualResult> ActualResults { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ExpectedResult> ExpectedResults { get; set; }
    }
}