using DigitalSignPdf.DigitalSign;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignPdf
{
    public class Signer : IDigitalSignPdf
    {
        private string _tempFolder;
        private string _javaPath;
        private string _filenameWithoutExtestion;
        private string _filenameFullPath;
        private string _signedFileFullPath;
        private string _jsignpdfLocation;

        /// <summary>
        /// Create instance of DigitalSignPdf class.
        /// </summary>
        /// <param name="temporaryFolder">full path of folder that needed to create files during sign proccess.</param>
        /// <param name="javaPath">full path of java on the server.</param>
        public Signer(string temporaryFolder, string javaPath)
        {
            _tempFolder = temporaryFolder;
            _javaPath = javaPath;
            _jsignpdfLocation = "\"" + Directory.GetCurrentDirectory() + "/assets/JSignPdf-1.6.4/JSignPdf.jar" + "\"";
        }

        /// <summary>
        /// Sign pdf with X509 digital signature.
        /// </summary>
        /// <param name="fileAsBase64String">pdf file as base 64 string. (hint: use Convert.ToBase64String(File.ReadAllBytes( __YOUR_FILE__)) ).</param>
        /// <param name="pfxFileLocation">PFX file that include the X509 digital signature. </param>
        /// <param name="pfxFilePassword">PFX file password.</param>
        /// <param name="optionalPassword">If supplied - will be set as password that user must give in order to open the signed pdf file.</param>
        /// <returns>Base 64 string of the signed pdf file. (hint: use System.IO.File.WriteAllBytes(__FILE_NAME_TO_SAVE_SIGNED_PDF__ , Convert.FromBase64String(signedFileBase64String)) ).</returns>
        public string Sign(string fileAsBase64String, string pfxFileLocation, string pfxFilePassword, string optionalPassword = "")
        {
            try
            {
                WriteFileToTempFolder(fileAsBase64String);

                SignFile(pfxFileLocation, pfxFilePassword, optionalPassword);

                if (IsSignedFileExists())
                {
                    string signedFileAsBase64String = Convert.ToBase64String(File.ReadAllBytes(_signedFileFullPath));
                    DeleteTemporaryFiles();
                    return signedFileAsBase64String;
                }
                else
                    throw new Exception("signed file not created for unknown reason.");

            }
            catch (Exception e)
            {
                Log("Problem in DigitalSignPdf class. Exception : " + e.Message + " StackTrace : " + e.StackTrace.ToString());
                return "";
            }
        }

        private void DeleteTemporaryFiles()
        {
            try
            {
                File.Delete(_signedFileFullPath);
                File.Delete(_filenameFullPath);
            }
            catch (Exception e)
            {
                Log("DigitalSignturePdf Error - can not delete at least one of the temporary files.");

            }
        }


        #region private_methods
        private void WriteFileToTempFolder(string fileAsBase64String)
        {
            _filenameWithoutExtestion = Guid.NewGuid().ToString("N");
            _filenameFullPath = _tempFolder + Path.DirectorySeparatorChar + _filenameWithoutExtestion + ".pdf";
            System.IO.File.WriteAllBytes(_filenameFullPath, Convert.FromBase64String(fileAsBase64String));

        }

        private void SignFile(string pfxFileLocation, string pfxFilePassword, string optionalPassword)
        {
            string arguments = " -jar " + _jsignpdfLocation + " -cl CERTIFIED_NO_CHANGES_ALLOWED  -kst PKCS12  -ksf " + pfxFileLocation + " -ksp " + pfxFilePassword;

            if (optionalPassword.Trim().Length > 0)
                arguments += " -e -upwd " + optionalPassword.Trim();

            arguments += " -ha SHA1 -V -ta CERTIFICATE  -d " + _tempFolder + Path.DirectorySeparatorChar;
            arguments += " " + _filenameFullPath;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = _javaPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                //process.StartInfo.CreateNoWindow = false;

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string err = process.StandardError.ReadToEnd();
                Log(output);

                if (err.Trim().Length > 0)
                    Log("Problem in DigitalSignPdf Class : " + err);

                process.EnableRaisingEvents = true;
                process.WaitForExit();
            }


        }

        private bool IsSignedFileExists()
        {
            _signedFileFullPath = _tempFolder + Path.DirectorySeparatorChar + _filenameWithoutExtestion + "_signed.pdf";

            if (System.IO.File.Exists(_signedFileFullPath))
                return true;
            else
                return false;
        }


        private void Log(string s)
        {
            Console.WriteLine(s);
        }
        #endregion



    }
}
