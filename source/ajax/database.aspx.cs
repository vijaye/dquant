using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataViz;
using System.IO;

public partial class ajax_database : AjaxPage
{
    int pageLength = 10;

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    [AjaxMethod]
    public double add(double x, double y)
    {
        return x + y;
    }

    [AjaxMethod]
    public JsObject getColumns(string database, string table)
    {
        return DatabaseUtils.GetColumns(database, table);
    }

    [AjaxMethod]
    public long getLength(string database, string table)
    {
        return DatabaseUtils.GetRowCount(database, table);
    }

    [AjaxMethod]
    public List<Dictionary<string, string>> getTables(string database)
    {
        return DatabaseUtils.GetTables(database);
    }

    [AjaxMethod]
    public List<JsObject> getRows(string database, string table, int start, int length)
    {
        return DatabaseUtils.GetRows(database, table, start, length);
    }

    [AjaxMethod]
    public string renderTable(string database, string tableName, int pageNumber)
    {
        return Utils.RenderView("DbTableControl.ascx", new Dictionary<string, object> { 
            { "Database", database }, { "TableName", tableName } 
        });
    }
}