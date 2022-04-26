using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using System.Net.Http;
using WebSocketServer_Roulette.Models;

namespace WebSocketServer_Roulette
{
    public class Roulette : WebSocketBehavior
    {
        //Lista di client connessi
        private static List<WebSocket> _clientSockets = new List<WebSocket>();
        //numero di client connessi
        private static int _count;

        int _contaPartite =0;

        //method to deserialized json api
        //params unused but necessary
        static Spin SpinServer { get; set; } =
            JsonConvert.DeserializeObject<Spin>(
                new HttpClient().GetAsync(
                    new Uri("https://www.roulette.rip/api/play?bet=odd&wager=10")
                    ).Result.Content.ReadAsStringAsync().Result
                ); 

        protected override void OnOpen()
        {
            WebSocket clientN = Context.WebSocket;
            _count = _clientSockets.Count;
            Console.WriteLine("Richiesta connessione da client: " + (_count+1).ToString());
            // check if > 1
            if(_count > 1){
                Context.WebSocket.Close();
                Console.WriteLine("Connessione rifiutata");
            }else
                _clientSockets.Add(clientN);
        }
        //e.data works 
        protected override void OnMessage(MessageEventArgs e)
        {
            //e.Data -> string from client --> json
            if (Context.WebSocket == _clientSockets[0]) //first who asks
            {
                //respond to the client
                CheckBet(e.Data);
                _clientSockets[0].Send(JsonConvert.SerializeObject(SpinServer));  
            }
            else
            {
                //respond to the client
                CheckBet(e.Data);
                _clientSockets[1].Send(JsonConvert.SerializeObject(SpinServer));   //second
                
                _contaPartite++;
                if(_contaPartite >0)
                    SpinServer = JsonConvert.DeserializeObject<Spin>(
                        new HttpClient().GetAsync(
                            new Uri("https://www.roulette.rip/api/play?bet=odd&wager=10")
                            ).Result.Content.ReadAsStringAsync().Result
                        );
                    SpinServer.vincita = 0; 
                    SpinServer.totalMoneyBetsSpent = 0; 
            }
        }
        void CheckBet(string jsonPuntata)
        {
            int vincita = 0; //se va in negativo togliere i soldi al giocatore

            //controllo cosa ha scelto il giocatore

            // creo classe per deserializzare il json
            Puntata puntata = JsonConvert.DeserializeObject<Puntata>(jsonPuntata);
            
            if (puntata.numero.estrazione == SpinServer.roll.number && puntata.numero.estrazione !=null){
                if (puntata.numero.estrazione != 0)
                vincita = (int)(puntata.numero.scommessa * 35); //case 1:1 any -->  35:1
                else
                vincita = (int)(puntata.numero.scommessa * 36); //case 1:1 zero -->  36:1
            }
            

            //casi pagamento somma puntata 1:1
            if (puntata.colore.coloreScelto != null && puntata.colore.coloreScelto.Equals(SpinServer.roll.color))
                vincita = (int)(puntata.colore.scommessa * 2);
            
            //casi pagamento somma puntata 1:1

            //colore
            if (puntata.colore.coloreScelto !=null && puntata.colore.coloreScelto.Equals(SpinServer.roll.color) )
                vincita = (int)(puntata.colore.scommessa * 2);

            //parita
            if (puntata.parita.paritaScelta != null && puntata.parita.paritaScelta.Equals(SpinServer.roll.parity) )
                vincita = (int)(puntata.parita.scommessa * 2);

            //n1-18
            if ( puntata.n1_18.n1_18_Scelto !=null && (bool)puntata.n1_18.n1_18_Scelto)
                if(SpinServer.roll.number < 19)
                vincita = (int)(puntata.n1_18.scommessa * 2);

            //n1_19
            if ( puntata.n19_36.n19_36_Scelto !=null && (bool)puntata.n19_36.n19_36_Scelto )
                if (SpinServer.roll.number > 19)
                    vincita = (int)(puntata.n19_36.scommessa * 2);

            
            //soldi spesi per giocare
             SpinServer.totalMoneyBetsSpent =  (int)(puntata.numero.scommessa + puntata.colore.scommessa + puntata.parita.scommessa + puntata.n1_18.scommessa + puntata.n19_36.scommessa);
            //guadagno = ricavo - spesa -> vincita- bets
            SpinServer.vincita =  vincita; 
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string path = "ws://127.0.0.1:7890";
            //creo istanza
            WebSocketServer ws = new WebSocketServer(path);

            ws.AddWebSocketService<Roulette>("/Roulette");
            ws.Start();
            Console.WriteLine($"Server partito a {path}");
 
            Console.ReadKey();

            ws.Stop();
        }
    }
}
