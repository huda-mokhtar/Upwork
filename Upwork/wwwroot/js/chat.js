class Message {
    constructor(username, text, when) {
        this.userName = username;
        this.text = text;
        this.when = when;
    }
}

// userName is declared in razor page.
const username = userName;
const textInput = document.getElementById('messageText');
/*const when = document.getElementById('when');*/
const RecevierId = document.getElementById('ReceiverId').value;
const chat = document.getElementById('chat');
const messagesQueue = [];

document.getElementById('submitButton').addEventListener('click', () => {
    var currentdate = new Date();
    when.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })

});

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;
    
    let when = new Date();
    let message = new Message(username, text, when);
    sendMessageToHub(message, RecevierId);
    addMessageToChat(message);
}

function addMessageToChat(message) {
    let isCurrentUserMessage = message.userName === username;
    let containerParent = document.createElement('div');
    containerParent.className = isCurrentUserMessage ? "col-md-5  offset-md-7" : "col-md-5";
    let container = document.createElement('div');
   
    container.className = isCurrentUserMessage ? "container darker" : "container";
   
    let text = document.createElement('p');
    text.innerHTML = message.text;
    text.className = isCurrentUserMessage ? "text-left pt-2  text-white" : "text-left pt-2"
    let when = document.createElement('span');
    when.className = isCurrentUserMessage ? "time-right" : "time-Reciver";
    var currentdate = new Date();
    when.innerHTML = 
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })

    container.appendChild(text);
    container.appendChild(when);
    containerParent.appendChild(container)
    chat.appendChild(containerParent);
}
