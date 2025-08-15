#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.SQLiteStore;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Store;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.SerialPort;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.EventLogger;
using FTOptix.AuditSigning;
using FTOptix.OPCUAClient;
using FTOptix.MQTTClient;
#endregion

public class Bools_Replay_Query : BaseNetLogic
{
    private Store myStore;
    private object[,] resultSet;
    DateTime dateQuery;
    string itemRtn;

    public override void Start()
    {
    }
    public override void Stop()
    {
    }
    [ExportMethod]
    public void Bools_Query()
    {
     string PLC = Owner.Owner.Owner.Owner.BrowseName;    // "AB_MagnaPak";
     string Bool_Number = Owner.Owner.BrowseName;        // "0";


          myStore = Project.Current.Get<Store>($"DataStores/" + PLC);

        // Build SQL Date Time Query String
         dateQuery = DateTime.Parse(Project.Current.GetVariable($"Deployment/"+PLC+"/Bools/"+Bool_Number+"/Bool/Rewind/Date_Time").Value);
         string sqlFormattedDate = dateQuery.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");




        //  Value
         // Query Data Base for Value
         string queryString = $"SELECT Value FROM Bools_" +Bool_Number + " WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         // Store Value from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/Bools/"+Bool_Number+"/Bool/Rewind/Value").Value = (dynamic)resultSet[0, 0];
        
       
       
    }
}
