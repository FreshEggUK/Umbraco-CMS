using NUnit.Framework;
using Umbraco.Cms.Core.Install.Models;
using Umbraco.Cms.Persistence.SqlServer.Services;

namespace Umbraco.Cms.Tests.UnitTests.Umbraco.Cms.Persistence.SqlServer;

[TestFixture]
public class SqlLocalDbDatabaseProviderMetadataTests
{
    [Test]
    [TestCase("ignored", "myDatabase", "ignored", "ignored", true, ExpectedResult = "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\myDatabase.mdf;Integrated Security=True;TrustServerCertificate=True")]
    [TestCase("ignored", "myDatabase2", "ignored", "ignored", false /*ignored*/, ExpectedResult = "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\myDatabase2.mdf;Integrated Security=True;TrustServerCertificate=True")]
    public string GenerateConnectionString(string server, string databaseName, string login, string password, bool integratedAuth)
    {
        var sut = new SqlLocalDbDatabaseProviderMetadata();
        return sut.GenerateConnectionString(new DatabaseModel()
        {
            DatabaseName = databaseName,
            Login = login,
            Password = password,
            Server = server,
            IntegratedAuth = integratedAuth,
        });
    }

    [Test]
    [TestCase("Server=myServer;Database=myDatabase;Integrated Security=true", ExpectedResult = false)] // SqlServer
    [TestCase("Server=myServer;Database=myDatabase;User Id=myLogin;Password=myPassword", ExpectedResult = false)] // SqlServer
    [TestCase("Server=tcp:cmstest27032000.database.windows.net,1433;Database=test_27032000;User ID=asdasdas@cmstest27032000;Password=123456879", ExpectedResult = false)] // Azure
    [TestCase("Data Source=|DataDirectory|/Umbraco.sqlite.db;Cache=Shared;Foreign Keys=True;Pooling=True", ExpectedResult = false)] // Sqlite
    [TestCase("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\\umbraco.mdf", ExpectedResult = true)] // localDB
    public bool CanRecognizeConnectionString(string connectionString)
    {
        var sut = new SqlLocalDbDatabaseProviderMetadata();
        return sut.CanRecognizeConnectionString(connectionString);
    }
}
