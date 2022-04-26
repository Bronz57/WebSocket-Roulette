using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable
namespace WebSocketServer_Roulette.Models
{

    public class Spin
    {
        public Roll roll { get; set; }
        [JsonProperty("vincita")]
        public int vincita { get; set; }
        public int totalMoneyBetsSpent { get; set; }

    }
    //numero generato da api
    public class Roll
    {
        [JsonProperty("number")]
        public long number { get; set; }

        [JsonProperty("color")]
        public string color { get; set; }

        [JsonProperty("parity")]
        public string parity { get; set; }
    }
    public partial class Puntata
    {
        [JsonProperty("numero")]
        public Numero numero { get; set; }

        [JsonProperty("parita")]
        public Parita? parita { get; set; }

        [JsonProperty("colore")]
        public Colore? colore { get; set; }

        [JsonProperty("n_1_18")]
        public N1_18? n1_18 { get; set; }

        [JsonProperty("n_19_36")]
        public N19_36? n19_36 { get; set; }

    }
    public partial class Colore
    {
        [JsonProperty("coloreScelto")]
        public string? coloreScelto { get; set; }

        [JsonProperty("scommessa")]
        public int? scommessa { get; set; }
    }

    public partial class N19_36
    {
        [JsonProperty("n_19_36_scelto")]
        public bool? n19_36_Scelto { get; set; }

        [JsonProperty("scommessa")]
        public int? scommessa { get; set; }
    }

    public partial class N1_18
    {
        [JsonProperty("n_1_18_scelto")]
        public bool? n1_18_Scelto { get; set; }

        [JsonProperty("scommessa")]
        public int? scommessa { get; set; }
    }

    public partial class Numero
    {
        [JsonProperty("estrazione")]
        public int? estrazione { get; set; }

        [JsonProperty("scommessa")]
        public int? scommessa { get; set; }
    }

    public partial class Parita
    {
        [JsonProperty("paritaScelta")]
        public string? paritaScelta { get; set; }

        [JsonProperty("scommessa")]
        public int? scommessa { get; set; }
    }
}
