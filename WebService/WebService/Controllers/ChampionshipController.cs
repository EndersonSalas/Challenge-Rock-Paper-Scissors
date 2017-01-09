using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebService.ErrorException;
using WebService.Models;

namespace WebService.Controllers
{
    public class ChampionshipController : ApiController
    {
        DBRockPaperScissorsEntities dataBase = new DBRockPaperScissorsEntities();
        
        /// <summary>
        /// Receives the championship data and computes it to identify the winner. 
        /// The first and second place are stored into a database with their respective scores.
        /// </summary>
        /// <param name="data">The data of the championship</param>
        /// <returns>Returns the winner of the championship</returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.ActionName("new")]
        public PlayerPlaying newGame(List<PlayerPlaying> data)
        {
            try
            {
            List<PlayerPlaying> resultTournament = champions(data);
            result(new Result (resultTournament[0].name, resultTournament[1].name));
            return resultTournament[0];
            }catch {
                throw;
            }
           
        }

        /// <summary>
        /// Stores the first and second place of a tournament, each user is stored with their respective scores. 
        /// The user names will be unique, but the same user can win 1 or more championships. 
        /// </summary>
        /// <param name="first">The name of the winner of the championship.</param>
        /// <param name="second">The name of the second place of the championship.</param>
        /// <returns>Returns the operation status if successfully.</rturns>
        [System.Web.Http.AcceptVerbs("POST")]
        public JObject result(Result resultChampionship)
        {

            Players playerFirst = dataBase.Players.Where(item => item.Name == resultChampionship.first).FirstOrDefault();
            Players playerSecond = dataBase.Players.Where(item => item.Name == resultChampionship.second).FirstOrDefault();

            if (playerFirst != null)
            {
                playerFirst.Points += 3;
            }
            else
            {
                playerFirst = new Players();
                playerFirst.Name = resultChampionship.first;
                playerFirst.Points = 3;
                dataBase.Players.Add(playerFirst);
            }
            if (playerSecond != null)
            {
                playerSecond.Points += 1;
            }
            else
            {
                playerSecond = new Players();
                playerSecond.Name = resultChampionship.second;
                playerSecond.Points = 1;
                dataBase.Players.Add(playerSecond);
            }

            dataBase.SaveChanges();
            JObject json = JObject.Parse("{status: 'success'}");
            return json;

        }

       
        /// <summary>
        /// Retrieves the names of the top players of all championships. 
        /// </summary>
        /// <param name="count">Sets the count of records to be retrieved. If not provided, the default value is 10.</param>
        /// <returns>Returns the list of player names based on the count provided.</returns>
         [System.Web.Http.AcceptVerbs("GET")]
        public List<String> top(int count = 10)
        {
            List<String> listPlayers = dataBase.Players
                .Take(count)
                .OrderByDescending(player => player.Points)
                .Select(player => player.Name.Trim()).ToList();
            return listPlayers;
        }

         /// <summary>
         /// Retrieves the names and points of the top players of all championships.
         /// This is used for provide data to the web page client
         /// </summary>
         /// <param name="count">Sets the count of records to be retrieved. If not provided, the default value is 10.</param>
         /// <returns>Returns the list of player names and points based on the count provided</returns>
         [System.Web.Http.AcceptVerbs("GET")]
         [ApiExplorerSettings(IgnoreApi = true)]
         public List<Players> topPlayers(int count = 10)
         {
             List<Players> listPlayers = dataBase.Players
                 .Take(count)
                 .OrderByDescending(player => player.Points)
                 .ToList();
             return listPlayers;
         }

        private List<PlayerPlaying> champions(List<PlayerPlaying> listPlayers) {
            int aux = 0;
            int sizePlayer= listPlayers.Count;
            if ((listPlayers.Count % 2) == 0)
            {
                for (int i = 0; i < sizePlayer - 1; i++)
                {
                    List<PlayerPlaying> twoPlayers = new List<PlayerPlaying>();
                    PlayerPlaying playerWinner = new PlayerPlaying();
                    if (listPlayers.Count == 2)
                    {
                        playerWinner = winner(listPlayers);
                        if (playerWinner == listPlayers[0])
                        {
                            listPlayers[0] = playerWinner;
                            listPlayers[1] = listPlayers[1];
                        }
                        else
                        {

                            PlayerPlaying playerSecond = new PlayerPlaying();
                            playerSecond = listPlayers[0];
                            listPlayers[0] = playerWinner;
                            listPlayers[1] = playerSecond;
                        }
                    }
                    else
                    {
                        twoPlayers.Add(listPlayers[aux]);
                        twoPlayers.Add(listPlayers[aux + 1]);
                        playerWinner = winner(twoPlayers);
                        listPlayers.RemoveAt(aux);
                        listPlayers.RemoveAt(aux);
                        listPlayers.Insert(aux, playerWinner);
                        aux++;
                        if (aux == listPlayers.Count) {
                            aux = 0;
                        }
                    }

                }
            }
            else {
                throw new PlayException("Not enough players");
            }
            return listPlayers;
        }

        private PlayerPlaying winner(List<PlayerPlaying> listPlayers)
        {

            if (listPlayers.Count == 2)
            {

                int choose = chooseWinner(listPlayers[0].strategy.ToUpper(), listPlayers[1].strategy.ToUpper());
                if (!listPlayers[0].strategy.ToUpper().Equals("S") && !listPlayers[0].strategy.ToUpper().Equals("P") && !listPlayers[0].strategy.ToUpper().Equals("R"))
                {
                    throw new PlayException("One strategy of some player it´s wrong");
                }
                if (!listPlayers[1].strategy.ToUpper().Equals("S") && !listPlayers[1].strategy.ToUpper().Equals("P") && !listPlayers[1].strategy.ToUpper().Equals("R"))
                {
                    throw new PlayException("One strategy of some player it´s wrong");
                }

                return listPlayers[choose];


            }
            else
            {
                throw new PlayException("Missing players");
            }
        }

        private int chooseWinner(string oneStrategy, string twoStrategy)
        {
            switch (oneStrategy)
            {
                case "P":
                    if (twoStrategy.Equals("P"))
                    {
                        return 0;
                    }
                    else
                    {
                        if (twoStrategy.Equals("S"))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case "S":
                    if (twoStrategy.Equals("S"))
                    {
                        return 0;
                    }
                    else
                    {
                        if (twoStrategy.Equals("R"))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case "R":
                    if (twoStrategy.Equals("R"))
                    {
                        return 0;
                    }
                    else
                    {
                        if (twoStrategy.Equals("P"))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
            }
            return 2;
        }

    }
}
