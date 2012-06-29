using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;



public static class HardwareInfo
    {
        public static String GetProcessorId()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String Id = String.Empty;
            foreach (ManagementObject mo in moc)
            {

                Id = mo.Properties["Name"].Value.ToString();
                break;
            }
            return Id;
        }

        public static String GetHDDSerialNo()
        {
            ManagementObjectSearcher mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject moDisk in mosDisks.Get())
            {
                return moDisk["Model"].ToString() + " (" + Math.Round(((((double)Convert.ToDouble(moDisk["Size"]) / 1024) / 1024) / 1024), 2) + " GB)";
            }
            return " ";
        }
    }



