using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Formatting;
using System.Web.Script.Serialization;
using Rock_Paper_Scissors.Models;
using Rock_Paper_Scissors.ErrorException;
using Newtonsoft.Json;
using System.Web.Configuration;

namespace Rock_Paper_Scissors.Controllers
{
    public class PlayController : Controller
    {
        private string webService = WebConfigurationManager.AppSettings.Get("WebService");
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ResultWinner(HttpPostedFileBase file)
        {
            Winner winner = new Winner();
            try
            {
                if (Path.GetExtension(file.FileName).Equals(".txt"))
                {
                    int counter = 0;
                    string line;
                    string dataFile = "";
                    string path = Server.MapPath("~/Records/" + file.FileName);

                    file.SaveAs(path);
                    StreamReader fileRead = new StreamReader(path);
                    while ((line = fileRead.ReadLine()) != null)
                    {
                        dataFile += line;
                        counter++;
                    }
                    fileRead.Close();

                    winner = responseWebServer(dataFile);

                }
                else
                {
                    throw new ApplicationException();
                }
            }
            catch (NullReferenceException) {
                winner.name = "No file uploaded";
                winner.strategy = "";
            }
            catch (ApplicationException)
            {
                winner.name = "The extension of the file must be .txt";
                winner.strategy = "";
            }
            catch (PlayException ex)
            {
                winner.name = ex.Message;
                winner.strategy = "";
            }
            return View(winner);
        }

        private Winner responseWebServer(string dataFile)
        {
            Winner winner = new Winner();
            HttpClient client = new HttpClient();
            List<string> extractData = dataFile.Split(new char[] { '[', ',', ']', ' ', '"', '}', '{' }).ToList();
            List<string> dataPlayers = new List<string>();
            List<Winner> listPlayer = new List<Winner>();
            bool aux = false;
            
            for (int i = 0; i < extractData.Count; i++)
            {
                if (!extractData[i].Equals(""))
                {
                    dataPlayers.Add(extractData[i]);
                }
            }
            for (int j = 0; j < dataPlayers.Count; j += 2)
            {
                listPlayer.Add(new Winner { name = dataPlayers[j], strategy = dataPlayers[j + 1] });
            }
            
            for (int i = 0; i < listPlayer.Count; i++)
            {
                if (!listPlayer[i].strategy.ToUpper().Equals("S") && !listPlayer[i].strategy.ToUpper().Equals("P") && !listPlayer[i].strategy.ToUpper().Equals("R"))
                {
                    aux = true;
                    break;
                }
            }
            if (aux)
            {
                throw new PlayException("One strategy of some player it´s wrong");
            }
            if (!((listPlayer.Count % 2) == 0))
            {
                throw new PlayException("Not enough players");
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post,webService + "new");
            JavaScriptSerializer json_serializerSend = new JavaScriptSerializer();

            string data= ""+ json_serializerSend.Serialize(listPlayer)+"";
            requestMessage.Content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.SendAsync(requestMessage).Result;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (response.IsSuccessStatusCode)
            {
                string dataResponse = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var result = (IDictionary<string, object>)json_serializer.DeserializeObject(dataResponse);
                winner.name = result["name"].ToString();
                winner.strategy = result["strategy"].ToString();
            }
            else
            {
                throw new PlayException("Something wrong in web service");
            }
            return winner;
        }


        public ActionResult TopTen()
        {
            List<Players> result= new List<Players>();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync(webService+"topPlayers").Result;
            if (response.IsSuccessStatusCode)
            {
                string dataResponse = response.Content.ReadAsStringAsync().Result;
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var data = JsonConvert.DeserializeObject<List<Players>>(dataResponse.Trim());
                result = data;
            }
            return View(result);
        }

        public ActionResult ResetTable()
        {
            DBRockPaperScissorsEntities dataBase = new DBRockPaperScissorsEntities();
            dataBase.Database.ExecuteSqlCommand("Truncate table Players ");
            return View("TopTen");
        }


    }
}