var connection = new signalR.HubConnectionBuilder()
    .withUrl('/chatHub')
    .build();

connection.on('receiveMessage',addMessageToChat);

connection.start()
    .catch(error => {
        console.error(error.message);
    });

function sendMessageToHub(message, user) {
    connection.invoke('sendMessage', message, user);
}
