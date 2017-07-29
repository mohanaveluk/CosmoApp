using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GenerateCosmoKey.Models
{
    public interface IFill
    {
        void Fill(SqlDataReader reader);
    }
}