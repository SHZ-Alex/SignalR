let cloakSpan = document.getElementById("cloakCounter");
let stoneSpan = document.getElementById("stoneCounter");
let wandSpan = document.getElementById("wandCounter");

const connectionDeathlyHallows = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/deathyHallows").build();

connectionDeathlyHallows.on("UpdateDealthyHallowCount", (cloak, stone, wand) => {
    cloakSpan.innerText = cloak.toString();
    stoneSpan.innerText = stone.toString();
    wandSpan.innerText = wand.toString();
});


function fulfilled() {
    connectionDeathlyHallows.invoke("GetRaceStatus").then((raceCounter) => {
        cloakSpan.innerText = raceCounter.cloak.toString();
        stoneSpan.innerText = raceCounter.stone.toString();
        wandSpan.innerText = raceCounter.wand.toString();
    });
}

function rejected() {
}


connectionDeathlyHallows.start().then(fulfilled, rejected);