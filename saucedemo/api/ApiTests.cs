using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;
using System;

namespace ApiTests
{
    [TestFixture]
    public class ClientApiTests
    {
        private ApiSettings _apiSettings;

        [SetUp]
        public void Setup()
        {
            _apiSettings = ConfigurationHelper.GetApiSettings();
        }

        [Test]
        public void CreateClient_Success_Returns200()
        {
            var client = new RestClient(_apiSettings.BaseUrl);
            var request = new RestRequest(_apiSettings.Endpoint, Method.POST);
            request.AddHeader("Content-Type", _apiSettings.ContentType);

            var requestBody = new
            {
                name = "Bob Doe",
                email = "bob.doe@example.com",
                balance = 100
            };

            request.AddJsonBody(requestBody);

            var response = client.Execute(request);

            Assert.AreEqual(200, (int)response.StatusCode);

            var responseBody = JObject.Parse(response.Content);
            Assert.IsTrue(responseBody.ContainsKey("id"));
            Assert.AreEqual("Bob Doe", responseBody["name"].ToString());
            Assert.AreEqual("bob.doe@example.com", responseBody["email"].ToString());
            Assert.AreEqual(100, (int)responseBody["balance"]);
        }

        [Test]
        public void CreateClient_InvalidBalance_Returns400()
        {
            var client = new RestClient(_apiSettings.BaseUrl);
            var request = new RestRequest(_apiSettings.Endpoint, Method.POST);
            request.AddHeader("Content-Type", _apiSettings.ContentType);

            var requestBody = new
            {
                name = "Bob Doe",
                email = "bob.doe@example.com",
                balance = "abc"
            };

            request.AddJsonBody(requestBody);

            var response = client.Execute(request);

            Assert.AreEqual(400, (int)response.StatusCode);
            var responseBody = JObject.Parse(response.Content);
            Assert.AreEqual("Balance must be an integer", responseBody["error"].ToString());
        }

        [Test]
        public void CreateClient_MissingName_Returns400()
        {
            var client = new RestClient(_apiSettings.BaseUrl);
            var request = new RestRequest(_apiSettings.Endpoint, Method.POST);
            request.AddHeader("Content-Type", _apiSettings.ContentType);

            var requestBody = new
            {
                email = "bob.doe@example.com",
                balance = 100
            };

            request.AddJsonBody(requestBody);

            var response = client.Execute(request);

            Assert.AreEqual(400, (int)response.StatusCode);
            var responseBody = JObject.Parse(response.Content);
            Assert.AreEqual("Name is required", responseBody["error"].ToString());
        }

        [Test]
        public void CreateClient_MissingBalance_DefaultsToZero()
        {
            var client = new RestClient(_apiSettings.BaseUrl);
            var request = new RestRequest(_apiSettings.Endpoint, Method.POST);
            request.AddHeader("Content-Type", _apiSettings.ContentType);

            var requestBody = new
            {
                name = "Bob Doe",
                email = "bob.doe@example.com"
            };

            request.AddJsonBody(requestBody);

            var response = client.Execute(request);

            Assert.AreEqual(200, (int)response.StatusCode);

            var responseBody = JObject.Parse(response.Content);
            Assert.AreEqual(0, (int)responseBody["balance"]);
        }
    }
}
