//create connection

const connectionUserCount = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("/hubs/userCount", signalR.HttpTransportType.WebSockets)
    .build();


//connect to methods that hub invokes aka receive notfications from hub
connectionUserCount.on("UpdateViews", (value) => {
    let newCountSpan = document.getElementById("CountViews");
    newCountSpan.innerText = value.toString();
});

connectionUserCount.on("UpdateTotalUsers", (value) => {
    let newCountSpan = document.getElementById("TotalUsers");
    newCountSpan.innerText = value.toString();
});

connectionUserCount.onclose((error) => {
    document.body.style.background = "red";
});
connectionUserCount.onreconnected((connectionId) => {
    document.body.style.background = "white";
});
connectionUserCount.onreconnecting((error) => {
    document.body.style.background = "orange";
});


function newWindowLoadedOnClient() {
    connectionUserCount.invoke("NewWindowLoaded", "SHZ")
        .then((value) => console.log(value));
}

function fulfilled() {
    console.log("Connection to User Hub Successful");
    newWindowLoadedOnClient();
}



function rejected() {}
connectionUserCount.start().then(fulfilled, rejected);