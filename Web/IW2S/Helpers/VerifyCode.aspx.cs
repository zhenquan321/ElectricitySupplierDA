using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IW2S.Helpers
{
    public partial class VerifyCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetNoStore();
            VerifyCodeClass validatephoto = new VerifyCodeClass();
            validatephoto.FontSize = 18;
            string validatenumber = validatephoto.CreateVerifyCode(4);
            Session["_ValidateCode"] = validatenumber;
            validatephoto.CreateImageOnPage(validatenumber, true, 3, 4, this.Context, false, this.Page);     
          
        }
    }
}