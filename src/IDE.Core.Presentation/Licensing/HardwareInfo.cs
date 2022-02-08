using System;
using System.Text;
using System.Management;
using System.Security.Cryptography;


namespace IDE.Core.Presentation.Licensing
{
    public class HardwareInfo
    {


        /// <summary>
        /// Get CPU ID
        /// </summary>
        /// <returns></returns>
        public static string GetProcessorId()
        {
            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["ProcessorId"].ToString();
                    break;
                }

                return _id;

            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Get motherboard serial number
        /// </summary>
        /// <returns></returns>
        public static string GetMotherboardID()
        {

            try
            {
                //ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                //ManagementObjectCollection _mbsList = _mbs.Get();
                //string _id = string.Empty;
                //foreach (ManagementObject _mo in _mbsList)
                //{
                //    _id = _mo["SerialNumber"].ToString();
                //    break;
                //}

                //return _id;

                string mbInfo = string.Empty;
                ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                scope.Connect();
                ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());

                foreach (PropertyData propData in wmiClass.Properties)
                {
                    if (propData.Name == "SerialNumber")
                    {
                        mbInfo = propData.Value.ToString();
                        break;
                    }
                }

                if (mbInfo == "Default string")
                    return string.Empty;

                return mbInfo;
            }
            catch
            {
                return string.Empty;
            }

        }


        /// <summary>
        /// Combine CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
        /// </summary>
        /// <returns></returns>
        public static string GenerateUID(string appName)
        {
            //Combine the IDs and get bytes
            string _id = string.Concat(appName, GetProcessorId(), GetMotherboardID());
            byte[] _byteIds = Encoding.UTF8.GetBytes(_id);

            //Use MD5 to get the fixed length checksum of the ID string
            MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
            byte[] _checksum = _md5.ComputeHash(_byteIds);

            //Convert checksum into 4 ulong parts and use BASE36 to encode both
            string _part1Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 0));
            string _part2Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 4));
            string _part3Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 8));
            string _part4Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 12));

            //Concat these 4 part into one string
            return string.Format("{0}-{1}-{2}-{3}", _part1Id, _part2Id, _part3Id, _part4Id);
        }

        public static bool ValidateUIDFormat(string UID)
        {
            if (!string.IsNullOrWhiteSpace(UID))
            {
                string[] _ids = UID.Split('-');

                return (_ids.Length == 4);
            }
            else
            {
                return false;
            }

        }
    }

}
