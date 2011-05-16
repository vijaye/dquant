using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataViz;
using System.IO;

public partial class ajax_odata : AjaxPage
{
    int pageLength = 10;

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    [AjaxMethod]
    public List<Dictionary<string, object>> getRows(string url, int start, int length)
    {
        return ODataUtils.GetRows(url, start, length);
    }
}