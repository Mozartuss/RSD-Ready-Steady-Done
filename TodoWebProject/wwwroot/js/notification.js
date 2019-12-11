

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();


connection.on("sendToast",
    (userName, taskName) => {

        toastr["success"](userName + " added a new Task: <br/> <b>" + taskName + "</b>");
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
    });


connection.start().catch(err => console.error(err.toString())).then(function () {
    console.log("connected");
});

function sendNotification() {
    const userName = document.getElementById("userName").value;
    const taskName = document.getElementById("taskName").value;
    connection.invoke("PushNotification", userName, taskName).catch(err => console.error(err.toString()));
}
