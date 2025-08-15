#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.SQLiteStore;
using FTOptix.MQTTClient;
#endregion

public class TimePurgev1 : BaseNetLogic
{
    Store dataStore;
    int pollingTime;
    Double peristenceInterval;
    Boolean enabled;
    private PeriodicTask myPeriodicTask;
    private LongRunningTask myLongRunningTask;

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        dataStore = (Store)Owner;
        pollingTime = LogicObject.GetVariable("PollingTime").Value;
        Log.Info($"Polling Time; {pollingTime} ms");
        myPeriodicTask = new PeriodicTask(ltr_AutoPurge, pollingTime, LogicObject);
        myPeriodicTask.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        myPeriodicTask?.Dispose();
        myLongRunningTask?.Dispose();
    }

    private void ltr_AutoPurge()
    {
        enabled = LogicObject.GetVariable("Enabled").Value;

        if (enabled)
        {
            myLongRunningTask = new LongRunningTask(autoPurge, LogicObject);
            myLongRunningTask.Start();
        }
    }

    private void autoPurge()
    {
        peristenceInterval = LogicObject.GetVariable("RecordPersistenceInterval").Value;
        var tables = dataStore.Tables;
        foreach (var table in tables )
        {
            if (table.RecordLimit == 0)
            {
                DateTime purgeDate = DateTime.Now.AddMilliseconds(-peristenceInterval);
                datePurge(table.NodeId, purgeDate);
            }
        }
        myLongRunningTask?.Dispose();
    }

    [ExportMethod]
    public void datePurge(NodeId table, DateTime date)
    {
        string tableName = InformationModel.Get(table).BrowseName;
        string[] header = null;
        object[,] data = null;
        string query = $"DELETE FROM {tableName} WHERE LocalTimestamp < \"{date.ToString("yyyy-MM-ddTHH:mm:ss.fff")}\"";
        dataStore = (Store)InformationModel.Get(table).Owner.Owner;
        dataStore.Query(query, out header, out data);
    }
}
