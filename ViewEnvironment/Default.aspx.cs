using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;

public partial class _Default : System.Web.UI.Page
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get environment variables and dump them
        IDictionary vars = System.Environment.GetEnvironmentVariables();
        System.Environment.GetEnvironmentVariables();

        lblTime.Text = DateTime.Now.ToString();
        lblDotNetVersion.Text = Environment.Version.ToString();
        lblPort.Text = Environment.GetEnvironmentVariable("PORT");
        lblInstanceID.Text = Environment.GetEnvironmentVariable("INSTANCE_GUID");
        lblInstanceIndex.Text = Environment.GetEnvironmentVariable("INSTANCE_INDEX");
        lblInstanceStart.Text =  DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Environment.TickCount)).ToString();
        lblBoundServices.Text = Environment.GetEnvironmentVariable("VCAP_SERVICES");
        
        Response.Write("Environement Varibles<br>");
        foreach (DictionaryEntry entry in vars)
        {
            // add to querystring all to dump all environment variables
            
                Response.Write("<pre>&#9;" + entry.Key + " = " + entry.Value + "</pre>");
        }

        Response.Write("Installed .NET versions<br>");
        foreach(Version version in InstalledDotNetVersions()){
            Response.Write("<pre>&#9;" +version + "</pre>");
        }
    }
    protected void btnKill_Click(object sender, EventArgs e)
    {
        log("Kaboom.");
        Environment.Exit(-1);
    }

    private void log(string message)
    {
        Console.WriteLine(message);
    }
    public static Collection<Version> InstalledDotNetVersions()
    {
        Collection<Version> versions = new Collection<Version>();
        RegistryKey NDPKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
        if (NDPKey != null)
        {
            string[] subkeys = NDPKey.GetSubKeyNames();
            foreach (string subkey in subkeys)
            {
                GetDotNetVersion(NDPKey.OpenSubKey(subkey), subkey, versions);
                GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Client"), subkey, versions);
                GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Full"), subkey, versions);
            }
        }
        return versions;
    }

    private static void GetDotNetVersion(RegistryKey parentKey, string subVersionName, Collection<Version> versions)
    {
        if (parentKey != null)
        {
            string installed = Convert.ToString(parentKey.GetValue("Install"));
            if (installed == "1")
            {
                string version = Convert.ToString(parentKey.GetValue("Version"));
                if (string.IsNullOrEmpty(version))
                {
                    if (subVersionName.StartsWith("v"))
                        version = subVersionName.Substring(1);
                    else
                        version = subVersionName;
                }

                Version ver = new Version(version);

                if (!versions.Contains(ver))
                    versions.Add(ver);
            }
        }
    }
}