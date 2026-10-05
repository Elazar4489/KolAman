using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationGate.Services
{
    public interface IFileWatcher
    {
        public Task Manager();
        public void OnCreated(object sender, FileSystemEventArgs e);
    }
}
