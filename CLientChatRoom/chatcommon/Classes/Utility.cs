using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace chatcommon.Classes
{
    public class Utility
    {
        public static DateTime DateTimeMinValueInSQL2005 = new DateTime(1753, 1, 1);

        public static DateTime convertToDateTime(string dateString, bool? isFromDatePicker = false)
        {
            var listDateElement = dateString.Split('/');

            try
            {
                if (isFromDatePicker == true && listDateElement.Count() > 1)
                {
                    int day = Convert.ToInt32(listDateElement[1]);
                    int month = Convert.ToInt32(listDateElement[0]);
                    int year = Convert.ToInt32(listDateElement[2].Split(' ')[0]);
                    dateString = day + "/" + month + "/" + year;// +" "+ DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                }
            }
            catch (Exception)
            {
                Log.write("Error parsing date " + dateString, "WAR");
            }

            DateTime outDate = new DateTime();
            if (DateTime.TryParse(dateString, out outDate) && outDate > DateTimeMinValueInSQL2005)
                return outDate;

            return DateTimeMinValueInSQL2005;
        }

        public static bool convertToBoolean(string boolString)
        {
            bool outBool = new bool();
            if (bool.TryParse(boolString, out outBool))
                return outBool;

            return false;
        }

        public static string encodeStringToBase64(string stringToEncode)
        {
            if (!string.IsNullOrEmpty(stringToEncode))
            {
                byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(stringToEncode);
                string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

                return returnValue;
            }

            return stringToEncode;
        }

        public static string decodeBase64ToString(string encodedString)
        {
            string returnValue = encodedString;
            if (!string.IsNullOrEmpty(encodedString) && isBase64Encoded(encodedString))
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedString);
                returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            return returnValue;
        }

        private static bool isBase64Encoded(string inputString)
        {
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(inputString);
                string decoded_data = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

                string encoded_data = encodeStringToBase64(decoded_data);

                if (encoded_data != inputString)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }            

            return true;
        }

        public static bool uploadFIle(string ftpUrl, string fileFullPath, string username, string password)
        {
            bool isComplete = false;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpUrl);
            req.UseBinary = true;
            req.KeepAlive = true;
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.Credentials = new NetworkCredential(username, password);
            Stream requestStream = null;
            byte[] buffer;
            StreamReader streamReaderSource = new StreamReader(fileFullPath);
            try
            {
                buffer = Encoding.UTF8.GetBytes(streamReaderSource.ReadToEnd());
                req.ContentLength = buffer.Length;
                requestStream = req.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
            }
            catch (WebException ex)
            {
                String status = ((FtpWebResponse)ex.Response).StatusDescription;
                Log.error(status);
            }
            finally
            {
                requestStream.Close();
                streamReaderSource.Close();
            }

            downloadFIle(ftpUrl, fileFullPath, username, password);

            FtpWebResponse response = (FtpWebResponse)req.GetResponse();
            if (response.StatusCode.Equals(FtpStatusCode.ClosingData)) // && count > 0)
                isComplete = true;

            return isComplete;
        }

        public static bool downloadFIle(string ftpUrl, string fileFullPath, string username, string password)
        {
            bool isComplete = false;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpUrl);
            req.UseBinary = true;
            req.KeepAlive = true;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Credentials = new NetworkCredential(username, password);
            req.Timeout = 600000;
            FtpWebResponse response = null;
            Stream ftpStream = null;
            FileStream fs = null;
            try
            {
                response = (FtpWebResponse)req.GetResponse();
                ftpStream = response.GetResponseStream();

                long cl = response.ContentLength;
                int bufferSize = 4096;  
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);

                fs = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.Read);

                while (readCount > 0)
                {
                    fs.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
            }
            catch (WebException ex)
            {
                String status = ((FtpWebResponse)ex.Response).StatusDescription;
                Log.error(status);
            }
            finally
            {
                fs.Close();
                response.Close();
                ftpStream.Close();
            }


            if (response.StatusCode.Equals(FtpStatusCode.ClosingData))
                isComplete = true;

            return isComplete;
        }

        public static Dictionary<T, P> concat<T, P>(Dictionary<T, P> dictTarget, Dictionary<T, P> dictSource)
        {
            foreach (var dict in dictSource)
            {
                dictTarget.Add(dict.Key, dict.Value);
            }

            return dictTarget;
        }

        public static List<T> concat<T>(List<T> Target, List<T> Source)
        {
            foreach (var value in Source)
            {
                Target.Add(value);
            }

            return Target;
        }

    }
}
