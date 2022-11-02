"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:6001/chatHub", {
        accessTokenFactory: async () => {
            return new Promise((resolve) => {
                fetch('/ChatRoom?handler=Token').then(async (response) => {
                    let token = await response.json();
                    resolve(token.accessToken);
                });
            });
        }
    })
    .withAutomaticReconnect()
    .build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    var messageList = document.getElementById("messagesList");
    if (messageList.childNodes.length > 50) {
        messageList.removeChild(messageList.childNodes[0]);
    }

    messageList.appendChild(li);
    li.textContent = `${message}`;
    messageList.parentNode.scrollTop = messageList.parentNode.scrollHeight;
});

connection.start().then(function () {
    connection.invoke("AddToGroup", chatGroupId)
        .then(function () {
            document.getElementById("sendButton").disabled = false;
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    if (!message) {
        event.preventDefault()
        return;
    }

    $.ajax('/ChatRoom?handler=AddMessage', {
        data: JSON.stringify({ Message: message, ChatGroupId: chatGroupId }),
        contentType: 'application/json',
        type: 'POST',
        dataType: "json",
        headers:
        {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
    event.preventDefault();
});