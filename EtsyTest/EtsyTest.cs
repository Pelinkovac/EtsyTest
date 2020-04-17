using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EtsyTest
{    
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class Etsy<TWebDriver> where TWebDriver : IWebDriver, new()
    {
        IWebDriver _driver;
        readonly string _url = "https://www.etsy.com/";
        public string RandomString(int len, string tip)
        {
            StringBuilder _pool = new StringBuilder("._-abcdefghijklmnopqrstuvwxyz1234567890");
            StringBuilder _str = new StringBuilder();
            Random rng = new Random();
            if (tip != "email")
                _pool.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&'*+/=?^`{|}~");
            while (_str.Length < len)
            {   
                int index = Convert.ToInt32(rng.NextDouble() * _pool.Length);
                if (_str.Length == 0 && index == 0)
                    _str.Append("a");
                else
                    _str.Append(_pool.ToString()[index]);
            }
            if(tip == "email")
                _str.Append("@mail.com");
            return _str.ToString();
        }
        [SetUp]
        public void Initialize()
        {
            _driver = new TWebDriver();
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(_url);
            Assert.AreEqual(_url, _driver.Url);
            Thread.Sleep(1000);
        }
        //----------------2. Use-case: Sign-in------------------//
        [Test]
        public void TestSignIn()
        {
            IWebElement _signInNav = _driver.FindElement(By.XPath("//button[@class='wt-btn wt-btn--small wt-btn--transparent wt-mr-xs-1 inline-overlay-trigger signin-header-action select-signin']"));
            if(_signInNav.Displayed)
                _signInNav.Click();
            Thread.Sleep(1000);

            IWebElement _email = _driver.FindElement(By.CssSelector("#join_neu_email_field"));
            if(_email.Displayed)
            { 
                _email.Click();
                _email.SendKeys("timvikinzi@gmail.com");
            }
            Thread.Sleep(1000);

            IWebElement _password = _driver.FindElement(By.XPath("//input[@id='join_neu_password_field']"));
            if(_password.Displayed)
            {
                _password.Click();
                _password.SendKeys("perideriprogrameri");
            }
            Thread.Sleep(1000);

            IWebElement _signInBtn = _driver.FindElement(By.Name("submit_attempt"));
            if(_signInBtn.Displayed)
                _signInBtn.Click();
            Thread.Sleep(1000);

            IWebElement _account = _driver.FindElement(By.XPath("//li[@class='user-nav has-sub-nav']//a[@class='nav-link']"));
            if (_account.Displayed) 
            { 
                _account.Click();
                Assert.Pass();
            }
        }
        //----------------1. Use-case: Register account------------------//
        [Test]
        public void TestRegister()
        {
            IWebElement _signInNav = _driver.FindElement(By.XPath("//button[@class='wt-btn wt-btn--small wt-btn--transparent wt-mr-xs-1 inline-overlay-trigger signin-header-action select-signin']"));
            if(_signInNav.Displayed)
                _signInNav.Click();
            Thread.Sleep(1000);

            IWebElement _registerNav = _driver.FindElement(By.XPath("//button[@class='wt-btn wt-btn--outline wt-btn--small inline-overlay-trigger register-header-action select-register']"));
            if(_registerNav.Displayed)
                _registerNav.Click();
            Thread.Sleep(1000);

            IWebElement _email = _driver.FindElement(By.CssSelector("#join_neu_email_field"));
            if (_email.Displayed) 
            { 
                _email.Click();
                _email.SendKeys(RandomString(10, "email"));
            }

            IWebElement _name = _driver.FindElement(By.Id("join_neu_first_name_field"));
            if (_name.Displayed)
            { 
                _name.Click();
                _name.SendKeys(RandomString(7, "name"));
            }
            Thread.Sleep(1000);

            IWebElement _password = _driver.FindElement(By.Id("join_neu_password_field"));
            if (_password.Displayed)
            {
                _password.Click();
                _password.SendKeys(RandomString(10, "password"));
            }
            Thread.Sleep(1000);

            IWebElement _registerBtn = _driver.FindElement(By.XPath("//button[@name='submit_attempt']"));
            if(_registerBtn.Displayed)
                _registerBtn.Click();
            Thread.Sleep(1000);

            IWebElement _account = _driver.FindElement(By.XPath("//li[@class='user-nav has-sub-nav']//a[@class='nav-link']"));
            if (_account.Displayed)
            {
                _account.Click();
                Assert.Pass();
            }
        }
        //----------------3. Use-case: Browsing categories------------------//
        [Test]
        public void TestBrowse()
        {
            Random rand = new Random();

            //Select nav bar
            IWebElement _navBar = _driver.FindElement(By.XPath("/html[1]/body[1]/div[5]/div[1]/div[1]/ul[1]"));

            //Select category
            IList<IWebElement> _li = _navBar.FindElements(By.TagName("li"));
            int i = _li.Count;
            i = Convert.ToInt32(rand.NextDouble() * i);
            IWebElement _category = _li[i];

            //Hover over category
            Actions _hover = new Actions(_driver);
            _hover.MoveToElement(_category).Perform();
            Thread.Sleep(2000);

            //Select subcategory container
            IWebElement _div = _driver.FindElement(By.XPath("/html[1]/body[1]/div[5]/div[2]/div[1]/div[1]"));
            if (!_div.Displayed)
                //Doesn't have subcategories
                _category.Click();
            else
            {
                //Select subcategory container corresponding to category
                IList<IWebElement> _divs = _div.FindElements(By.XPath("*"));
                _div = _divs[i];

                //Check if subcategory has aside element
                IWebElement _aside = _driver.FindElement(By.TagName("aside"));
                if (_aside != null)
                {
                    //Select random aside element
                    IList<IWebElement> _asides = _aside.FindElements(By.TagName("li"));
                    int j = _asides.Count;
                    j = Convert.ToInt32(rand.NextDouble() * j);
                    IWebElement _asideLi = _asides[j];
                    _hover.MoveToElement(_asideLi).Perform();
                    Thread.Sleep(2000);
                    //Select corresponding subcategory container matching to aside element
                    _div = _div.FindElement(By.XPath($"//div[{i+1}]/div[1]/div[1]/div[1]/section[{j+1}]"));
                }
                //Select a random subcategory
                IList<IWebElement> _a = _div.FindElements(By.XPath("/descendant::a"));
                i = _a.Count;
                i = Convert.ToInt32(rand.NextDouble() * i);

                IWebElement _subcategory = _a[i];
                _subcategory.Click();
                Thread.Sleep(4000);
            }
        }
        //----------------9. Use-case: Follow------------------//
        [Test]
        public void TestFollow()
        {

        }
        //----------------10. Use-case: Sign-out------------------//
        [Test]
        public void TestSignOut()
        {
            TestSignIn();
            Thread.Sleep(2000);

            IWebElement _account = _driver.FindElement(By.XPath("//a[@class='nav-link activated']"));
            if(_account.Displayed)
                _account.Click();
            Thread.Sleep(1000);

            IWebElement _signOutBtn = _driver.FindElement(By.Id("sub-nav-user-menu-326835-4"));
            if(_signOutBtn.Displayed)
            {
                _signOutBtn.Click();
            }
            Thread.Sleep(1000);

            if (!_signOutBtn.Displayed)
                Assert.Pass();
            else
                Assert.Fail();
        }
        [TearDown]
        public void EndTest()
        {
            _driver.Close();
            _driver.Dispose();
        }
    }
}