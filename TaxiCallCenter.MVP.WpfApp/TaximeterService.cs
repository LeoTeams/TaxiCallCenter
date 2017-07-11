using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using TaxiCallCenter.MVP.WpfApp.Models;
using TaxiCallCenter.MVP.WpfApp.Validation;

namespace TaxiCallCenter.MVP.WpfApp
{
    public sealed class TaximeterService : IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly ChromeOptions options;
        private readonly IWebDriver driver;
        private readonly Random random = new Random();

        private String dispatcherHomeWindowHandle;

        public TaximeterService()
        {
            this.options = new ChromeOptions();
            this.driver = new ChromeDriver(this.options);
        }

        public async Task InitializeAsync()
        {
            await this.semaphore.WaitAsync();
            try
            {
                this.driver.Navigate().GoToUrl("https://lk.taximeter.yandex.ru");

                if (this.driver.Url.StartsWith("https://lk.taximeter.yandex.ru/login"))
                {
                    await this.RandomDelayAsync(1000, 1500);

                    var yandexLogin = this.driver.FindElement(By.LinkText("Войти через Яндекс"));
                    yandexLogin.Click();

                    await this.RandomDelayAsync(1000, 1500);

                    var usernameField = this.driver.FindElement(By.Name("login"));
                    usernameField.SendKeys("barnaul-dispatcher@yandex.ru");

                    await this.RandomDelayAsync(1000, 1500);

                    var passwordField = this.driver.FindElement(By.Name("passwd"));
                    passwordField.SendKeys("b6MROHvjlD2nxpqtOo76");

                    await this.RandomDelayAsync(1000, 1500);

                    usernameField.Submit();
                }

                await this.RandomDelayAsync(1000, 1500);

                Ensure.State.MeetCondition(this.driver.Url.StartsWith("https://lk.taximeter.yandex.ru"));

                this.driver.FindElement(By.LinkText("Orders")).Click();

                await this.RandomDelayAsync(1000, 1500);

                Ensure.State.MeetCondition(this.driver.Url.StartsWith("https://lk.taximeter.yandex.ru/dispatcher"));

                this.dispatcherHomeWindowHandle = this.driver.CurrentWindowHandle;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async Task<String> MakeOrderAsync(OrderInfo order)
        {
            await this.semaphore.WaitAsync();
            try
            {
                if (this.dispatcherHomeWindowHandle != this.driver.CurrentWindowHandle)
                {
                    this.driver.SwitchTo().Window(this.dispatcherHomeWindowHandle);
                }

                this.driver.FindElement(By.Id("btn-new")).Click();
                await this.RandomDelayAsync(2000, 3000);

                var lastWindowHandle = this.driver.WindowHandles.Last();
                this.driver.SwitchTo().Window(lastWindowHandle);

                Ensure.State.MeetCondition(this.driver.Url.StartsWith("https://lk.taximeter.yandex.ru/dispatcher/new"));

                var fieldSms = this.driver.FindElement(By.Id("sms"));
                if (fieldSms.Selected) fieldSms.Click();
                await this.RandomDelayAsync(100, 200);
                var fieldShowPhone = this.driver.FindElement(By.Id("show-phone"));
                if (fieldShowPhone.Selected) fieldShowPhone.Click();
                await this.RandomDelayAsync(100, 200);

                this.driver.FindElement(By.Id("phone1")).SendKeys(order.Phone);
                await this.RandomDelayAsync(100, 200);

                //this.driver.FindElement(By.Id("time")).SendKeys(order.TrueDateTime.ToString("HH:mm"));
                this.SetElementValue("time", order.TrueDateTime.ToString("HH:mm"));
                await this.RandomDelayAsync(100, 200);
                this.driver.FindElement(By.Id("date")).SendKeys(order.TrueDateTime.ToString("MM/dd/yyyy"));
                await this.RandomDelayAsync(100, 200);

                var wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(10));
                wait.Until(x => x.FindElement(By.Id("contanier-count-order")).Displayed);

                //this.SetElementValue("AddressFromStreet", order.AddressFromStreet);
                this.driver.FindElement(By.Id("AddressFromStreet")).SendKeys(order.AddressFromStreet);
                await this.RandomDelayAsync(100, 200);
                //this.SetElementValue("AddressFromHouse", order.AddressFromHouse);
                this.driver.FindElement(By.Id("AddressFromHouse")).SendKeys(order.AddressFromHouse);
                await this.RandomDelayAsync(100, 200);

                //this.SetElementValue("Address_0__Street", order.AddressToStreet);
                this.driver.FindElement(By.Id("Address_0__Street")).SendKeys(order.AddressToStreet);
                await this.RandomDelayAsync(100, 200);
                //this.SetElementValue("Address_0__House", order.AddressToHouse);
                this.driver.FindElement(By.Id("Address_0__House")).SendKeys(order.AddressToHouse);
                await this.RandomDelayAsync(100, 200);

                this.driver.FindElement(By.Id("description")).SendKeys(order.AdditionalInfo);
                await this.RandomDelayAsync(100, 200);

                this.driver.FindElement(By.Id("rule")).SendKeys("Обычный");
                await this.RandomDelayAsync(100, 200);

                this.driver.FindElement(By.Id("tariff")).SendKeys("Эконом");
                await this.RandomDelayAsync(250, 500);

                wait.Until(x => !String.IsNullOrEmpty(x.FindElement(By.Id("calc-cost")).GetAttribute("value")));
                var price = this.driver.FindElement(By.Id("calc-cost")).GetAttribute("value");
                return price;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public void Dispose()
        {
            this.driver?.Dispose();
        }

        private void SetElementValue(String id, String value)
        {
            this.driver.ExecuteJavaScript("document.getElementById(arguments[0]).value = arguments[1];", id, value);
        }

        private Task RandomDelayAsync(Int32 from, Int32 to)
        {
            return Task.Delay(this.random.Next(from, to));
        }
    }
}
