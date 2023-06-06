using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Text;
using System.Collections.ObjectModel;

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
            var listOfLinks = new List<string>();
            string baseDirectory = "C:\\Lots\\";

            driver.Navigate().GoToUrl("https://torgi.gov.ru/new/public/lots/reg");

            //Прокрутка всей сраницы. P.S. Может занять очень много времени
            //IList<IWebElement> showMoreButtons = driver.FindElements(By.XPath("//*[contains(text(), 'Показать еще')]"));

            //// Нажимаем кнопки, пока они не исчезнут
            //while (showMoreButtons.Count > 0)
            //{
            //    foreach (IWebElement button in showMoreButtons)
            //    {
            //        try
            //        {
            //            button.Click();
            //        }
            //        catch (Exception)
            //        {
            //            // Игнорируем исключения, если кнопка больше не найдена
            //        }
            //    }
            //    showMoreButtons = driver.FindElements(By.XPath("//*[contains(text(), 'Показать еще')]"));
            //}

            // Находим высоту страницы
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            long scrollHeight = (long)js.ExecuteScript("return Math.max( document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight );");

            // Прокручиваем страницу до конца
            for (int i = 0; i < scrollHeight; i += 100)
            {
                js.ExecuteScript($"window.scrollTo(0, {i});");
            }

            //Скачиваем все ссыылки на лоты с страницы
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(80));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("a[class='lotLink'][target='_blank']")));
            ReadOnlyCollection<IWebElement> links = driver.FindElements(By.CssSelector("a[class='lotLink'][target='_blank']"));

            foreach (IWebElement link in links)
            {
                listOfLinks.Add(link.GetAttribute("href"));
            }

            //Проходимся по каждой ссылке
            for (var i = 0; i < links.Count; i++)
            {
                // переход на страницу
                driver.Navigate().GoToUrl(listOfLinks[i]);

                var ww = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                ww.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='lots-item']")));

                var lotId = listOfLinks[i].Split('/')[7];
                var lotTitle = driver.FindElement(By.CssSelector(".header_title")).Text;
                var lotsInfoPrices = driver.FindElements(By.CssSelector("div[class='prices__row__price-cell lotPrice']")).ToList();
                var category = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Категория объекта']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();

                if (listOfLotCategories.Contains(category))
                {
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
                    var lotType = ((driver.FindElement(By.XPath("//*[contains(text(), ' Вид торгов ')]"))).FindElement(By.XPath(".."))).FindElement(By.CssSelector("div[class='lotAttributeValue']")).Text.Trim();
                    //Предмет торгов(наименование лота)
                    var lotSub = ((driver.FindElement(By.XPath("//*[contains(text(), ' Предмет торгов (наименование лота) ')]"))).FindElement(By.XPath(".."))).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                    //Описание лота
                    var lotDescription = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Описание лота']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                    //Субъект местонахождения имущества
                    var locationSubject = ((driver.FindElement(By.XPath("//*[contains(text(), ' Субъект местонахождения имущества ')]"))).FindElement(By.XPath(".."))).FindElement(By.CssSelector("div[class='lotAttributeValue']")).Text.Trim();
                    //Местонахождение имущества
                    var location = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Местонахождение имущества']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                    //Категория объекта
                    var objectCategory = driver.FindElement(By.CssSelector("app-entity-attribute-simple[attributename='Категория объекта']")).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                    //Форма собственности
                    var ownershipForm = ((driver.FindElement(By.XPath("//*[contains(text(), ' Форма собственности ')]"))).FindElement(By.XPath(".."))).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
                    //Цель предоставления земельного участка
                    //Нет у Земли сельскохозяйственного назначения, Земли населенных пунктов, Земли специального назначения, Земли лесного фонда, Земли водного фонда, Земельные участки (категория не установлена)
                    //Есть у Земли особо охраняемых территорий и объектов, Земельные участки (не образованы)
                    var landPurpose = "Нет данных";

                    if (listOfCategoriesThatHavePurpose.Contains(category))
                    {
                        landPurpose = ((driver.FindElement(By.XPath("//*[contains(text(), ' Цель предоставления земельного участка ')]"))).FindElement(By.XPath(".."))).FindElement(By.CssSelector("div[class='attr_value']")).Text.Trim();
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

            // закрытие браузера
            driver.Quit();
            
        }
    }
}

