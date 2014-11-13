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

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFPVendorRespond
    {
        public class RFPList
        {
            public RFP rfp { get; set; }
            public RFPINVITE rfpInvite { get; set; }
            public VENDOR vendor { get; set; }
            public ICollection<RFPINVITE> rfpInviteList { get; set; }
        }

        public class FileNames_RFPRespond
        {
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        public List<FileNames_RFPRespond> GetFiles()
        {
            List<FileNames_RFPRespond> fileList = new List<FileNames_RFPRespond>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/RFPs"));

            int i = 0;
            foreach (var file in dirInfo.GetFiles())
            {
                fileList.Add(new FileNames_RFPRespond()
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
