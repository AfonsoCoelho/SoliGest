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

            // Navigate to 'Avarias'
            _driver.Navigate().GoToUrl(AssistanceUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Select first avaria panel
            var panel = _driver.FindElement(By.CssSelector(".panel"));
            var headerText = panel.FindElement(By.CssSelector(".panel-header h3")).Text;
            var avariaId = headerText.Split(':')[1].Trim();

            // Click 'Alocar Automaticamente'
            panel.FindElement(By.CssSelector("button.auto-aloc")).Click();

            // Wait for auto-allocate modal
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal .modal-content")));

            // Extract candidate name
            var nameParagraph = _driver.FindElements(By.CssSelector(".modal .modal-content p"))
                .FirstOrDefault(p => p.Text.Contains("Nome:"));
            if (nameParagraph == null)
                throw new Exception("Could not find 'Nome:' paragraph in auto-allocate modal.");
            var match = Regex.Match(nameParagraph.Text, @"Nome:\s*(.+)");
            var autoName = match.Success ? match.Groups[1].Value.Trim() : throw new Exception("Failed to extract auto candidate name.");

            // Confirm auto-allocation
            _driver.FindElement(By.CssSelector(".modal .confirm-btn")).Click();
            wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();
            Assert.Contains(autoName, alert.Text);
            alert.Accept();

            // Reload panels
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Click 'Consultar' on allocated panel
            var panels = _driver.FindElements(By.CssSelector(".panel"));
            var target = panels.First(p => p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains($"Avaria ID: {avariaId}"));
            target.FindElement(By.CssSelector("button.view-btn")).Click();

            // Verify InfoWindow shows name
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".gm-style-iw")));
            var info = _driver.FindElement(By.CssSelector(".gm-style-iw")).Text;
            Assert.Contains(autoName, info);
        }

        [Fact]
        public void ManualAllocate_Should_Assign_Technician_And_InfoWindow_Shows_Name_For_Id2()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // Login
            _driver.Navigate().GoToUrl(BaseUrl + "/login");
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();
            wait.Until(ExpectedConditions.AlertIsPresent()).Accept();

            // Navigate to Avarias
            _driver.Navigate().GoToUrl(AssistanceUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Manual-allocate avaria ID 2
            var panel2 = _driver.FindElements(By.CssSelector(".panel")).First(p => p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains("Avaria ID: 2"));
            panel2.FindElement(By.CssSelector("button.manual-aloc")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal .table-container tbody tr")));

            // Pick second technician
            var rows = _driver.FindElements(By.CssSelector(".modal .table-container tbody tr"));
            if (rows.Count < 2) throw new Exception("Less than two technicians available.");
            var secondRow = rows[1];
            var manualName = secondRow.FindElement(By.CssSelector("td:nth-child(2)")).Text.Trim();
            secondRow.FindElement(By.CssSelector("button.allocate-btn")).Click();

            // Verify allocation alert
            wait.Until(ExpectedConditions.AlertIsPresent());
            var manualAlert = _driver.SwitchTo().Alert();
            Assert.Contains(manualName, manualAlert.Text);
            manualAlert.Accept();

            // View InfoWindow for panel 2
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));
            var target2 = _driver.FindElements(By.CssSelector(".panel")).First(p => p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains("Avaria ID: 2"));
            target2.FindElement(By.CssSelector("button.view-btn")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".gm-style-iw")));
            var manualInfo = _driver.FindElement(By.CssSelector(".gm-style-iw")).Text;
            Assert.Contains(manualName, manualInfo);
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
