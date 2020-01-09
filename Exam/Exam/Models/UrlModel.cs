using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Npgsql;

namespace Exam.Models
{
    public class UrlModel
    {
        public readonly string Domain;
        public readonly string Url;

        private UrlModel(string domain, string url)
        {
            Domain = domain;
            Url = url;
        }

        public UrlModel()
        {
        }

        public async Task AppendToDomain(string domain, IEnumerable<string> urls)
        {
            var _dbConnection =
                new NpgsqlConnection(
                    "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password");
            using (_dbConnection)
            {
                _dbConnection.Open();
                foreach (var url in urls)
                {
                    var sqlCommand =
                        string.Format(
                            "Insert into \"Exam\" (\"Domain\",\"Url\") values (\'{0}\',\'{1}\')",
                            domain, url);
                    var _dbCommand = new NpgsqlCommand(sqlCommand, _dbConnection);
                    _dbCommand.ExecuteNonQuery();
                }
            }

            _dbConnection.Close();
        }

        public async Task<IEnumerable<UrlModel>> GetAllUrls(string domain)
        {
            var urls = new List<UrlModel>();
            var _dbConnection =
                new NpgsqlConnection(
                    "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password");
            using (_dbConnection)
            {
                _dbConnection.Open();
                var sqlCommand = string.Format("Select distinct * from \"Exam\" WHERE \"Domain\"=\'{0}\'", domain);
                var _dbCommand = new NpgsqlCommand(sqlCommand, _dbConnection);
                var reader = await _dbCommand.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var url = reader.GetString(1);
                        var model = new UrlModel(domain, url);
                        urls.Add(model);
                    }
                }

                reader.Close();
            }

            _dbConnection.Close();
            return urls;
        }
    }
}