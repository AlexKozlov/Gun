using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassesForSerialize;

namespace TestNetWork_Server.NetWorkClass
{
    class DualPull
    {
        SomeText oldPull;
        SomeText newPull;

        public DualPull()
        {

        }
        public DualPull(SomeText basePull)
        {
            oldPull = basePull;
            newPull = null;
        }

        public void setPull(SomeText obj)
        {
            newPull = obj;
        }

        public SomeText getPull()
        {
            if (newPull == null)
            {
                return oldPull;
            }

            return newPull;
        }
    }
}
