//create connection

const connectionUserCount = new signalR.HubConnectionBuilder()
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



//invoke hub methods aka send notification to hub
function newWindowLoadedOnClient() {
    connectionUserCount.invoke("NewWindowLoaded", "SHZ")
        .then((value) => console.log(value));
}


//start connection
function fulfilled() {
    console.log("Connection to User Hub Successful");
    newWindowLoadedOnClient();
}



function rejected() {
    //rejected logs
}
connectionUserCount.start().then(fulfilled, rejected);