
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chathub")
        .withAutomaticReconnect([0, 1000, 5000, null])
        .build();


connection.start().then(function () {
    console.log("Connected");
    window.signalRConnection = connection;
}).catch(function (err) {
    return console.error(err.toString());
});