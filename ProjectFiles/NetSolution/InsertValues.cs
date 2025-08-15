#region Using directives
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.Store;
using System;
using UAManagedCore;
using FTOptix.UI;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using FTOptix.System;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.EventLogger;
using FTOptix.AuditSigning;
using FTOptix.OPCUAClient;
using FTOptix.SerialPort;
using FTOptix.MQTTClient;
#endregion

public class InsertValues : BaseNetLogic {
    public override void Start() {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop() {
        // Insert code to be executed when the user-defined logic is stopped
    }
    [ExportMethod]
    public void InsertRowValues() {
        // Get the current project folder.
        var project = Project.Current;

        // Save the names of the columns of the table to an array
        string[] columns = { "Time_Stamp", "Contactor", "Top", "Second_Top", "Second_Bottom", "Bottom" };

        // Create and populate a matrix with values to insert into the odbc table
        var rawValues = new object[1, 6];

        // Column TimeStamp
        //rawValues[0, 0] = DateTime.UtcNow;
        string date = Project.Current.GetVariable("Model/Time_Stamp").Value;
        rawValues[0, 0] = DateTime.Parse(date);

        // Column VariableToLog1
        rawValues[0, 1] = (string)Project.Current.GetVariable("Model/Contactor").Value;

        // Column VariableToLog2
        rawValues[0, 2] = (int)Project.Current.GetVariable("Model/Top").Value;

        // Column VariableToLog3
        rawValues[0, 3] = (int)Project.Current.GetVariable("Model/Second_Top").Value;

        // Column VariableToLog4
        rawValues[0, 4] = (int)Project.Current.GetVariable("Model/Second_Bottom").Value;

         // Column VariableToLog5
        rawValues[0, 5] = (int)Project.Current.GetVariable("Model/Bottom").Value;

        var myStore = LogicObject.Owner as Store;

        // Get Table1 from myStore
        var table1 = myStore.Tables.Get<Table>("ResinConcentrations");

        // Insert values into table1
        table1.Insert(columns, rawValues);
    }
}
