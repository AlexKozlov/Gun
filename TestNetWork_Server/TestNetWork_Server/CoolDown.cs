using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestNetWork_Server
{
    public class CoolDown
    {
        int counter;
        int reloadTime;

        public CoolDown()
        {
            counter = 0;
            reloadTime = 0;
        }
        public CoolDown(int reloadTime)
        {
            counter = 0;
            this.reloadTime = reloadTime;
        }

        public bool isReload()
        {
            if (counter >= reloadTime)
            {
                return true;
            }
            return false;

        }
        public void addTime()
        {
            //if (counter < reloadTime)
            //{
            counter++;
            //}
        }
        public void clear()
        {
            counter = 0;
        }

        public int Counter
        {
            set { this.counter = value; }
            get { return this.counter; }
        }
        public int ReloadTime
        {
            set { this.reloadTime = value; }
            get { return this.reloadTime; }
        }
    }
    
}
