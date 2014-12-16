using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;

namespace Microsoft.Data.Entity.SqlServer.FunctionalTests
{
    public class SqlServerChangeTrackingTest : ChangeTrackingTestBase<SqlServerNorthwindQueryFixture>
    {
        public SqlServerChangeTrackingTest(SqlServerNorthwindQueryFixture fixture)
            : base(fixture)
        {
        }
    }
}
