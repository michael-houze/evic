using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Web.Hosting;
using System.ComponentModel.DataAnnotations;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFPVendorRespond
    {
        public class RFPList
        {
            public RFP RFP { get; set; }
            public RFPINVITE RFPInvite { get; set; }
            public VENDOR vendor { get; set; }
            public ICollection<RFPINVITE> RFPInviteList { get; set; }

            [Required]
            [FileExtensions(Extensions = "xlsx,xls")]
            public HttpPostedFileBase File { get; set; }

            [FileExtensions(Extensions = "xlsx,xls,pdf")]
            public HttpPostedFileBase Catalog { get; set; }
        }

        public class FileNames_RFPResponse
        {
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        public List<FileNames_RFPResponse> GetFiles()
        {
            List<FileNames_RFPResponse> fileList = new List<FileNames_RFPResponse>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/RFPs"));

            int i = 0;
            foreach (var file in dirInfo.GetFiles())
            {
                fileList.Add(new FileNames_RFPResponse()
                {
                    FileId = i + 1,
                    FileName = file.Name,
                    FilePath = dirInfo.FullName + @"\" + file.Name
                });
            }
            return fileList;
        }
    }
}
