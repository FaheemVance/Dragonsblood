$(function () {
    var manager = $.connection.userHub;

    manager.client.logOff = function () {
        document.getElementById('logoutForm').submit();
    }
});