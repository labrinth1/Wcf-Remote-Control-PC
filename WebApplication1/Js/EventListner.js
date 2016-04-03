
function getMousePos(canvas, evt) {
    var rect = canvas.getBoundingClientRect();
    return {
        x: Math.round(evt.clientX - rect.left),
        y: Math.round(evt.clientY - rect.top)
    };
}
function keyupdown(event)
{
    /*
    The client needs to know if the keyboard event is keyup or keydown.
    Therefore im just adding a 1 or 0 to the end. This will be handled in the client application.
    */
    var keydown = "0"; 
    if (event.type == 'keydown') {
        keydown = "1"; 
    }
    postKeyBoardkey(event.keyCode + keydown);
}

 function MouseUpDownEvents(event) {
    var windowsKeyCode;
    switch (event.which) {
        // Mouseevents in web is not the same as in windows so convert it here.
        case 1://Left button
            windowsKeyCode = 2;
            break;
        case 2://Middle button
            windowsKeyCode = 32;
            break;
        case 3://Right button
            windowsKeyCode = 8;
            break;
        default:
            alert('Unknown mouse event!');
    }
    if (event.type == "mouseup") {
        //For Keyup event just multiply the value by 2
        windowsKeyCode *= 2;
    }
    postMouseKey(windowsKeyCode);
 }
 function mousemoveevt(evt)
 {
     var mousePos = getMousePos(canvas, evt);
     $('#spos').text('X:' + mousePos.x + 'Y:' + mousePos.y);
     postMouseMove(mousePos.x, mousePos.y);
 }

function postMouseMove(x, y) {
    $.ajax({
        type: "POST", 
        url: "/Service.svc/MouseCoords", 
        data: JSON.stringify({ rawPackets: [x, y] }), 
        contentType: "application/json", 
        processdata: false, 
        cache: false,
        error: ServiceFailed
    });
}


function postKeyBoardkey(keycode) {
    $.ajax({
        type: "POST", 
        url: "/Service.svc/KeyBoardHandler", 
        data: JSON.stringify({ rawPackets: keycode }), 
        contentType: "application/json",
                processdata: false, 
                cache: false,
                error: ServiceFailed
            });
    
}
function postMouseKey(keycode) {
    $.ajax({
        type: "POST", 
        url: "/Service.svc/MouseEvents", 
        data: JSON.stringify({ rawPackets: keycode }), 
        contentType: "application/json", 
        processdata: false, 
        cache: false,
        error: ServiceFailed
    });
}

function ServiceFailed(result) {
    alert('Service call failed: ' + result.status + '' + result.statusText);

}

