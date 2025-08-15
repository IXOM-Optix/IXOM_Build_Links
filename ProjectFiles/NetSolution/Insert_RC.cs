#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
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
using FTOptix.OPCUAClient;
using FTOptix.EventLogger;
using FTOptix.MQTTClient;
#endregion

public class Insert_RC : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void Method1(
        string DataBaseName, 
        string LocalTimestamp, 
        float Contactor,
        float Top,
        float Upper,
        float Lower,
        float Bottomup )
    {
        // Get the current project folder.
         var project = Project.Current;


        // Save the names of the columns of the table to an array
        string[] columns = { 
            "LocalTimestamp", 
            "Contactor", 
            "Top", 
            "Upper", 
            "Lower", 
            "Bottom" };

        // Create and populate a matrix with values to insert into the odbc table
         var rawValues = new object[1, 6];

        // Column TimeStamp


        // Use Variable for Time Stamp
         string date = Project.Current.GetVariable("Model/Resin_Concentrations/LocalTimestamp").Value;
         rawValues[0, 0] = DateTime.Parse(date);

        // Column VariableToLog1
         rawValues[0, 1] = (string)Project.Current.GetVariable("Model/Resin_Concentrations/Contactor").Value;

        // Column VariableToLog2
         rawValues[0, 2] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Top").Value;

        // Column VariableToLog3
         rawValues[0, 3] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Upper").Value;

        // Column VariableToLog4
         rawValues[0, 4] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Lower").Value;

        // Column VariableToLog5
        rawValues[0, 5] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Bottom").Value;

        
        // Set Store  @ Erik this is using Owner Reference I need to change to use variable ("/DataStores/" + AB_MagnaPak )
        var myStore = LogicObject.Owner as Store;

        // Get Table1 from myStore
        var table1 = myStore.Tables.Get<Table>("ResinConcentrations");

        // Insert values into table1
        table1.Insert(columns, rawValues);
    }
}
