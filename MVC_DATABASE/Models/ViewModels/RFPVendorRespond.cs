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
            public RFPINVITE rfpinvite { get; set; }
            public ICollection<RFPINVITE> inviteList { get; set; }
        }
        public List<_fileNames> GetFiles()
        {
           
            List<_fileNames> lst_Files = new List<_fileNames>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/RFPs"));
           
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {

                lst_Files.Add(new _fileNames() {

                    FileId = i + 1, FileName = item.Name, FilePath = dirInfo.FullName+@"\"+item.Name});
                    i = i + 1;
                }
               
                return lst_Files;
            }
        }

    public class _fileNames
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
