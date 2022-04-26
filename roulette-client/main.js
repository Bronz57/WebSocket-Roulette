function BuildJson () {
  return {
    "numero" :
    {
          "estrazione" : null,
          "scommessa"    : 0
    },
    "parita" :
    {
          "paritaScelta" : null, 
          "scommessa"    : 0
    },
    "colore" :
    {
          "coloreScelto" : null,
          "scommessa"    : 0
    },
    "n_1_18" :
    {
          "n1_18_Scelto" : false,
          "scommessa"    : 0
    },
    "n_19_36" :
    {
          "n_19_36_Scelto" : false,
          "scommessa"    : 0
    }
   };
} 

const roulette = BuildJson();
var roulette_dummy = roulette;
var canSpin = false;

function CheckIfCan(entrata){
  if(entrata > 0 && entrata>=GetChip())
    return true;
  return false;
}

//get the chip selected
function GetChip() {
  for (let radio of document.getElementsByName('fiches')) 
    if(radio.checked) return parseInt(radio.value)
}
//driver
function active(id){ //number or string
  let saldo = document.getElementById('entrata');
  if(!CheckIfCan(parseInt(saldo.value))){
    alert("you don't have sufficient balance")
    return -1;
  }
  let str_puntata = `You bet ${GetChip()} on `
  if(id == "n1_18_Scelto")
    str_puntata += "1 to 18"
  else if(id == "n19_36_Scelto")
    str_puntata += "19 to 36"
  else
    str_puntata += id;
  alert(str_puntata);

  saldo.value = parseInt(saldo.value)-GetChip();
  if(!isNaN(id)){
    roulette_dummy.numero.estrazione = id;
    roulette_dummy.numero.scommessa = GetChip();
  } else if(id == "odd" || id =='even'){
    roulette_dummy.parita.paritaScelta = id;
    roulette_dummy.parita.scommessa = GetChip();
  } else if(id=='red' || id=='black'){
    roulette_dummy.colore.coloreScelto = id;
    roulette_dummy.colore.scommessa = GetChip();
  } else if(id=='n1_18_Scelto'){
    roulette_dummy.n_1_18.n1_18_Scelto = true;
    roulette_dummy.n_1_18.scommessa = GetChip();
  } else {
    roulette_dummy.n_19_36.n_19_36_Scelto = true;
    roulette_dummy.n_19_36.scommessa = GetChip();
  }

  canSpin = true;
}


//WEBSOCKET DATA\\
const ws = new WebSocket("ws://127.0.0.1:7890/Roulette");

ws.addEventListener("open", () =>{  //connection event
  alert("Connected to the AMB Roulette")
});

ws.addEventListener("message", e =>{  //what the srv's sent
  ManageJson(e.data);
  roulette_dummy = BuildJson(); //reset json
  canSpin = false; //reset spin
});

ws.addEventListener("close", () =>{  //closed event
  alert("The server has closed the connection")
});

//send the json to the websocket server
function SendToWebSocketSrv(){
  if(canSpin){
    alert("Good Luck!")
    spin();
    setTimeout(()=> { ws.send(JSON.stringify(roulette_dummy))}, 2000)
  }
  else
    alert("No bets placed")
}

function ManageJson(item){
  const spin = JSON.parse(item);
  alert(`Roulette pulled out: ${parseInt(spin.roll.number)}`)
  if(spin.vincita>0){
    let saldoCorrente = document.getElementById('entrata')
    saldoCorrente.value = parseInt(saldoCorrente.value)+ spin.vincita;

    let Payout = document.getElementById('payout')
    Payout.value = spin.vincita + parseInt(Payout.value)
  }
  else{
    let perse = document.getElementById('fichesPerse')
    perse.value = spin.totalMoneyBetsSpent + parseInt(perse.value)
  }
}


function spin() {
  var roulette = document.getElementById("active");        
  roulette.classList.add("active");
  setTimeout(() => {  roulette.classList.remove("active"); }, 2000);
}