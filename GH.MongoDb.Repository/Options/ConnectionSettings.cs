using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Authentication;
using MongoDB.Driver;

namespace GH.MongoDb.Repository.Options
{
    public class ConnectionSettings
    {
        public ConnectionSettings(string dbName, string connString = "localhost", string user = "", string pwd = "", int port = 27017, SslProtocols protocols = SslProtocols.None)
        {
            DbName = dbName;
            ConnectionStringOrServerName = connString;
            Port = port;
            UserName = user;
            Password = pwd;
            SslProtocols = protocols;
        }

        private MongoClientSettings _settings = new MongoClientSettings();

        public string ConnectionStringOrServerName { get; set; }
        public string DbName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool ConnectionStringIsMongoUrl => ConnectionStringOrServerName.Contains("mongodb://");
        public SslProtocols SslProtocols { get; set; }

        public MongoClientSettings ClientSettings
        {
            get
            {

                if (ConnectionStringIsMongoUrl)
                {
                    _settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionStringOrServerName));
                }
                else
                {
                    _settings = new MongoClientSettings() { Server = new MongoServerAddress(ConnectionStringOrServerName) };
                    if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    {
                        var credential = MongoCredential.CreateCredential(DbName, UserName, Password);
                        _settings.Credentials = new List<MongoCredential>() { credential };
                    }
                }
                if (SslProtocols != SslProtocols.None)
                {
                    _settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols };
                }
                return _settings;
            }
        }

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(MongoClientSettings);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this._settings, null);
            }
            set
            {
                Type myType = typeof(MongoClientSettings);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this._settings, value, null);
            }
        }
    }
}