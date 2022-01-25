using BeautiesShop.Entity_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BeautiesShop.Other
{
    internal class Transition
    {
        public static Frame MainFrame { get; set; }
        private static KosmeticDBEntities datacontext;
        public static KosmeticDBEntities Datacontext
        {
            get 
            {
                if (datacontext==null)
                {
                    datacontext = new KosmeticDBEntities();
                }
               return datacontext;
            }
        }
    }
}
