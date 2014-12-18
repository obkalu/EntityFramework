﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Xunit;

namespace Microsoft.Data.Entity.FunctionalTests
{
    public abstract class ChangeTrackingTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : NorthwindQueryFixtureBase, new()
    {
        [Fact]
        public virtual void Entity_reverts_when_state_set_to_unchanged()
        {
            using (var context = CreateContext())
            {
                var customer = context.Customers.First();
                var firstTrackedEntity = context.ChangeTracker.Entries<Customer>().Single();
                var originalPhoneNumber = customer.Phone;
                
                customer.Phone = "425-882-8080";

                context.ChangeTracker.DetectChanges();

                Assert.Equal(customer.CustomerID, firstTrackedEntity.Property(c => c.CustomerID).CurrentValue);
                Assert.Equal(EntityState.Modified, firstTrackedEntity.State);
                Assert.Equal("425-882-8080", firstTrackedEntity.Property(c => c.Phone).CurrentValue);

                firstTrackedEntity.SetState(EntityState.Unchanged, acceptChanges: false);
                
                Assert.Equal(customer.CustomerID, firstTrackedEntity.Property(c => c.CustomerID).CurrentValue);                
                Assert.Equal(originalPhoneNumber, firstTrackedEntity.Property(c => c.Phone).CurrentValue);
                Assert.Equal(EntityState.Unchanged, firstTrackedEntity.State);
            }
        }

        [Fact]
        public virtual void Multiple_entities_can_revert()
        {
            using (var context = CreateContext())
            {
                var customerPostalCodes = context.Customers.Select(c => c.PostalCode);
                var customerRegion = context.Customers.Select(c => c.Region);

                foreach (var customer in context.Customers)
                {
                    customer.PostalCode = "98052";
                    customer.Region = "'Murica";
                }

                Assert.Equal(91, context.ChangeTracker.Entries().Count());
                Assert.Equal("98052", context.Customers.First().PostalCode);
                Assert.Equal("'Murica", context.Customers.First().Region);

                foreach (var entityEntry in context.ChangeTracker.Entries())
                {
                    entityEntry.SetState(EntityState.Unchanged, acceptChanges: false);
                }

                var newCustomerPostalCodes = context.Customers.Select(c => c.PostalCode);
                var newCustomerRegion = context.Customers.Select(c => c.Region);

                Assert.Equal(customerPostalCodes, newCustomerPostalCodes);
                Assert.Equal(customerRegion, newCustomerRegion);
            }
        }

        protected NorthwindContext CreateContext()
        {
            return Fixture.CreateContext();
        }

        protected ChangeTrackingTestBase(TFixture fixture)
        {
            Fixture = fixture;
        }

        protected TFixture Fixture { get; private set; }
    }
}
