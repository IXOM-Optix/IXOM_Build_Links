#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.CoreBase;
using FTOptix.DataLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
using FTOptix.Core;
using FTOptix.OPCUAClient;
using FTOptix.Modbus;
using FTOptix.OPCUAServer;
using System.ComponentModel;
using FTOptix.EventLogger;
using FTOptix.AuditSigning;
using FTOptix.SerialPort;
using FTOptix.MQTTClient;
#endregion

public class DBQueryTest : BaseNetLogic
{
    private Store myStore;
    private object[,] resultSet;
    DateTime dateQuery;
    string itemRtn;
  
    public override void Start()
    {
        myStore = Project.Current.Get<Store>("DataStores/EmbeddedDatabase1");
    }
    public override void Stop()
    {
    }
    [ExportMethod]
    public void Query()
    {
        dateQuery = DateTime.Parse(Project.Current.GetVariable("Model/DateQuery").Value);
        string sqlFormattedDate = dateQuery.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
        string queryString = $"SELECT Top FROM ResinConcentrations WHERE Time_Stamp <= '" + sqlFormattedDate + "'ORDER BY Time_Stamp DESC LIMIT 2";
        myStore.Query(queryString, out _, out resultSet);
        Project.Current.GetVariable("Model/ItemRtn").Value = (dynamic)resultSet[0, 0];
    }

}
