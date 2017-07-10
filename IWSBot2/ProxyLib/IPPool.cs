using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML;
using System.Threading;
using AISSystem;

namespace ProxyLib
{
    public class IPPool
    { 
        int maxsize = 100;
        int max_timeout = 2000;
        object ip_token = new object();
        Dictionary<string, long> provider_ips = new Dictionary<string, long>();
        Stack<IP> avaliable_ips = new Stack<IP>();
        Dictionary<string, object> history = new Dictionary<string, object>();


        public int AvaliableIps
        {
            get
            {
                lock (ip_token)
                {
                    return avaliable_ips.Count;
                }
            }
        }

      
        IPPool()
        {
            //GetIpFromHaodaili(); 
            //GetProxyFromProxy360();
            GetIpFromPaidProvider(); 
            //GetIpFromRosinstrument();
            //GetProxyFromProxyComRu();
            //GetProxyFromProxyList();
        }

        public static readonly IPPool Instance = new IPPool();

        public IP GetIpBlock(string nick_name)
        {
            nick_name = nick_name ?? "";
            IP ip = IPPool.Instance.GetIp();
            while (ip == null)
            {
                //log( nick_name .ToUpper()+ " no avaliable proxy ip");
                Thread.Sleep(1000);
                ip = IPPool.Instance.GetIp();
            }
            
            return ip;
        }

        public IP GetIp()
        {
            IP ip = null;
            try
            {
                if (avaliable_ips.Count > 0)
                    ip = avaliable_ips.Pop();                
            }
            catch
            {
            }

            return ip;
        }

        #region Get ProxyIp from Paid provider

        List<IpProvider> providers = new List<IpProvider>();    
        

        WebHelper web = new WebHelper();
        void GetIpFromPaidProvider()
        {
            get_ip_providers();
            foreach (var provider in providers)
            {
                Thread t = new Thread(new ParameterizedThreadStart(start_get_ips_from_paid_provider));
                t.Start(provider); 
            }
        }

        private void get_ip_providers()
        {
            providers.Clear();
            string[] ps = AISSystem.AppSettingHelper.GetAppSetting("IPProviders").SplitWith("$;");
            foreach (var p in ps)
            {
                string url = p.SubBefore("&provider_name");
                string name = p.SubAfter("&provider_name=").SubBefore("&");
                string ip_sp = p.SubAfter("&ip_sp=").SubBefore("&");
                string port_sp = p.SubAfter("&port_sp=").SubBefore("&");
                int? intervals = p.SubAfter("&intervals=").ExInt();
                IpProvider pro = new IpProvider { name = name, url = url };
                if (intervals.HasValue)
                    pro.intervals = intervals.Value ;
                if (!string.IsNullOrEmpty(ip_sp))
                    pro.ip_sp = ip_sp;
                if (!string.IsNullOrEmpty(port_sp))
                    pro.port_sp = port_sp;
                providers.Add(pro);
                log(pro.name + " Initialized");
            }
        }

        private void start_get_ips_from_paid_provider(object o)
        {
            IpProvider provider= o as IpProvider;
            while (true)
            {
                try
                {
                    while ((DateTime.Now - provider.lst_get_ip_at).TotalMilliseconds < provider.intervals)
                    {
                        Thread.Sleep(1000);
                    }
                    provider.lst_get_ip_at = DateTime.Now;
                    provider.get_times++;
                    var _ips = get_ip_from_provider(provider.url, provider.ip_sp, provider.port_sp);
                    if (_ips != null && _ips.Count > 0)
                    {
                        provider.total_ips += _ips.Count;
                        _ips.ForEach(x => add_ip_if_pass_test(x, provider.name));
                    }
                    //else
                    //    log(provider.name + " RETRUN EMPTY IP");
                }
                catch (Exception ex)
                {
                    log( provider.name +" ERROR :" + ex.Message);
                }
            }
        }         

        List<IP> get_ip_from_provider(string url, string ip_splitor, string port_spliter = ":")
        {
            List<IP> list = new List<IP>();        
            var _ips = web.GetHtml(url , null).SplitWith(ip_splitor);
            if (_ips == null || _ips.Length == 0)
            { 
                return null;
            }
            foreach (var _ip in _ips)
            {
                string ip = _ip.SubBefore(port_spliter  ).GetTrimed();
                int? port = _ip.SubAfter(port_spliter).ExInt();
                if (string.IsNullOrEmpty(ip) || !port.HasValue || !ip.GetDigital().ExLong().HasValue)
                    continue;
                list.Add(new IP { Ip = ip, Port = port.Value });
            }
            return list;
        }

 
        class IpProvider
        {
            public string name { get; set; }
            public string url { get; set; }
            string _ip_sp = "\r";
            public string ip_sp { get { return _ip_sp; } set { _ip_sp = value; } }
            string _port_sp = ":";
            public string port_sp { get { return _port_sp; } set { _port_sp = value; } }
            int _intervals = 1000 * 60 * 4;
            public int intervals { get { return _intervals; } set { _intervals = value; } }
            public DateTime lst_get_ip_at { get; set; }
            public int get_times { get; set; }
            public int total_ips { get; set; }
            public int avaliable_ips { get; set; }
        }
         
        #endregion

        #region Get IP from Free


        public void GetIpFromRosinstrument()
        {
            Thread t = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        string url = "http://www.rosinstrument.com/proxy/";
                        var xdoc = web.GetXDoc(url, null);
                        if (xdoc == null)
                            return;
                        var lis = xdoc.Root.GetDescendant("a", "well-known").GetAncetor("fieldset").GetDescendants("a");
                        if (lis == null)
                            return;
                        List<IP> list = new List<IP>();
                        lis.ForEach(x =>
                        {
                            string txt = x.GetValue();
                            string ip = txt.SubBefore(":");
                            var port = txt.SubAfter(":").ExInt();
                            if (!string.IsNullOrEmpty(ip) && port.HasValue)
                            {
                                add_ip_if_pass_test(new IP { Ip = ip, Port = port.Value }, "RosinstrumentFREE");
                            }

                        });

                        Thread.Sleep(120000);
                    }
                }));
            t.Start();
             
        }

        void GetProxyFromProxyList()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //http://proxy-list.org/english/index.php?p=1
                    string url = "http://proxy-list.org/english/index.php?p=1";
                    var xdoc = web.GetXDoc(url, null);
                    while (xdoc != null)
                    {
                        var lis = xdoc.Root.GetDescendants("li", "class", "proxy");
                        if (lis == null || lis.Count == 0)
                            break;
                        lis.ForEach(x =>
                        {
                            string txt = x.GetValue();
                            string ip = txt.SubBefore(":");
                            var port = txt.SubAfter(":").ExInt();
                            if (port.HasValue && !string.IsNullOrEmpty(ip))
                                add_ip_if_pass_test(new IP { Ip = ip, Port = port.Value }, "ProxyListFREE");
                        });
                        url = xdoc.Root.GetDescendantByClass("a", "next").GetHref();
                        if (string.IsNullOrEmpty(url))
                            break;
                        if (url.IsStartWith("."))
                            url = url.ReplaceWith(".", "http://proxy-list.org/english");
                        xdoc = web.GetXDoc(url, null);
                    }

                    Thread.Sleep(120000);
                }
            }));
            t.Start();
        }

        void GetProxyFromProxyComRu()
        {
            Thread t = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        try
                        {
                            string baseUrlFormath = "http://www.proxy.com.ru/touming/list_{0}.html";

                            var xdoc = web.GetXDoc("http://www.proxy.com.ru/touming/", null, "gb2312");
                            //  <td bgcolor="#D3D3D3">
                            var ptg = xdoc.Root.GetDescendants("td", "bgcolor", "D3D3D3");
                            if (ptg == null)
                                return;
                            var pagesNum = ptg.Last().GetValue().SubAfter("共").ExInt();
                            if (!pagesNum.HasValue)
                            {
                                return;
                            }
                            for (int page = 1; page <= pagesNum; page++)
                            {
                                xdoc = web.GetXDoc(baseUrlFormath.FormatStr(page), null, "gb2312");
                                if (xdoc == null)
                                    continue;
                                var tds = xdoc.Root.GetDescendants("td", "透明代理");
                                foreach (var td in tds)
                                {
                                    var tags = td.GetAncetor("tr").GetDescendants("td");
                                    if (tags == null || tags.Count < 3)
                                        continue;
                                    string ip = tags[1].GetValue().GetTrimed();
                                    int? port = tags[2].GetValue().GetTrimed().ToInt();
                                    if (string.IsNullOrEmpty(ip) || !port.HasValue)
                                        continue;
                                    add_ip_if_pass_test(new IP { Ip = ip, Port = port.Value }, "ProxyComRuFREE");
                                }
                            }
                        }
                        catch
                        {
                        }


                        Thread.Sleep(120000);
                    }
                }));

            t.Start();

        }

        void GetProxyFromProxy360()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    try
                    {
                        string url = "http://www.proxy360.cn/default.aspx";
                        var xdoc = web.GetXDoc(url, null);
                        if (xdoc == null)
                            return;
                        var tags = xdoc.Root.GetDescendantById("div", "ctl00_ContentPlaceHolder1_upProjectList").GetDescendants("div", "name", "list_proxy_ip");
                        if (tags == null)
                            continue;
                        List<IP> list = new List<IP>();
                        tags.ForEach(x =>
                        {
                            string ip = x.GetDescendantByClass("span", "tbBottomLine").GetValue().GetTrimed();
                            var port = x.GetDescendantByClass("span", "tbBottomLine").GetFollowSibling("span").GetValue().ExInt();
                            if (!string.IsNullOrEmpty(ip) && port.HasValue)
                            {
                                add_ip_if_pass_test(new IP { Ip = ip, Port = port.Value }, "ProxyListFREE");
                            }

                        });
                    }
                    catch
                    { 
                    }
                    Thread.Sleep(120000);

                }
            }));
            t.Start();
        }
        
        void GetProxyFromKuaidaili()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    try
                    {
                        string baseUrlFormath = "http://www.kuaidaili.com/free/inha/{0}/";

                        var xdoc = web.GetXDoc("http://www.kuaidaili.com/free/", null);
                        var ptg = xdoc.Root.GetDescendant("div", "id", "listnav").GetDescendants("a");
                        if (ptg == null)
                            return;
                        var pagesNum = ptg.Last().GetValue().ExInt();
                        if (!pagesNum.HasValue)
                        {
                            return;
                        }
                        for (int page = 1; page <= pagesNum; page++)
                        {
                            xdoc = web.GetXDoc(baseUrlFormath.FormatStr(page), null);
                            if (xdoc == null)
                                continue;
                            var tds = xdoc.Root.GetDescendant("table", "class", "table table-bordered table-striped").GetDescendants("tr");
                            foreach (var td in tds)
                            {
                                string ip = td.GetDescendant("td").GetValue();
                                int? port = td.GetDescendant("td").GetFollowSibling("td").GetValue().ExInt();
                                if (string.IsNullOrEmpty(ip) || !port.HasValue)
                                    continue;
                                add_ip_if_pass_test(new IP { Ip = ip, Port = port.Value }, "ProxyComRuFREE");
                            }
                        }
                    }
                    catch
                    {
                    }

                    Thread.Sleep(120000);
                }
            }));

            t.Start();
        }
        
        void GetIpFromHaodaili()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //http://www.haodailiip.com/guonei
                    string url = "http://www.haodailiip.com/guonei";
                    var xdoc = web.GetXDoc(url, null);
                    int? page = 1;
                    while (xdoc != null)
                    {
                        var lis = xdoc.Root.GetDescendant("table", "class", "proxy_table").GetDescendants("tr");
                        if (lis == null || lis.Count == 0)
                            return;
                        lis.ForEach(x =>
                        {
                            string ip = x.GetValue().SubAfter("i0=\"").SubBefore("\"");
                            if (ip.IsContain("k"))
                            {
                                ip = ip.Replace("k", "2");
                            }
                            if (ip.IsContain("f"))
                            {
                                ip = ip.Replace("f", "1");
                            }
                            if (ip.IsContain("j"))
                            {
                                ip = ip.Replace("j", "5");
                            }
                            var po = x.GetValue().SubAfter("p1=").SubBefore(";").ExInt();
                            var rt = x.GetValue().SubAfter("p1=").SubAfter("/").ExInt();
                            var port = po / rt;
                            if (port.HasValue && !string.IsNullOrEmpty(ip))
                                add_ip_if_pass_test(new IP { Ip = ip, Port = port.Value }, "ProxyListFREE");
                        });
                        url = xdoc.Root.GetDescendantByClass("a", "下一页").GetHref();
                        if (string.IsNullOrEmpty(url))
                        {
                            var ptg = xdoc.Root.GetDescendant("td", "class", "td760").GetDescendant("p", "style", "font-size").GetValue();
                            int? pagesNum = ptg.SubstringBefore("&nbsp;下一页").SubLastStringAfter("&nbsp;").ExInt();
                            page++;
                            if(page <= pagesNum)
                            {
                                url = "http://www.haodailiip.com/guonei/{0}".FormatStr(page);
                            }
                        }
                        if (string.IsNullOrEmpty(url))
                            return;                      
                        xdoc = web.GetXDoc(url, null);
                    }

                    Thread.Sleep(120000);
                }
            }));
            t.Start();
        }

        #endregion

        #region Test

        WebHelperNoCookieProxy proxy = new WebHelperNoCookieProxy();      
        string[] testUrls = new string[] { "http://www.baidu.com","http://www.taobao.com"};
        int currrent_test_url_index = 0;
        public string GetTestUrl()
        {
            try
            {
                string test_url = testUrls[currrent_test_url_index];
                currrent_test_url_index++;
                currrent_test_url_index = currrent_test_url_index % testUrls.Length;
                return test_url;
            }
            catch
            {
                return "http://www.163.com";
            }
        }

        #endregion

        #region Log
         
        long avalid_ips = 0;
        long checked_ips = 0; 
        long duplicated_ips = 0;
        public Action<string> msgHandler { get; set; }
        void log(string msg)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var pro in provider_ips)
                    sb.Append(" " + pro.Key + ":" + pro.Value);
                foreach (var pro in providers)
                    sb.Append(" " + pro.name + "[" + pro.total_ips + "/" + pro.get_times + "]");
                msg = DateTime.Now + " : " + msg + " CHK:" + checked_ips
                    + " AVL:" + avalid_ips
                    + " INP:" + avaliable_ips.Count
                    + " DUP:" + duplicated_ips
                   + sb.ToString();
                if (msgHandler != null)
                    msgHandler(msg);
                else
                    Console.WriteLine(msg);
            }
            catch
            { 
            }
        }

        #endregion

        #region Check Ip

        int ip_life_time_seconds = 60*10;

        void add_ip_if_pass_test(IP ip, string provider)
        {
            try
            { 

                checked_ips++;

                DateTime dt = DateTime.Now;
                string test_url = GetTestUrl();
                string html = proxy.GetFastHtml(test_url,ip.Ip, ip.Port,"utf-8",max_timeout);
                if (string.IsNullOrEmpty(html)) 
                    return; 

                int totalMs = (int)(DateTime.Now - dt).TotalMilliseconds;
                if (totalMs > max_timeout) 
                    return;
                ip.TestSpeedMilliseconds = totalMs;                 

                ip.CreatedAt = DateTime.Now;
                avaliable_ips.Push(ip);


                if (!provider_ips.ContainsKey(provider))
                    provider_ips.Add(provider, 0);
                provider_ips[provider]++;
                avalid_ips++;
                log("avalid " + ip + " " + totalMs + "ms");

                //清除掉过时的ip
                var timeout_ips = avaliable_ips.Where(fip => (DateTime.Now - fip.CreatedAt).TotalSeconds >= ip_life_time_seconds).ToList();
                if (timeout_ips != null && timeout_ips.Count > 0)
                {
                    lock (ip_token)
                    {
                        avaliable_ips.Clear();
                        var ips = avaliable_ips.ToArray().Where(fip => (DateTime.Now - fip.CreatedAt).TotalSeconds < ip_life_time_seconds).ToList();
                        if (ips != null && ips.Count > 0)
                        {
                            ips.ForEach(x => avaliable_ips.Push(x));
                        }
                    }
                } 
                while (avaliable_ips.Count > maxsize)
                {
                    Thread.Sleep(1000);
                }

            }
            catch
            {
            }
        }

        #endregion  
    }

    public class IP
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TestSpeedMilliseconds { get; set; }
        bool _is_avaliable=true ;
        public bool is_avaliable { get { return _is_avaliable; } set { _is_avaliable = value; } }
    }
     
}
