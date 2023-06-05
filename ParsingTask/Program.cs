using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Text;

using System;
using System.IO;
using System.Net;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Runtime;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using HtmlAgilityPack;

namespace ConsoleApp
{
    class Program
    {

        public static List<string> listOfLotCategories = new List<string> 
        { 
            "Земли сельскохозяйственного назначения",
            "Земли населенных пунктов",
            "Земли специального назначения",
            "Земли особо охраняемых территорий и объектов",
            "Земли лесного фонда",
            "Земли водного фонда",
            "Земельные участки (категория не установлена)",
            "Земельные участки (не образованы)",
        };


        public static List<string> listOfCategoriesThatHavePurpose = new List<string>
        {
            "Земли особо охраняемых территорий и объектов",
            "Земельные участки (не образованы)",
        };

        public static List<string> listOfCategoriesThatDontHavePurpose = new List<string>
        {
            "Земли сельскохозяйственного назначения",
            "Земли населенных пунктов",
            "Земли специального назначения",
            "Земли лесного фонда",
            "Земли водного фонда",
            "Земельные участки (категория не установлена)",
        };

        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            WebClient client = new WebClient();
            string baseDirectory = "C:\\Lots\\";

            driver.Navigate().GoToUrl("https://torgi.gov.ru/new/public/lots/reg");

            // newWait = new WebDriverWait(driver, TimeSpan.FromSeconds(80));

            //// Дожидаемся появления элемента <app-root>
            //newWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("app-button[label='Показать еще']")));

            //while (driver.FindElements(By.CssSelector("app-button[label='Показать еще']")).Count != 0);
            //{//IWebElement loadMoreButton = driver.FindElement(By.CssSelector("button[class='button outline']"));

                //driver.FindElement(By.CssSelector("app-button[label='Показать еще']")).Click();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(80));

                // Дожидаемся появления элемента <app-root>
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("a[class='lotLink'][target='_blank']")));

                ReadOnlyCollection<IWebElement> links = driver.FindElements(By.CssSelector("a[class='lotLink'][target='_blank']"));

                var linkAttr = new List<string>();
                foreach (IWebElement link in links)
                {
                    linkAttr.Add(link.GetAttribute("href"));
                }


                for (var k = 0; k < links.Count; k++)
                {

                    string lotId = linkAttr[k].Split('/')[7];

                    System.Threading.Thread.Sleep(150);

                    // переход на страницу
                    driver.Navigate().GoToUrl(linkAttr[k]);

                    var ww = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                    ww.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='lots-item']")));

                    // Скачиваем HTML код страницы


                    var lotTitle = driver.FindElement(By.CssSelector(".header_title")).Text;
                    //var startingPrice = driver.FindElement(By.CssSelector("span[data-ng-bind-html='lot.startPrice | number']"));
                    var lotsInfo = driver.FindElements(By.CssSelector("div[class='lotAttributeValue']")).ToList();
                    var lotsInfoSecond = driver.FindElements(By.CssSelector("div[class='attr_value']")).ToList();
                    var lotss = driver.FindElements(By.CssSelector("div[class='attr_value']")).ToList();
                    var lotsInfoPrices = driver.FindElements(By.CssSelector("div[class='prices__row__price-cell lotPrice']")).ToList();
                    var category = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Категория объекта']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();

                    foreach (var lotInfo in lotsInfo)
                    {
                        string fghm = lotInfo.Text;
                    }

                    foreach (var lotInfo in lotsInfoSecond)
                    {
                        string fghm = lotInfo.Text;
                    }
                    foreach (var lotInfo in lotsInfoPrices)
                    {
                        string fghm = lotInfo.Text;
                    }

                    if (listOfLotCategories.Contains(category))
                    {
                        //Номер
                        var lotNumber = lotsInfo[1].Text;

                        var startingPrice = "Нет данных";
                        var auctionStep = "Нет данных";

                        if (lotsInfoPrices.Count > 0)
                        {
                            //Начальная цена
                            startingPrice = lotsInfoPrices[0].Text;
                            //Шаг аукциона
                            auctionStep = lotsInfoPrices[1].Text;
                        }
                        //Вид торгов
                        var lotType = lotsInfo[2].Text;
                        //Предмет торгов(наименование лота)
                        var lotSub = lotsInfoSecond[0].Text;
                        //var lotInfoddd = driver.FindElements(By.CssSelector("div[class='attr']")).ToList(); //.FindElement(By.XPath("//*[contains(text(),' Предмет торгов (наименование лота) ')]"))..FindElement(By.CssSelector("div[class='attr_value']")).Text;

                        //Описание лота
                        var lotDescription = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Описание лота']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                        //Субъект местонахождения имущества
                        var locationSubject = lotsInfo[4].Text;
                        //Местонахождение имущества
                        var location = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Местонахождение имущества']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                        //Категория объекта
                        var objectCategory = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Категория объекта']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                        //Форма собственности
                        var ownershipForm = lotss[7].Text;
                        //Цель предоставления земельного участка
                        //Нет у Земли сельскохозяйственного назначения, Земли населенных пунктов, Земли специального назначения, Земли лесного фонда, Земли водного фонда, Земельные участки (категория не установлена)
                        //Есть у Земли особо охраняемых территорий и объектов, Земельные участки (не образованы)
                        var landPurpose = "Нет данных";

                        if (listOfCategoriesThatHavePurpose.Contains(category))
                        {
                            landPurpose = lotss[8].Text;
                        }


                        string lotDirectory = baseDirectory + lotId;

                        if (!Directory.Exists(lotDirectory))
                        {
                            Directory.CreateDirectory(lotDirectory);
                        }


                        // Создаем файл info.txt и записываем в него характеристики земельного участка
                        var infoFilePath = Path.Combine(lotDirectory, "info.txt");
                        using (var infoFile = new StreamWriter(infoFilePath, false, Encoding.UTF8))
                        {
                            infoFile.WriteLine($"Наименование: {lotTitle}");
                            infoFile.WriteLine($"Начальная цена: {startingPrice}");
                            infoFile.WriteLine($"Шаг аукциона: {auctionStep}");
                            infoFile.WriteLine($"Вид торгов: {lotType}");
                            infoFile.WriteLine($"Предмет торгов (наименование лота): {lotSub}");
                            infoFile.WriteLine($"Описание лота: {lotDescription}");
                            infoFile.WriteLine($"Субъект местонахождения имущества: {locationSubject}");
                            infoFile.WriteLine($"Местонахождение имущества: {location}");
                            infoFile.WriteLine($"Категория объекта: {objectCategory}");
                            infoFile.WriteLine($"Форма собственности: {ownershipForm}");

                            infoFile.WriteLine($"Цель предоставления земельного участка: {landPurpose}");
                        }

                        // Скачиваем изображение, если оно есть
                        try
                        {
                            var imgUrl = driver.FindElement(By.CssSelector("img[class='selected-image-list-item']")).GetAttribute("src");
                            var imgFileName = "image.png";
                            var imgFilePath = Path.Combine(lotDirectory, imgFileName);

                            client.DownloadFile(imgUrl, imgFilePath);
                        }
                        catch (NoSuchElementException)
                        {
                            // Нет изображения
                        }
                    }

                }
            //}

            // закрытие браузера
            driver.Quit();
            
        }
    }
}

