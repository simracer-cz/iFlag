using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace iFlag
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
           using(Mutex mutex = new Mutex(false, "iFlag"))
           {
              if(!mutex.WaitOne(0, false))
              {
                 //MessageBox.Show("Process already running");
                 return;
              }
           
              GC.Collect();                
              Application.EnableVisualStyles();
              Application.SetCompatibleTextRenderingDefault(false);
              Application.Run(new mainForm());
           }
        }
    }
}
