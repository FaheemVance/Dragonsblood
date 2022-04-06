$(function() {

    /** Setup Chat **/
    var chat = $.connection.chatHub;
    var container = $('#ChatContainer');
    var body = $('#ChatBody');
    var minimiseButton = $('#minimiseChat');

    chat.client.sendMessage = function(message, doping, room) {
        if (doping) {
            ping();
        }
        var roomName = getCleansedIdNames(room);
        var chatArea = $('#Messages-' + roomName);
        var roomTab = document.getElementById('#tab-' + roomName);
        var body = $('#ChatBody');

        chatArea.append(message);

        if ($(roomTab).hasClass('active')) {
            body.scrollTop(body[0].scrollHeight);
        } else {
            $(roomTab).addClass('unread');
        }
    }

    chat.client.updateUsers = function(users) {
        var userArea = $('#userList');

        var con = Array(users);
        var array = con[0];

        var length = array.length;

        userArea.html('');

        for (i = 0; i < length; i++) {
            userArea.append('<li>' + array[i] + '</li>');
        }
    }

    chat.client.sendErrorMessage = function(error) {
        var chatArea = $('#ChatBox');
        var errorHtml = '<strong>' + error + '</strong>';
        chatArea.html(errorHtml);
        var chatMin = $('#ChatBoxMin');
        chatMin.remove();
    }

    chat.client.updateInactive = function() {
        var chatArea = $('#ChatBox');
        var errorHtml = '<strong>Due to your inactivity you have been disconnected from chat, please refresh your browser to reconnect.</strong>';
        chatArea.html(errorHtml);
        var chatMin = $('#ChatBoxMin');
        chatMin.html(errorHtml);
    }

    chat.client.setChatRoomData = function(roomList, roles) {
        var roomDropdown = $('#roomList');
        var con = Array(roomList);
        var array = con[0];
        var length = array.length;

        roomDropdown.html('');

        for (i = 0; i < length; i++) {
            roomDropdown.append('<li>' + array[i] + '</li>');
        }

        $('#roomList > li').on('click', function() {
            selectRoomFromDropdown(this);
        });

        if (roles != null) {

            var roomPermissionList = $('#permissionList');
            var container = Array(roles);
            var roleArray = container[0];
            var roleLength = roleArray.length;

            roomPermissionList.html('');

            for (i = 0; i < roleLength; i++) {
                roomPermissionList.append('<li>' + roleArray[i] + '</li>');
            }

            $('#permissionList > li').on('click', function () {
                selectPermissionFromDropdown(this);
            });
        }
    }

    chat.client.redirect = function(url) {
        var location = 'http://' + window.location.host + url;
        window.location = location;
    }

    chat.client.updateAlerts = function(count) {
        var alertBody = $('#AlertNav');

        var inactiveItem = '<span style="font-size: 18px;" class="glyphicon glyphicon-globe"></span>';
        var activeItem = '<span style="font-size: 18px;" class="glyphicon glyphicon-globe active"></span>';
        var inactiveBadge = '<span class="badge">' + count + '</span>';
        var activeBadge = '<span class="badge active">' + count + '</span>';

        var icon = '';

        if (count > 0) {
            icon = activeItem + activeBadge;
        } else {
            icon = inactiveItem;
        }

        alertBody.html('').html(icon);
        pulse();
    }

    chat.client.reConnect = function() {
        $('#chatList').html('');
    };

    chat.client.setRooms = function(rooms, selectedRoomIndex) {
        var roomTabArea = $('#ActiveRooms');
        var roomTabs = $('#RoomTabs');

        var roomArray = Array(rooms);
        var array = roomArray[0];

        var length = array.length;

        roomTabArea.html('');
        roomTabs.html('');;

        for (var i = 0; i < length; i++) {
            var safeName = getCleansedIdNames(array[i]);
            var displayName = displayRoomNames(array[i]);

            roomTabArea.append('<li role="presentation" id="#tab-' + safeName + '"><a href="#room-' + safeName + '" aria-controls="' + safeName + '" role="tab" data-toggle="tab"><div>' + displayName + '</div></a></li>');
            roomTabs.append('<div role="tabpanel" class="tab-pane" id="room-' + safeName + '"><ul id="Messages-' + safeName + '"></ul></div>');

            /*
            if (i === selectedRoomIndex) {
                
            } else {
                roomTabArea.append('<li  id="#tab-' + safeName + '" role="presentation"><a href="#room-' + safeName + '" aria-controls="' + safeName + '" role="tab" data-toggle="tab"><div>' + displayName + '</div></a></li>');
                roomTabs.append('<div role="tabpanel" class="tab-pane" id="room-' + safeName + '"><ul id="Messages-' + safeName + '"></ul></div>');
            }
            */
        }

        var tabChildren = $('#ActiveRooms').children();
        var roomChildren = $('#RoomTabs').children();
        $(tabChildren[selectedRoomIndex]).addClass('active');
        $(roomChildren[selectedRoomIndex]).addClass('active');

        setLeaveRoomGlyphs();

        $('#ActiveRooms > li')
            .on('click', changeTab);

        chat.server.getUpdatedMessages();
    }

    function changeTab(click) {
        var parent = click.currentTarget;
        var name = getCleansedIdNames(parent.textContent);

        var tab = document.getElementById('#tab-' + name);

        if ($(tab).hasClass('unread')) {
            $(tab).removeClass('unread');
        }

        chat.server.updateCurrentRoom(name);
    }

    function leaveRoom() {
        var parent = this.parentNode.lastChild.lastChild;
        var name = parent.firstChild.data;
        chat.server.leaveRoom(name);
    }

    function setLeaveRoomGlyphs(newRoom) {
        var activerooms = $('#ActiveRooms').children();
        var glyph = '<span class="glyphicon glyphicon-remove-circle"></span>';

        for (var i = 0; i < activerooms.length; i++) {

            if ($(activerooms[i])[0].innerText.trim() === 'All') {
                continue;
            }

            $(activerooms[i]).prepend(glyph);
        }

        $('#ActiveRooms > li > span.glyphicon-remove-circle')
            .on('click', leaveRoom);
    }

    chat.client.updateRoomMessages = function (messages, room) {
        var roomName = getCleansedIdNames(room);
        var roomTab = $('#Messages-' + roomName);
        var body = $('#ChatBody');
        var messageArray = Array(messages);
        var array = messageArray[0];

        var length = array.length;

        roomTab.html('');
        for (i = 0; i < length; i++) {
            roomTab.append(array[i]);
        }

        body.scrollTop(body[0].scrollHeight);
    }

    chat.client.updateRoomUsers = function(users) {
        $('#ChatRoomUsers').html('');
        for (var i = 0; i < users.length; i++) {
            $('#ChatRoomUsers').append('<li>' + users[i] + '</li>');
        }
    }

    chat.client.updateAddToGroup = function (current, all, user) {
        var currentList = $('#currentRolesDisplay');
        var removeList = $('#removeList');
        var userDisplay = $('#userHidden');
        var allList = $('#roleList');
        var modalBody = $('#UserGroupModalBody');

        if (user == null) {
            modalBody.html('No Chat User Exists For This User, user must sign in before permissions can be set');
            return;
        }

        var currentArray = Array(current);
        var carray = currentArray[0];
        var clength = carray.length;

        userDisplay.html('');

        for (i = 0; i < clength; i++) {
            currentList.append('<li>' + carray[i] + '</li>');
            removeList.append('<li>' + carray[i] + '</li>');
        }

        var allArray = Array(all);
        var aarray = allArray[0];
        var alength = aarray.length;

        for (i = 0; i < alength; i++) {
            allList.append('<li>'+ aarray[i] +'<li>');
        }
        userDisplay.html(user);
        $('#roleList > li').on('click', function() {
            $('#selectedRole').html(this.textContent);
        });

        $('#CreateRoleButton').on('click', function() {
            chat.server.addUserGroup($('#selectedRole').html(), $('#userHidden').html());
            $('#UserGroupModal').modal('hide');
        });

        $('#removeList > li').on('click', function () {
            $('#selectedRoleToRemove').html(this.textContent);
        });

        $('#RemoveRoleButton').on('click', function () {
            chat.server.removeUserFromGroup($('#selectedRoleToRemove').html(), $('#userHidden').html());
            $('#UserGroupModal').modal('hide');
        });

    }

    chat.client.updateMotd = function(message) {
        var activeRoom = $('#Motd p');
        if (message === '' || message == null) {
            $('#Motd').addClass('hidden');
        } else {
            $('#Motd').removeClass('hidden');
        }
        $(activeRoom)[0].innerHTML = message;
    }

    function initialise() {
        $('#sendBtn').click(function() {
            var input = $('#textInput');
            var room = $('#ActiveRooms > li.active > a > div').html();
            if (!input.val().trim())
                return false;

            chat.server.send(input.val(), room);
            $('#textInput').val('').focus();
        });
    }

    var tryingToReconnect = false;

    $.connection.hub.connectionSlow(function() {

    });

    $.connection.hub.reconnecting(function() {
        tryingToReconnect = true;
    });

    $.connection.hub.reconnected(function() {
        tryingToReconnect = false;
    });

    $.connection.hub.disconnected(function() {
        setTimeout(function() {
            $.connection.hub.start().done(initialise);
        }, 5000); // Restart connection after 5 seconds.
    });

    $.connection.hub.error(function(error) {
        console.log('SignalR error: ' + error)
    });

    $.connection.hub.start().done(initialise);

    /** Setup Functions **/

    function onEnter(e) {
        if (!e) e = window.event;
        var input = $('#textInput');
        if (!input.val().trim())
            return false;
        var keyCode = e.keyCode || e.which;
        if (keyCode == '13') {
            var room = $('#ActiveRooms > li.active > a > div').html();
            chat.server.send($('#textInput').val(), room);
            $('#textInput').val('').focus();

            return false;
        }
    }

    function ping() {
        document.getElementById('Ping').play();
    }

    function pulse() {
        document.getElementById('Pulse').play();
    }

    function disablePing() {
        document.getElementById('Ping').muted = true;
        chat.server.updatePing(false);
        var elem = $('#PingOff');
        elem.addClass('glyphicon-volume-off');
        elem.removeClass('glyphicon-volume-up');
        $('#PingOff').off('click');
        elem.attr('id', 'PingOn');

        $('#PingOn').on('click', function() {
            enablePing();
        });
    }

    function enablePing() {
        document.getElementById('Ping').muted = false;
        chat.server.updatePing(true);
        var elem = $('#PingOn');
        elem.removeClass('glyphicon-volume-off');
        elem.addClass('glyphicon-volume-up');
        $('#PingOn').off('click');
        elem.attr('id', 'PingOff');

        $('#PingOff').on('click', function() {
            disablePing();
        });

    }

    function disablePulse() {
        document.getElementById('Pulse').muted = true;
        chat.server.updateAlert(true);
    }

    function enablePulse() {
        document.getElementById('Pulse').muted = false;
        chat.server.updateAlert(true);
    }

    function selectRoomFromDropdown(room) {
        $('#selectedRoom').text(room.textContent);
    }

    function selectPermissionFromDropdown(permission) {
        $('#selectedPermission').text(permission.textContent);
    }

    function getCleansedIdNames(item) {
        var displayName = item;
        if (item.indexOf(' ') > -1) {
            displayName = item.replace(' ', '-');
        }
        return displayName.trim();
    }

    function displayRoomNames(item) {
        var display = item.replace('-', ' ');

        return display;
    }

    /** Setup Bindings **/

    $('#textInput').bind('keyup', function(e) { onEnter(e); });
    $('#textInput').focus();

    $('#PingOn').on('click', function() {
        enablePing();
    });
    $('#JoinRoomBtn').on('click', function () {
        chat.server.updateRoomList();
        $('#RoomModal').modal();
    });

    $('div[data-button=AddToGroup]').on('click', function () {
        var button = this.parentNode;
        var user = button.children[0].textContent;

        chat.server.getAddToGroupData(user);
        $('#UserGroupModal').modal();
        $('#UserGroupModal').on('hidden.bs.modal', function() {
            $('#userHidden').html('');
            $('#currentRolesDisplay').html('');
            $('#roleList').html('');
            $('#UserGroupModalBody').html('<ul id="RoleTabs" class="chat-tabs tabs-custom" role="tablist">'+
                    '<li role="presentation" class="active"><a href="#currentRoles" aria-controls="current" role="tab" data-toggle="tab">Current Roles</a></li>'+
                    '<li role="presentation"><a href="#addNewRole" aria-controls="addNew" role="tab" data-toggle="tab">Add New Role</a></li>' +
                    '<li role="presentation"><a href="#removeRole" aria-controls="remove" role="tab" data-toggle="tab">Remove Role</a></li>' +
                '</ul>' +
                '<br style="clear: both;"/>' +
                '<div class="tab-content chat-content"><div role="tabpanel" class="tab-pane active" id="currentRoles"><p>These are the user\'s current roles</p><ul id="currentRolesDisplay"></ul></div>'+
                '<div role="tabpanel" class="tab-pane" id="addNewRole"><p>Choose a role to add to the current user</p><label class="hidden" id="userHidden"></label>'+
                '<div class="dropdown"><div class="dropdown-toggle modal-dropdown" data-toggle="dropdown"><label id="selectedRole">Select a role...</label><span class="caret nest-right nest-dropdown-caret"></span></div><ul class="dropdown-menu" role="menu" id="roleList"></ul></div>'+
                '<div id="CreateRoleButton" class="btn btn-primary">Add Role</div></div>' +
                '<div role="tabpanel" class="tab-pane" id="removeRole">' +
                '<p>Choose a role to add to the current user</p><div class="dropdown"><div class="dropdown-toggle modal-dropdown" data-toggle="dropdown"><label id="selectedRoleToRemove">Select a role...</label><span class="caret nest-right nest-dropdown-caret"></span></div>' +
                '<ul class="dropdown-menu" role="menu" id="removeList"></ul></div><div id="RemoveRoleButton" class="btn btn-primary">Remove Role</div></div></div>');
        });
    });

    $('#CreateRoomButton').on('click', function () {
        var name = $('#CreateRoomName').val();
        var permission = $('#selectedPermission').html();
        chat.server.createRoom(name, permission);
        $('#RoomModal').modal('hide');
    });

    $('#JoinExistingRoom').on('click', function() {
        chat.server.joinRoom($('#selectedRoom').html());
        $('#RoomModal').modal('hide');
    });

    $('#PingOff').on('click', function () {
        disablePing();
    });

    $('#logOffButton').click(function() {
        chat.server.disconnect();
    });

    $('#minimiseChat').on('click', function () {
        if (container.hasClass('collapse')) {
            container.removeClass('collapse');
            minimiseButton.removeClass('glyphicon-chevron-down');
            minimiseButton.addClass('glyphicon-chevron-up');
        } else {
            container.addClass('collapse');
            minimiseButton.removeClass('glyphicon-chevron-up');
            minimiseButton.addClass('glyphicon-chevron-down');
        }
    });
});