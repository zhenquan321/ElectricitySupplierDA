using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AISSystem;
//using HTML;
using System.Xml.Linq;
using System.Net;
using System.IO;
using IWSData.Model;
using ProxyLib;

namespace IWSBot.Queries
{
    public class WhoisQuery
    {
        //WebHelper web = new WebHelper();
        ProxyLib.WebHelperNoCookieProxy web = new ProxyLib.WebHelperNoCookieProxy();
        public website GetWhois(string domain)
        {
            website webs = new website();

            //测试
            //将下面的代码复制到UnitTest1里面进行测试
            //[TestMethod]
            //public void whois() 
            //{
            //    WhoisQuery s = new WhoisQuery();
            //    s.GetWhois("bookzx.org");
            //}

            //获取pv ,ip
            //http://www.alexa.cn/index.php?url=bookzx.org

            //js文件地址  http://www.alexa.cn/jquery_alexa_new_beta.js
            //date="url="+str+"&sig="+sig+"&keyt="+keyt
            //bookzx.org,835c4d4506618f02994adada985f41f3,1421651144
            //http://alexa.cn/api0523.php?url=bookzx.org&sig=835c4d4506618f02994adada985f41f3&keyt=1421651144


            string url = "http://www.alexa.cn/index.php?url={0}".FormatStr(domain);
            var xdoc = web.GetHtml(url, null, "utf-8");
            var companyName = xdoc.SubstringAfter("主办单位名称").SubstringBefore("</font>").SubstringAfter("<font>");
            companyName = BaiduQuery.RemoveInivalidChar(companyName);
            if (!string.IsNullOrEmpty(companyName))
                webs.CompanyName = companyName;
            var idetifiNo = xdoc.SubstringAfter("网站备案/许可证号").SubstringAfter("<font>").SubstringBefore("</font");//xdoc.SubstringAfter("网站备案/许可证号").SubstringBefore("</a>").SubstringAfter("\">");
            if (!string.IsNullOrEmpty(idetifiNo) && !idetifiNo.Contains("无备案信息"))
            {
                idetifiNo = idetifiNo.SubstringBefore("</a>").SubstringAfter("\">");
                idetifiNo = idetifiNo.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\b", "");
                if (!string.IsNullOrEmpty(idetifiNo) && idetifiNo != "无备案信息")
                    webs.ICPLicense = idetifiNo;
            }

            //获取whois
            //http://whois.www.net.cn/whois/domain/bookzx.org?spm=5334.WHbookzxor.5.1

            //FD获取的链接
            //http://whois.www.net.cn/whois/api_whois?host=bookzx.org&_=1421306342508
            //http://whois.www.net.cn/whois/api_whois?host=bookzx.org
            //http://whois.www.net.cn/whois/api_whois?host=qiaxz.com
            //百度
            //http://whois.www.net.cn/whois/api_whois_full?host=baidu.com&web_server=whois.markmonitor.com&_=1422513346623
            //http://whois.www.net.cn/whois/api_whois_full?host=baidu.com
            //string url1 = "http://whois.www.net.cn/whois/api_whois?host={0}".FormatStr(domain);
            string url1 = "http://whois.alexa.cn/whois.php?u={0}".FormatStr(domain);//http://whois.alexa.cn/whois.php
            string url3 = "http://whois.www.net.cn/whois/api_whois_full?host={0}".FormatStr(domain);
            //如果不加下面这行代码，Json文件会出现{"code":"405","msg":"限制访问","success":false}
            //先让程序运行一遍网页，再进行抓取Json文件。
            //var html1 = web.GetHtml("http://whois.www.net.cn/whois/domain/{0}?spm=5334.WHbookzxor.5.1".FormatStr(domain),null,"gbk");
            var html = web.GetHtml(url1, null, "utf-8");

            if (html.IsContains("域名服务器:"))
            {
                string whoisdomain = html.SubstringAfter("域名服务器:").SubstringBefore("<br").Trim();
                string url2 = "http://whois.alexa.cn/whois.php?server={0}&who={1}".FormatStr(whoisdomain, domain);
                html = web.GetHtml(url2, null, "utf-8");
                if (!string.IsNullOrEmpty(html))
                {
                    string registrantName = html.SubstringAfter("Registrant Name:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(registrantName))
                        webs.RegistrantName = registrantName;
                    string email = html.SubstringAfter("Registrant Email:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(email))
                        webs.RegistrantEmail = email;
                    string sponsoringRegistrar = html.SubstringAfter("Registrar:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(sponsoringRegistrar))
                        webs.SponsoringRegistrar = sponsoringRegistrar;
                    DateTime? zhuceriqi = html.SubstringAfter("Creation Date:").SubstringBefore("<br").ToDateTime();
                    if (zhuceriqi.HasValue)
                        webs.RegistrationDate = zhuceriqi;
                    DateTime? daoqiriqi = html.SubstringAfter("Registrar Registration Expiration Date:").SubstringBefore("<br").ToDateTime();
                    if (daoqiriqi.HasValue)
                        webs.ExpirationDate = daoqiriqi;
                    string dns = html.SubstringAfter("Name Server:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(dns))
                        webs.DNS = dns;
                    string phone = html.SubstringAfter("Registrant Phone:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(phone))
                        webs.RegistrantPhone = phone;
                    string address = html.SubstringAfter("Registrant Street:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(address))
                        webs.RegistrantAddress = address;
                    string adminEmail = html.SubstringAfter("Admin Email:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(adminEmail))
                        webs.AdminEmail = adminEmail;
                    string adminPhone = html.SubstringAfter("Admin Phone:").SubstringBefore("<br").GetTrimed();
                    if (!string.IsNullOrEmpty(adminPhone))
                        webs.AdminPhone = adminPhone;
                }
            }

            if (string.IsNullOrEmpty(webs.ICPLicense))
            {
                //&qq-pf-to=pcqq.group
                string wurl = "http://seo.chinaz.com/?host={0}".FormatStr(domain);
                string whtml = web.GetHtml(wurl, null, "utf-8");
                if (whtml.IsContains("获取不到Seo数据,可能是网站无法访问造成"))
                {
                    Console.WriteLine("获取不到Seo数据,可能是网站无法访问造成");
                }
                if (!string.IsNullOrEmpty(whtml))
                {
                    string license = whtml.SubstringAfter("备案号:").SubstringAfter("/font>").SubstringBefore("<font").GetTrimed();
                    if (license.IsContain("&nbsp"))
                        license = license.SubstringBefore("&nbsp");
                    if (!string.IsNullOrEmpty(license))
                        webs.ICPLicense = license;
                    //string seokw = whtml.SubstringAfter("dekey='").SubstringBefore("'");
                }
                else
                {
                    Console.WriteLine("备案号Html没有提取到");
                }
            }

            string baiduvurl = "http://www.baidu.com/s?wd={0}%40v".FormatStr(domain);
            var baiduvhtml = web.GetHtml(baiduvurl, null, "utf-8");

            if (!string.IsNullOrEmpty(baiduvhtml))
            {
                if (string.IsNullOrEmpty(webs.BDV))
                {
                    webs.BDV = baiduvhtml.SubLastStringAfter("主体识别码:").SubstringBefore("</span>").SubLastStringAfter(">");
                }
                if (string.IsNullOrEmpty(webs.ICPLicense))
                {
                    webs.ICPLicense = baiduvhtml.SubLastStringAfter("备案编号：").SubstringBefore("</td>").SubLastStringAfter(">");
                }
                if (string.IsNullOrEmpty(webs.Whois_txt))
                {
                    webs.Whois_txt = BaiduQuery.RemoveInivalidChar(baiduvhtml.SubLastStringAfter("经营范围：").SubstringBefore("</div>").SubstringAfter("data-origin=\"").SubstringBefore("\">"));
                }

                var bdwebtype = baiduvhtml.SubLastStringAfter("商家类型：").SubstringBefore("</td>").SubLastStringAfter(">");

                if (string.IsNullOrEmpty(bdwebtype))
                {
                    bdwebtype = baiduvhtml.SubLastStringAfter("机构类型：").SubstringBefore("</td>").SubLastStringAfter(">");

                }
                if (!string.IsNullOrEmpty(bdwebtype))
                {
                    webs.WebsiteType = bdwebtype;
                }
                if (string.IsNullOrEmpty(webs.BDV))
                {
                    webs.BDV = "";
                }
            }

            return webs;
        }
    }
}

