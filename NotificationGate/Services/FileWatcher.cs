using Confluent.Kafka;
using Microsoft.Extensions.Options;
using NotificationGate.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationGate.Services
{
    public class FileWatcher
    {
        private readonly PathSettings _path;
        private readonly KafkaService _kafkaService;
      
        public FileWatcher(KafkaService kafkaService, IOptions<PathSettings> path)
        {
            _kafkaService = kafkaService;
            _path = path.Value;
        }
        public async Task Manager()
        {
            Console.WriteLine($"the path: {_path.Path}");
            //using var watcher = new FileSystemWatcher(_path.Path);
            using var watcher = new FileSystemWatcher(@"C:\Users\elazar\KolAman\alert-simulator\alerts");
            Console.WriteLine("after creating");
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Created += OnCreated;


            watcher.Filter = "*.ready";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
        public async void OnCreated(object sender, FileSystemEventArgs e)
        {
            string path = Path.ChangeExtension(e.FullPath, ".json");
            Console.WriteLine(path);
            string value = File.ReadAllText(path);
            Message<string, string> message = new();
            message.Value = value;
            await _kafkaService.SendToKafka(message);
        }
    }
}
