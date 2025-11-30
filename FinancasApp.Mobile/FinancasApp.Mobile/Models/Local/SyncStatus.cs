using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Models/Local/SyncStatus.cs
namespace FinancasApp.Mobile.Models.Local;

public enum SyncStatus
{
    Synced = 0,
    Pending = 1,
    Conflict = 2,
    Deleted = 3
}
