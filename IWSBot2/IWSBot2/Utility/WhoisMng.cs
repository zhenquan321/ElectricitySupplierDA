using AISSystem;
using IWSBot.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IWSData.Model;
using MongoDB.Driver;
using MongoDB.Bson;

namespace IWSBot.Utility
{
    public  class WhoisMng
    {
        WhoisMng()
        { 
            //var it = ProxyLib.IPPool.Instance;
        }

        public static readonly WhoisMng Instance = new WhoisMng();


        public void start_whois()
        {
            while (true)
            {
                try
                {
                    
                        start_whois_ext();
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                }

                Thread.Sleep(30000);
            }

        }

        private void start_whois_ext()
        {
            WhoisQuery whois = new WhoisQuery();
            try
            {

                Random random = new Random();
                int skip = random.Next(0, 5);

                var builder = Builders<website>.Filter;
                var filter = builder.Eq(x => x.CompanyName, null);
                filter &= builder.Eq(x => x.RegistrantName, null);
                filter &= builder.Eq(x => x.BDV, null);
                filter &= builder.Eq(x => x.ICPLicense, null);

                var col = MongoDBHelper.Instance.Getiws_websites();
                                
                var w = col.Find(filter).Limit(1).FirstOrDefault();

                    if (w == null) return;
                    var w2 = whois.GetWhois(w.Domain);
                    w.AdminEmail = (w2.AdminEmail ?? w.AdminEmail) ?? "";
                    w.AdminPhone = (w2.AdminPhone ?? w.AdminPhone) ?? "";
                    //w.BaiduExternalLinks = w2.BaiduExternalLinks ?? w.BaiduExternalLinks;
                    //w.BaiduPages = w2.BaiduPages ?? w.BaiduPages;
                    w.CompanyName = (w2.CompanyName ?? w.CompanyName) ?? "";
                    w.DNS = (w2.DNS ?? w.DNS) ?? "";
                    w.ExpirationDate = (w2.ExpirationDate ?? w.ExpirationDate) ?? DateTime.MinValue;
                    w.ICPLicense = (w2.ICPLicense ?? w.ICPLicense) ?? "";
                    w.IDCType = (w2.IDCType ?? w.IDCType) ?? "";
                    w.IP = (w2.IP ?? w.IP) ?? 0;
                    w.PV = (w2.PV ?? w.PV) ?? 0;
                    w.RegistrantAddress = (w2.RegistrantAddress ?? w.RegistrantAddress) ?? "";
                    w.RegistrantEmail = (w2.RegistrantEmail ?? w.RegistrantEmail) ?? "";
                    w.RegistrantName = (w2.RegistrantName ?? w.RegistrantName) ?? "";
                    w.RegistrantPhone = (w2.RegistrantPhone ?? w.RegistrantPhone) ?? "";
                    w.RegistrationDate = (w2.RegistrationDate ?? w.RegistrationDate) ?? DateTime.MinValue;
                    w.SponsoringRegistrar = (w2.SponsoringRegistrar ?? w.SponsoringRegistrar) ?? "";
                    w.UV = (w2.UV ?? w.UV) ?? 0;
                    w.WebsiteType = (w2.WebsiteType ?? w.WebsiteType) ?? "";
                    w.Whois_txt = (w2.Whois_txt ?? w.Whois_txt) ?? "";
                    w.BDV = (w2.BDV ?? w.BDV) ?? "";

                    var updateWebsiteId = new UpdateDocument { { "$set", new QueryDocument { 
                    { "AdminEmail", w.AdminEmail },
                    {"AdminPhone",w.AdminPhone} ,
                    {"CompanyName",w.CompanyName} ,
                    {"DNS",w.DNS} ,
                    {"ExpirationDate",w.ExpirationDate} ,
                    {"ICPLicense",w.ICPLicense} ,
                    {"IDCType",w.IDCType} ,
                    {"IP",w.IP} ,
                    {"PV",w.PV} ,
                    {"RegistrantAddress",w.RegistrantAddress} ,
                    {"RegistrantEmail",w.RegistrantEmail} ,
                    {"RegistrantName",w.RegistrantName} ,
                    {"RegistrantPhone",w.RegistrantPhone} ,
                    {"RegistrationDate",w.RegistrationDate} ,
                    {"SponsoringRegistrar",w.SponsoringRegistrar} ,
                    {"UV",w.UV} ,
                    {"WebsiteType",w.WebsiteType} ,
                    {"BDV",w.BDV} ,
                    {"Whois_txt",w.Whois_txt} 
                    } } };
                    
                    var result2 = col.UpdateOne(new QueryDocument { { "_id", w._id } }, updateWebsiteId);
                
            }

            catch (Exception ex)
            {
                log(ex.Message);
            }
        }


        void log(string msg)
        {
            Console.WriteLine(DateTime.Now + "  :  " + msg);
        }

        
    }
}
