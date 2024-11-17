const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .withAutomaticReconnect([0, 1000, 5000, 20000, null])
    .build();


connection.on("ReceiveUserConnected", function (userId, userName) {
    addMessage(`${userName} присоединился`);
});

connection.on("ReceiveAddRoomMessage", function (maxRoom, roomId, roomName, userId, userName) {
    addMessage(`${userName} создал комнату  ${roomName}`);
    fillRoomDropDown();
});

connection.on("ReceivePrivateMessage", function (senderId, senderName, receiverId, message, chatId, receiverName) {
    addMessage(`[Приватное сообщение ${receiverName} ] ${senderName} написал ${message}`);
})

connection.on("ReceiveUserDisconnected", function (userId, userName) {
    addMessage(`${userName} вышел`);
});

connection.on("ReceiveDeleteRoomMessage", function (deleted, selected, roomName, userName) {
    addMessage(`${userName} удалил комнату ${roomName}`);
});

connection.on("ReceivePublicMessage", function (roomId, UserId, userName, message, roomName) {
    addMessage(`[Публичное сообщение - ${roomName}] ${userName} написал ${message}`);
})

function addnewRoom(maxRoom) {
    let createRoomName = document.getElementById('createRoomName');
    var roomName = createRoomName.value;
    if (roomName == null && roomName == '') {
        return;
    }
    $.ajax({
        url: '/ChatRooms',
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({id: 0, name: roomName}),
        async: true,
        processData: false,
        cache: false,
        success: function (json) {
            connection.send("SendAddRoomMessage", maxRoom, json.id, json.name);
            createRoomName.value = '';
        },
        error: function (xhr) {
            alert('error');
        }
    })
}

function sendPrivateMessage() {
    let inputMsg = document.getElementById('txtPrivateMessage');
    let ddlSelUser = document.getElementById('ddlSelUser');
    let receiverId = ddlSelUser.value;
    let receiverName = ddlSelUser.options[ddlSelUser.selectedIndex].text;
    var message = inputMsg.value;
    connection.send("SendPrivateMessage", receiverId, message, receiverName);
    inputMsg.value = '';
}

document.addEventListener('DOMContentLoaded', (event) => {
    fillRoomDropDown();
    fillUserDropDown();
})

function fillUserDropDown() {

    $.getJSON('/ChatRooms/CurrentUser')
        .done(function (json) {
            var ddlSelUser = document.getElementById("ddlSelUser");
            ddlSelUser.innerText = null;
            json.forEach(function (item) {
                var newOption = document.createElement("option");
                newOption.text = item.userName;
                newOption.value = item.id;
                ddlSelUser.add(newOption);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            console.log("Request Failed: " + jqxhr.detail);
        });


}

function fillRoomDropDown() {
    $.getJSON('/ChatRooms')
        .done(function (json) {
            var ddlDelRoom = document.getElementById("ddlDelRoom");
            var ddlSelRoom = document.getElementById("ddlSelRoom");
            ddlDelRoom.innerText = null;
            ddlSelRoom.innerText = null;
            json.forEach(function (item) {
                var newOption = document.createElement("option");
                newOption.text = item.name;
                newOption.value = item.id;
                ddlDelRoom.add(newOption);
                var newOption1 = document.createElement("option");
                newOption1.text = item.name;
                newOption1.value = item.id;
                ddlSelRoom.add(newOption1);
            });
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            console.log("Request Failed: " + jqxhr.detail);
        });
}

function sendPublicMessage() {
    let inputMsg = document.getElementById('txtPublicMessage');
    let ddlSelRoom = document.getElementById('ddlSelRoom');
    let roomId = ddlSelRoom.value;
    let roomName = ddlSelRoom.options[ddlSelRoom.selectedIndex].text;
    var message = inputMsg.value;
    connection.send("SendPublicMessage", Number(roomId), message, roomName);
    inputMsg.value = '';
}



function deleteRoom() {
    let ddlDelRoom = document.getElementById('ddlDelRoom');
    var roomName = ddlDelRoom.options[ddlDelRoom.selectedIndex].text;
    let text = `Вы действительно хотите удалить комнату ${roomName}?`;
    if (confirm(text) == false) {
        return;
    }
    if (roomName == null && roomName == '') {
        return;
    }
    let roomId = ddlDelRoom.value;
    $.ajax({
        url: `/ChatRooms/${roomId}`,
        dataType: "json",
        type: "DELETE",
        contentType: 'application/json;',
        async: true,
        processData: false,
        cache: false,
        success: function (json) {
            /*ADD ROOM COMPLETED SUCCESSFULLY*/
            connection.send("SendDeleteRoomMessage", json.deleted, json.selected, roomName);
            fillRoomDropDown();
        },
        error: function (xhr) {
            alert('error');
        }
    })
}


function addMessage(msg) {

    if (msg == null && msg == '') {
        return;
    }

    let ui = document.getElementById('messagesList');
    let li = document.createElement("li");
    li.innerHTML = msg;
    ui.appendChild(li);
}

connection.start();