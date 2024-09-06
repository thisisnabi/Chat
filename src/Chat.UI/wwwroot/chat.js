 
var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7199/chats")
    .build();

connection.start().then(function () {
    console.log("Connected to the chat server!");
}).catch(function (err) {
    return console.error(err.toString());
});



document.getElementById('sendMessage').addEventListener('click', function () {
    const input = document.getElementById('inputMessage');
    const message = input.value.trim();

    var conversationid = localStorage.getItem("current_conversation"); 
    if (message && conversationid)
    {
        connection.invoke("SendMessage", conversationid, message)
            .catch(function (err) {
                return console.error(err.toString());
            });
    }
});

connection.on("NewMessage", (msg) => {
    debugger;

    var conversationid = localStorage.getItem("current_conversation");
    if (conversationid === msg.conversationId) {
        const messageHTML = `<div class="message">
                                 <p><strong>${msg.senderName}:</strong> ${msg.content}</p>
                                 <p class="time">${new Date(msg.sentAt).toLocaleString()}</p>
                             </div>
                           `;

        $('.messages').append(messageHTML);
    }
    else {
        var oldText = $(".conversation[data-id='" + msg.conversationId + "'] h4").text();
        $(".conversation[data-id='" + msg.conversationId + "'] h4").text(oldText + "  ***");
    }
    
});