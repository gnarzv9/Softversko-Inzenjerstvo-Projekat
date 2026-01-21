using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace InsomniaShop.SeleniumTests
{
    public class ShopSeleniumTests
    {
        private IWebDriver? driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void ShopPage_Loads()
        {
            driver!.Navigate().GoToUrl("http://127.0.0.1:5500/front/index.html");
            Assert.IsNotNull(driver.Title);

        }

        [Test]
        public void Games_Are_Loaded_From_Backend()
        {
            driver!.Navigate().GoToUrl("http://127.0.0.1:5500/front/index.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            var games = wait.Until(d => d.FindElements(By.ClassName("game-card")));

            Assert.IsTrue(games.Count > 0);
        }

        [Test]
        public void Can_Add_Game_To_Cart()
        {
            driver!.Navigate().GoToUrl("http://127.0.0.1:5500/front/index.html");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.XPath("//button[text()='Buy']"))).Click();

            var cart = wait.Until(d => d.FindElement(By.Id("cart-items")));
            Assert.IsNotNull(cart);
        }

        

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}