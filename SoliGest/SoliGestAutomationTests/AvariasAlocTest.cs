namespace SoliGestAutomationTests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    public class AvariasAlocTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "https://soligest.azurewebsites.net";
        private const string AssistanceUrl = BaseUrl + "/avarias";
        private const string TechnicianEmail = "technician@mail.com";
        private const string TechnicianPassword = "Tech1!";
        private const int DefaultTimeoutSeconds = 5;

        public AvariasAlocTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");
            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void AutoAllocate_Should_Assign_Technician_And_InfoWindow_Shows_Name()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // --- Login ---
            _driver.Navigate().GoToUrl(BaseUrl + "/login");
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();
            wait.Until(ExpectedConditions.AlertIsPresent());
            _driver.SwitchTo().Alert().Accept();

            _driver.Navigate().GoToUrl(AssistanceUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            var panel = _driver.FindElement(By.CssSelector(".panel"));
            var headerText = panel.FindElement(By.CssSelector(".panel-header h3")).Text; // e.g. "Avaria ID: 123"
            var avariaId = headerText.Split(':')[1].Trim();

            // Click 'Alocar Automaticamente'
            panel.FindElement(By.CssSelector("button.auto-aloc")).Click();

            // Wait for auto-allocate modal
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal .modal-content")));

            // Locate the paragraph containing the name
            var allParagraphs = _driver.FindElements(By.CssSelector(".modal .modal-content p"));
            var nameParagraph = allParagraphs.FirstOrDefault(p => p.Text.Contains("Nome:"));
            if (nameParagraph == null)
                throw new Exception("Could not find the paragraph with 'Nome:' in auto-allocate modal.");

            var paragraphText = nameParagraph.Text;
            var nameMatch = Regex.Match(paragraphText, @"Nome:\s*(.+)");
            if (!nameMatch.Success)
                throw new Exception($"Failed to extract candidate name from text: '{paragraphText}'");
            var candidateName = nameMatch.Groups[1].Value.Trim();

            // Confirm auto-allocation
            _driver.FindElement(By.CssSelector(".modal .confirm-btn")).Click();

            // Wait for allocation success alert
            wait.Until(ExpectedConditions.AlertIsPresent());
            var allocAlert = _driver.SwitchTo().Alert();
            Assert.Contains(candidateName, allocAlert.Text);
            allocAlert.Accept();

            // Wait for page reload: panels reappear
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Click 'Consultar' on the same avaria panel
            // Find the specific panel by its header text
            var panels = _driver.FindElements(By.CssSelector(".panel"));
            var targetPanel = panels.First(p =>
                p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains($"Avaria ID: {avariaId}"));
            // Click the view/consult button
            targetPanel.FindElement(By.CssSelector("button.view-btn")).Click();

            // Wait for Google Maps InfoWindow and verify name
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".gm-style-iw")));
            var infoWindow = _driver.FindElement(By.CssSelector(".gm-style-iw"));
            Assert.Contains(candidateName, infoWindow.Text);
        }

        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                System.Threading.Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
