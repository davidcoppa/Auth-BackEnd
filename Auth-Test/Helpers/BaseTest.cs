using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth_Test.Helpers
{
    public class BaseTest
    {
        protected BaseTest()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json");
            Configuration = builder.Build();

        }
        protected IConfiguration Configuration { get; private set; }

    }
}
