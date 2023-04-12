using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnection
{
    public class FileRoot
    {
        public static string root = Directory
            .GetParent(Directory.GetCurrentDirectory())
            .Parent
            .Parent.ToString();
    }
}
