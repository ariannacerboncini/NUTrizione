using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Var.Utilities.CsvHelper;

namespace Nutrizione;
public class Program
{
    public static void Main(string[] args)
    {
        string url = "https://www.alimentinutrizione.it/tabelle-nutrizionali/ricerca-per-ordine-alfabetico";
        var awaiter = CallURL(url);
        if (awaiter.Result != "")
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(awaiter.Result);
            var foodTable = doc.DocumentNode.SelectNodes("//table[@id = 'cercatabella']//li");
            List<FoodInfo> foodList = new List<FoodInfo>();
            foreach (var item in foodTable)
            {
                FoodInfo foodItem = new FoodInfo();               
                foodItem.Name = item.ChildNodes[1].InnerText.Trim();
                foodItem.Url = item.ChildNodes[1].ChildNodes[1].Attributes[0].Value;
                foodList.Add(foodItem);
            }
            //VarCsvHelper.WriteCsv(foodList, "C:\\temp", "alimenti");
            
            foreach(var food in foodList) 
            { 
                awaiter = CallURL($"https://www.alimentinutrizione.it{food.Url}");
                doc.LoadHtml(awaiter.Result);

                #region General Table
                var infoTable = doc.DocumentNode.SelectNodes("//*[@id=\"conttableft\"]/div[1]/table");                
                foreach (var item in infoTable)
                {                    
                    for (int i = 3; i < item.ChildNodes.Count - 1; i++)
                    {
                        switch (item.ChildNodes[i].ChildNodes[0].InnerText)
                        {
                            case "Categoria":
                                food.Category = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "Codice Alimento":
                                food.FoodId = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "Nome Scientifico":                            
                                food.ScientificName = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "English Name":
                                food.EnglishName = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "Informazioni":
                                food.Informations = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "Numero Campioni":
                                food.NumberOfSamples = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "Parte Edibile":
                                food.EdiblePart = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;
                            case "Porzione":
                                food.Portion = item.ChildNodes[i].ChildNodes[1].InnerText;
                                break;                            
                        }
                    }
                }
                //VarCsvHelper.AppendToCsv(foodTableList, "C:\\temp", "alimentiTabella");
                #endregion General Table

                #region Nutrients
                food.Nutrients = new List<Nutrient>();
                var nutrientsTable = doc.DocumentNode.SelectNodes("//*[@id=\"t3-content\"]/div[2]/article/section/table/tbody/tr");
                var currentCategory = string.Empty;
                foreach (var item in nutrientsTable)
                {
                    if (item.Attributes["class"].Value.Contains("title"))
                    {
                        currentCategory = item.InnerText.Trim();
                    }
                    if (item.Attributes["class"].Value.Contains("corpo"))
                    {                        
                        food.Nutrients.Add(new Nutrient { Category = currentCategory,
                            Description = item.ChildNodes[0].InnerText, 
                            ValuePer100g = item.ChildNodes[2].InnerText, 
                            DataSource = item.ChildNodes[6].InnerText,
                            Procedures = item.ChildNodes[7].InnerText,
                            References = item.ChildNodes[8].HasChildNodes ? item.ChildNodes[8].ChildNodes[0].Attributes["data-content"].Value : null
                    }) ;
                    }         
                }
                #endregion Nutrients

                #region Langual Codes
                food.LangualCodes = new List<LangualCode>();
                var langualCodeTable = doc.DocumentNode.SelectNodes("//*[@id=\"t3-content\"]/div[2]/article/section/div[2]/div[1]/div");
                foreach(var item in langualCodeTable)
                {
                    for (int i = 1; i < item.ChildNodes.Count; i++)
                    {
                        food.LangualCodes.Add(new LangualCode
                        {
                            Id = item.ChildNodes[i].InnerText.Replace('|', ' ').Trim(),
                            Info = item.ChildNodes[i].Attributes["data-content"].Value
                        });
                    }
                }                
                #endregion Langual Codes

                #region Chart
                string html = doc.DocumentNode.InnerHtml;
                int startingIndex = html.IndexOf("['Proteine', ");
                string chartData = html.Substring(startingIndex, 100);
                string pattern = @"\d+";
                var matches = Regex.Matches(chartData, pattern);
                food.Protein = matches[0].Value;
                food.Fat = matches[1].Value;
                food.Carbohydrate = matches[2].Value;
                food.Fiber = matches[3].Value;
                food.Alcohol = matches[4].Value;
                #endregion Chart
            }
        }

    }
    public static async Task<string> CallURL(string url)
    {
        HttpClient client = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        client.DefaultRequestHeaders.Accept.Clear();
        var response = client.GetStringAsync(url);
        return await response;
    }

}
