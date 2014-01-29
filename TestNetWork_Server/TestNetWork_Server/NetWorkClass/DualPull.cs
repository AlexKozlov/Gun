using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassesForSerialize;

namespace TestNetWork_Server.NetWorkClass
{
    public class DualPull
    {
        PlayerInfo oldPull;
        PlayerInfo newPull;

        public DualPull()
        {
            oldPull = new PlayerInfo();
        }
        public DualPull(PlayerInfo basePull)
        {
            oldPull = basePull;
            newPull = null;
        }

        //public void setPull(PlayerInfo obj)
        //{
        //    newPull = obj;
        //}

        //public PlayerInfo getPull()
        //{
        //    if (newPull == null)
        //    {
        //        return oldPull;
        //    }

        //    return newPull;
        //}

        public PlayerInfo Pull
        {
            set 
            {
                this.newPull = value;

                this.newPull.HP = oldPull.HP;
            }
            get 
            { 
                if (newPull == null)
                {
                    return oldPull;
                }

                return newPull;
            }
        }

        public PlayerInfo ClearPull()
        {
            if (this.newPull != null)
            {
                this.oldPull = this.newPull;
                this.newPull = null;
            }

            return this.oldPull;            
        }
    }
}
