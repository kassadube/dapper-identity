using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace webapp.Selenium.test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            IWebDriver driver;
            driver = new ChromeDriver("c:\\selenium");
            driver.Navigate().GoToUrl("http://www.wedoqa.com");
 
            Assert.Equal("WeDoQA", driver.Title);
            
            driver.Close();
            driver.Dispose();
        }
    }
}
