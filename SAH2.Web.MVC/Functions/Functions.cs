using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace SAH2.Web.MVC.Functions
{
    public static partial class Functions
    {
        public static FilePathResult GetFileToBrowse(string fileServerAddress, string fileName, string contentType)
        {
            var ext = fileServerAddress.Split('.').Last().Trim();
            var dotExt = (string.IsNullOrWhiteSpace(ext) ? string.Empty : ".") + ext;
            var fileDownloadName = $"{fileName}{dotExt}";
            HttpContext.Current.Response.AddHeader("Content-Disposition", $"inline; filename={fileDownloadName}");
            return new FilePathResult(fileServerAddress, contentType);
        }

        public static FileContentResult GetFileToDownload(string fileServerAddress, string fileName)
        {
            var isFileExists = File.Exists(fileServerAddress);

            if (!isFileExists)
                throw new FileNotFoundException(nameof(fileServerAddress), fileServerAddress);

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var fileBytes = File.ReadAllBytes(fileServerAddress);
            var fileExtension = fileServerAddress.Split('.').Last();
            var fileFullName = $"{fileName}.{fileExtension}";
            return new FileContentResult(fileBytes, MediaTypeNames.Application.Octet) { FileDownloadName = fileFullName };
        }
    }
}
