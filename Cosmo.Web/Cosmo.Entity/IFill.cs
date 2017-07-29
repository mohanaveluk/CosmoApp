using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Cosmo.Entity
{

    public interface IFill
    {
        void Fill(SqlDataReader reader);
    }
   
}
