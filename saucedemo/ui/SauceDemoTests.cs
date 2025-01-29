using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace SaucedemoTests
{
    public class Config
    {
        public string Url { get; set; }
        public Login Login { get; set; }
        public Filter Filter { get; set; }
        public Product Product { get; set; }
    }

    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Filter
    {
        public string Type { get; set; }
    }

    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
    }

    [TestFixture]
    public class SaucedemoTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private Config _config;

        [SetUp]
        public void SetUp()
        {
            _config = LoadConfig("config.json");
            _driver = new ChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _driver.Manage().Window.Maximize();
        }

        [Test]
        public void TestSauceDemo()
        {
            _driver.Navigate().GoToUrl(_config.Url);
            _driver.FindElement(By.Id("user-name")).SendKeys(_config.Login.Username);
            _driver.FindElement(By.Id("password")).SendKeys(_config.Login.Password);
            _driver.FindElement(By.Id("login-button")).Click();
            _wait.Until(d => d.FindElement(By.ClassName("product_sort_container")));

            var filterDropdown = _driver.FindElement(By.ClassName("product_sort_container"));
            filterDropdown.Click();
            
            _wait.Until(d => d.FindElement(By.XPath($"//option[text()='{_config.Filter.Type}']"))).Click();
            _wait.Until(d => d.FindElements(By.ClassName("inventory_item")).Any());

            var productElement = _driver.FindElement(By.XPath($"//div[contains(@class, 'inventory_item') and .//div[text()='{_config.Product.Name}']]"));
            var productName = productElement.FindElement(By.ClassName("inventory_item_name")).Text;
            var productPrice = productElement.FindElement(By.ClassName("inventory_item_price")).Text;

            Assert.AreEqual(_config.Product.Name, productName, "Назва товару не збігається.");
            Assert.AreEqual($"${_config.Product.Price}", productPrice, "Ціна товару не збігається.");

            productElement.Click();
            _wait.Until(d => d.FindElement(By.ClassName("inventory_details_name")));

            var detailName = _driver.FindElement(By.ClassName("inventory_details_name")).Text;
            var detailPrice = _driver.FindElement(By.ClassName("inventory_details_price")).Text;

            Assert.AreEqual(_config.Product.Name, detailName, "Назва товару на сторінці деталей не збігається.");
            Assert.AreEqual($"${_config.Product.Price}", detailPrice, "Ціна товару на сторінці деталей не збігається.");

            _driver.FindElement(By.ClassName("btn_inventory")).Click();
            _wait.Until(d => d.FindElement(By.ClassName("shopping_cart_badge")).Text == "1");

            var cartCount = _driver.FindElement(By.ClassName("shopping_cart_badge")).Text;
            Assert.AreEqual("1", cartCount, "Кількість товарів у кошику не дорівнює 1.");

            _driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            _wait.Until(d => d.FindElement(By.ClassName("inventory_item_name")));

            var cartProductName = _driver.FindElement(By.ClassName("inventory_item_name")).Text;
            var cartProductPrice = _driver.FindElement(By.ClassName("inventory_item_price")).Text;

            Assert.AreEqual(_config.Product.Name, cartProductName, "Назва товару в кошику не збігається.");
            Assert.AreEqual($"${_config.Product.Price}", cartProductPrice, "Ціна товару в кошику не збігається.");
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        private Config LoadConfig(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
