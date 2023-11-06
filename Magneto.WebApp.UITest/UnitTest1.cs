using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Magneto.WebApp.UITest
{
    [TestFixture]
    public class UITests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }


        //Register with Valid Credentials
        //Change Password after login
        [Test]
        public void RegisterWithValidCredentialsAndChangePassword_ShouldSucceed()
        {
            string domain = "example.com";
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            string email = $"user{uniqueId}@{domain}";

            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/create/");

            FindElementWithWait(driver,By.Id("firstname")).SendKeys(uniqueId);
            FindElementWithWait(driver,By.Id("lastname")).SendKeys(uniqueId);
            FindElementWithWait(driver,By.Id("email_address")).SendKeys(email);
            FindElementWithWait(driver,By.Id("password")).SendKeys("Pass12word3!");
            FindElementWithWait(driver, By.Id("password-confirmation")).SendKeys("Pass12word3!");
            IWebElement RegBtn = FindElementWithWait(driver, By.CssSelector("button[title='Create an Account']"));
            RegBtn.Click();

            //Wait for Registration message
            FindElementWithWait(driver, By.XPath("//div[text()='Thank you for registering with Main Website Store.']"));

            FindElementWithWait(driver,By.XPath("//a[contains(text(),'Change Password')]")).Click();

            //change password form
            FindElementWithWait(driver,By.Id("current-password")).SendKeys("Pass12word3!");
            FindElementWithWait(driver,By.Id("password")).SendKeys("Pass12word3!13");
            FindElementWithWait(driver,By.Id("password-confirmation")).SendKeys("Pass12word3!13");
            FindElementWithWait(driver,By.CssSelector("button[title='Save']")).Click();

            //Verify Info saved message
            Assert.IsTrue(FindElementWithWait(driver,By.XPath("//div[text()='You saved the account information.']")).Displayed);
           
        }

        [Test]
        public void RegisterWithBlankFields_ShouldFail()
        {
            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/create/");

            FindElementWithWait(driver, By.Id("email_address")).SendKeys("existingemail@example.com");
            FindElementWithWait(driver, By.Id("password")).SendKeys("password123");
            
            IWebElement RegBtn = FindElementWithWait(driver, By.CssSelector("button[title='Create an Account']"));
            RegBtn.Click();

            Assert.IsTrue(FindElementWithWait(driver,By.XPath("//div[text()='This is a required field.']")).Displayed);
        }

        [Test]
        public void RegisterWithExistingEmail_ShouldFail()
        {
            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/create/");

            // Fill in the registration form with an existing email address
            FindElementWithWait(driver,By.Id("firstname")).SendKeys("Existing");
            FindElementWithWait(driver,By.Id("lastname")).SendKeys("Email");
            FindElementWithWait(driver,By.Id("email_address")).SendKeys("existingemail@example.com");
            FindElementWithWait(driver,By.Id("password")).SendKeys("Pass12word3!");
            FindElementWithWait(driver,By.Id("password-confirmation")).SendKeys("Pass12word3!");
            IWebElement RegBtn = FindElementWithWait(driver, By.CssSelector("button[title='Create an Account']"));
            RegBtn.Click();

            Assert.IsTrue(FindElementWithWait(driver,By.XPath("//*[contains(text(),'There is already an account with this email address')]")).Displayed);
        }

        [Test]
        public void LoginAndLogoutWithValidCredentials_ShouldSucceed()
        {
            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");

            //Login
            FindElementWithWait(driver,By.Id("email")).SendKeys("existingemail@example.com");
            FindElementWithWait(driver, By.Id("pass")).SendKeys("Pass12word3!");
            FindElementWithWait(driver, By.CssSelector("#send2")).Click();

            //Verify My Account page after login
            Assert.IsTrue(FindElementWithWait(driver, By.XPath("//span[text()='My Account']")).Displayed);

            //Logout
            FindElementWithWait(driver, By.XPath("(//span[contains(text(),'guest post')]/following::button)[1]")).Click();
            FindElementWithWait(driver, By.XPath("(//a[contains(text(),'Sign Out')])[1]")).Click();

            //Verify signout
            Assert.IsTrue(FindElementWithWait(driver, By.XPath("//*[contains(text(),'You are signed out')]")).Displayed);
        }

        [Test]
        public void LoginWithInvalidCredentials_ShouldFail()
        {
            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");

            //Login with invalid id
            FindElementWithWait(driver, By.Id("email")).SendKeys("invalidemail@example.com");
            FindElementWithWait(driver, By.Id("pass")).SendKeys("invalidpassword");
            FindElementWithWait(driver, By.CssSelector("#send2")).Click();

            //Check err message
            Assert.IsTrue(FindElementWithWait(driver,By.XPath("//*[contains(text(),'The account sign-in was incorrect or your account is disabled temporarily')]")).Displayed);
        }


        [Test]
        public void AddMediumRedShirtToCartAndCompleteCheckout()
        {
            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/customer/account/login/");

            FindElementWithWait(driver, By.Id("email")).SendKeys("existingemail@example.com");
            FindElementWithWait(driver, By.Id("pass")).SendKeys("Pass12word3!");
            FindElementWithWait(driver, By.CssSelector("#send2")).Click();

            Assert.IsTrue(FindElementWithWait(driver, By.XPath("//span[text()='My Account']")).Displayed);

            // Navigate to the Men > Tops > Tees category
            FindElementWithWait(driver, By.XPath("//a[@id='ui-id-5']")).Click();
            FindElementWithWait(driver, By.XPath("//a[text()='Tees']")).Click();

            //Select tshirt
            FindElementWithWait(driver,By.XPath("//a[contains(text(),'Logan Heat')]")).Click();

            //apply red and medium size and Add to cart
            FindElementWithWait(driver,By.XPath("//*[@option-id='168']")).Click();
            Thread.Sleep(2000);
            FindElementWithWait(driver, By.XPath("//*[@option-id='58']")).Click();
            Thread.Sleep(2000);
            FindElementWithWait(driver,By.CssSelector(".product-info-main .action.tocart")).Click();

            Thread.Sleep(5000);
            // Proceed to checkout
            FindElementWithWait(driver,By.XPath("//a[@class='action showcart']")).Click();
            FindElementWithWait(driver,By.CssSelector(".minicart-wrapper .action.primary.checkout")).Click();

            Thread.Sleep(3000);

        /*
            // Fill in the checkout information
            FindElementWithWait(driver,By.Name("firstname")).SendKeys("Existing");
            FindElementWithWait(driver,By.Name("lastname")).SendKeys("Email");
            FindElementWithWait(driver,By.Name("street[0]")).SendKeys("123 Main St");
            FindElementWithWait(driver,By.Name("city")).SendKeys("New York");

            IWebElement State = driver.FindElement(By.Name("region_id"));
            SelectElement selectElement = new SelectElement(State);
            selectElement.SelectByText("New York");

            FindElementWithWait(driver,By.Name("telephone")).SendKeys("1234567890");
            FindElementWithWait(driver,By.Name("postcode")).SendKeys("23901");
        */

            FindElementWithWait(driver,By.CssSelector("input[value='flatrate_flatrate']")).Click();
            FindElementWithWait(driver,By.CssSelector(".button.action.continue.primary")).Click();
            Thread.Sleep(5000);
            FindElementWithWait(driver,By.CssSelector("button[title='Place Order']")).Click();

            // Verify that the order is placed successfully
            Assert.IsTrue(FindElementWithWait(driver,By.CssSelector(".checkout-success")).Displayed);
        }



        private static IWebElement FindElementWithWait(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            return wait.Until(ExpectedConditions.ElementExists(locator));
        }
    }
}
