using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignPdf.DigitalSign
{
    public interface IDigitalSignPdf
    {
        string Sign(string fileAsBase64String, string pfxFileLocation, string pfxFilePassword, string optionalPassword = "");

    }
}
