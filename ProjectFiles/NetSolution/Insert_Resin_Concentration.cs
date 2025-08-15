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
using FTOptix.OPCUAClient;
using FTOptix.MQTTClient;
#endregion

public class Insert_Resin_Concentration : BaseNetLogic
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




















public void Insert_RC(string DataStore, DateTime LocalTimestamp, int Contactor, int Top, int Upper, int Lower, int Bottom) {
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
            //Use Current Time stamp
                //rawValues[0, 0] = DateTime.UtcNow;
                //Project.Current.GetVariable("Model/Resin_Concentrations/Time_Stamp").Value = DateTime.UtcNow;

            // Use Variable for Time Stamp
                string date = Project.Current.GetVariable("Model/Resin_Concentrations/LocalTimestamp").Value;
                rawValues[0, 0] = DateTime.Parse(date);

        // Column VariableToLog1
        //rawValues[0, 1] = (string)Project.Current.GetVariable("Model/Resin_Concentrations/Contactor").Value;
        rawValues[0, 1] = Contactor;

        // Column VariableToLog2
        //rawValues[0, 2] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Top").Value;
        rawValues[0, 2] = Top;

        // Column VariableToLog3
        //rawValues[0, 3] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Upper").Value;
        rawValues[0, 3] = Upper;

        // Column VariableToLog4
        //rawValues[0, 4] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Lower").Value;
        rawValues[0, 4] = Lower;

         // Column VariableToLog5
        //rawValues[0, 5] = (int)Project.Current.GetVariable("Model/Resin_Concentrations/Bottom").Value;
        rawValues[0, 5] = Bottom;

        // @Erik  want to use DataStore String to link to Datastores/SIE_MagnaPak
        var myStore = LogicObject.Owner as Store;

        // Get Table1 from myStore
        var table1 = myStore.Tables.Get<Table>("ResinConcentrations");

        // Insert values into table1
        table1.Insert(columns, rawValues);
    }



}
