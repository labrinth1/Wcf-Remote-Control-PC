var keepGoing = true;

var imgScreen = document.getElementById("imgScreenShot");
var canvas = document.getElementById('myCanvas');
var context = canvas.getContext('2d');


function StopClient() {
    StartStopClient(0, false);
}
function StartClient() {
    StartStopClient(1, true);
}
function Bindevents() {
    $("#canvasWrapper").on("keyup keydown", keyupdown);
    $("#canvasWrapper").on("mousedown mouseup", MouseUpDownEvents);
    $(canvas).on('mousemove', mousemoveevt);
}
function UnBindevents() {
    $("#canvasWrapper").off("keyup keydown", keyupdown);
    $("#canvasWrapper").off("mousedown mouseup", MouseUpDownEvents);
    $(canvas).off('mousemove', mousemoveevt);
}

function StartStopClient(start, process) {
    $.ajax({
        type: "POST",
        url: "/Service.svc/StartClient",
        data: JSON.stringify({ rawPackets: start }),
        contentType: "application/json",
        cache: false,
        success: function () {
            if (process) {
                keepGoing = true;
                Bindevents();
                GetByte();
            }
            else {
                keepGoing = false;
                UnBindevents();
            }
        },
        error: ServiceFailed
    });
}

function GetByte() {
    if (keepGoing) {
        $.ajax({
            type: "GET",
            url: "/Service.svc/GetBase64Img",
            dataType: "text",
            cache: false,
            success: function (msg) {
                ServiceSucceeded(msg);
            },
            error: ServiceFailed
        });
        setTimeout('GetByte()', 500);
    }
}

function ServiceFailed(result) {
    UnBindevents();
    alert('Service call failed: ' + result.status + '' + result.statusText);

}

function ServiceSucceeded(result) {

    var plainBase64 = result.replace(/(<([^>]+)>)/ig, "");
    if (!plainBase64) {
        return;
    }
    /*
    I'm getting the image in base64 encoded format, just for simplicity.
    It would be a lot better performance to use a blob instead, but for this little hobby project we will do just fine :) 
    */
    imgScreen.src = "data:image/png;base64," + plainBase64;
    context.drawImage(imgScreen, 0, 0);

}




