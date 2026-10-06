using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationComponent.Services
{
    public class ElasticService
    {
        private readonly ElasticsearchClient _elasticClient;
        public ElasticService(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
    }
}
