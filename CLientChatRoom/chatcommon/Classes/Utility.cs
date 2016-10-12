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
    public static class Utility
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
            string returnValue = "";
            /*bool isValidBase64Encoded;
            if (!string.IsNullOrEmpty(encodedString))
            {
                encodedString = encodedString.Trim();
                isValidBase64Encoded = (encodedString.Length % 4 == 0) && Regex.IsMatch(encodedString, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
                if (isValidBase64Encoded)
                {
                    byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedString);
                    returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
                }
                else
                {
                    returnValue = encodedString;
                    Debug.WriteLine(string.Format("[Warning] - decode base64 of not encoded variable ({0})", encodedString));
                }
            }*/

            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedString);
                returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (Exception)
            {
                Debug.WriteLine(string.Format("[Warning] - decode base64 of not encoded variable ({0})", encodedString));
                return encodedString;
            }

            return returnValue;
        }

        public static bool uploadFIle(string ftpUrl, string fileFullPath, string username, string password)
        {
            bool isComplete = false;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpUrl);
            req.UseBinary = true;
            //req.UsePassive = true;
            req.KeepAlive = true;
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.Credentials = new NetworkCredential(username, password);
            Stream requestStream = null;
            //FileStream stream = null;

            // Copy the file contents to the request stream.
            //const int bufferLength = 2048;
            byte[] buffer;// = new byte[bufferLength];
            //int count = 0;
            //int readBytes = 0;
            StreamReader streamReaderSource = new StreamReader(fileFullPath);
            try
            {
                buffer = Encoding.UTF8.GetBytes(streamReaderSource.ReadToEnd());
                //stream = File.OpenRead(fileFullPath);

                

                //do
                //{
                    //readBytes = stream.Read(buffer, 0, buffer.Length);
                    req.ContentLength = buffer.Length;
                requestStream = req.GetRequestStream();
                //readBytes = stream.Read(buffer, 0, buffer.Length);
                requestStream.Write(buffer, 0, buffer.Length);
                    //count += readBytes;
                //}
                //while (readBytes != 0);
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
            //req.UsePassive = true;
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
                int bufferSize = 4096;  //Image file cannot be greater than 40 Kb
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



    }
}
