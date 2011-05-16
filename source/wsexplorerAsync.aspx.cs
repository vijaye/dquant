using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Services.Description;
using System.IO;
using System.Xml.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using DataViz;

public partial class wsexplorerAsync : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.Clear();
        var op = Request["op"];
        switch (op.ToLowerInvariant())
        {
            case "getServiceDetails":
                GetServiceDetails();
                break;

            case "getmethods":
                GetMethods();
                break;
        }
    }

    void GetMethods() { }

    void GetServiceDetails()
    {
        var url = Request["service"];
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        if (!url.ToLowerInvariant().EndsWith("?wsdl"))
            url += "?wsdl";

        try
        {
            using (WebClient wc = new WebClient())
            {
                string content = wc.DownloadString(url);
                var des = wc.DownloadString(url);

                var sd = ServiceDescription.Read(new StringReader(des));
                ServiceDescriptionImporter im = new ServiceDescriptionImporter();
                im.AddServiceDescription(sd, null, null);
                im.Style = ServiceDescriptionImportStyle.Client;
                im.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;

                CodeNamespace ns = new CodeNamespace();
                CodeCompileUnit unit = new CodeCompileUnit();
                unit.Namespaces.Add(ns);

                ServiceDescriptionImportWarnings warning = im.Import(ns, unit);

                if (warning == 0)
                {
                    var domProvider = CodeDomProvider.CreateProvider("CSharp");
                    var assemblyReferences = new string[] { "System.Web.Services.dll", "System.Xml.dll" };
                    var parms = new CompilerParameters(assemblyReferences);
                    var results = domProvider.CompileAssemblyFromDom(parms, unit);

                    if (results.Errors.Count > 0)
                    {
                        Response.Output.WriteLine("{ 'error' : [");
                        foreach (CompilerError ce in results.Errors)
                        {
                            Response.Output.WriteLine("'{0}',", Utils.EscapeString(ce.ErrorText));
                        }
                        Response.Output.WriteLine("]}");
                    }

                    object o = results.CompiledAssembly.CreateInstance("WebService");
                    Type t = o.GetType();
                }
                else
                {
                    Console.WriteLine("Warning: " + warning);
                }
            }
        }
        catch (Exception e)
        {
            Response.Output.WriteLine("{{ 'error' : '{0}' }}", Utils.EscapeString(e.Message));
        }
    }
}