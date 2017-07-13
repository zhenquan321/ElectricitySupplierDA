using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AISSystem;



namespace kuerjiudian
{

    public class IprIdentify : IIdentity
    {
        string authentication_type;
        public string AuthenticationType
        {
            get
            {
                return authentication_type;
            }
            set
            {
                authentication_type = value;
            }
        }

        bool isAuthenticated = false;
        public bool IsAuthenticated
        {
            get
            {
                return isAuthenticated;
            }
            set
            {
                isAuthenticated = value;
            }
        }

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string role;
        public string Role
        {
            get { return role; }
            set { role = value; }
        }
    }

    public class IPRPrincipal : IPrincipal
    {
        IprIdentify id = new IprIdentify();
        public IIdentity Identity
        {
            get
            {
                return id;
            }
            set
            {
                id = (IprIdentify)value;
            }
        }

        public bool IsInRole(string role)
        {
            return id != null && id.Role == role;
        }

        public static IPRPrincipal CreatePrinceipal(string name, string role)
        {
            IprIdentify id = new IprIdentify { IsAuthenticated = true, Name = name, Role = role };
            IPRPrincipal p = new IPRPrincipal();
            p.Identity = id;
            return p;
        }
    }

    public class IprAuthorizeAttribute : ActionFilterAttribute
    {
        string _role = null;
        public IprAuthorizeAttribute()
        { 
        }

        public IprAuthorizeAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            IEnumerable<string> values;
            if (!actionContext.Request.Headers.TryGetValues("Authorization", out values) 
                || values ==null || values.Count()==0
                || !values.Any(x=>Valid(x)))
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                return;
            }
            base.OnActionExecuting(actionContext);
        }

        static string secret_key = "&*LJIY(ghsnjj@#";
        static string salt = "h~ui123jkoh(*&";
        public static string GetToken(string usr,int? role)
        {
            Enums.Role r = (Enums.Role)role;
            usr = "{0}$;{1}$;{2}".FormatStr(usr,r, DateTime.Now.ToDateKey2());
            usr = CryptHelper.EncryptAES(usr, secret_key, salt);
            return usr;
        }

        public  bool Valid(string token)
        {
            try
            {
                token = CryptHelper.DecryptAES(token, secret_key, salt);
                var role = token.SubAfter("$;").SubBefore("$;");
                var dk = token.SubAfter("$;").SubAfter("$;").ToDouble();
                return dk.HasValue && DateTime.Now.ToDateKey() == (int)dk.Value 
                    &&(string.IsNullOrEmpty(_role)||role ==_role);
            }
            catch
            {
                return false;
            }
        }
    }


}