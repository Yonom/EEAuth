<div style="text-align: center">
    <row class="row">
        <h1 class="cover-heading"><b><?= $data['name'] ?></b><br/>
        <small> would like to verify your identity.</small></h1>

        <div class="row col-xs-offset-3 col-xs-6">
          <div id="accountselection">
            <p>Please select your account type:</p>
            <div class="links">
                <a target="_blank" class="btn btn-success" id="eelogin" onclick="continueClicked();">
                    EE.COM
                </a>
                <a target="_blank" class="btn btn-primary" id="fblogin" onclick="continueClicked();">
                    Facebook
                </a>
            </div>
            <h1 class="cover-heading"><small>OR</small></h1>
            <p>Join this Room Id: <p id="roomid"></p></p>
            <br>
            <button type="button" class="btn btn-success" onclick="continueClicked();">Continue »</button>
          </div>
        </div>

        <div class="row col-xs-offset-3 col-xs-6">
          <div id="codecontainer" class="code-container" style="display:none;">
              <br>
              <p>Enter the verification code here:</p>
              <form onsubmit="verify(); return false;">
                <input class="form-control" type="text" id="codeinput" value="" placeholder="########"><br>
                <button type="submit" class="btn btn-success">Verify Account »</button>
              </form>
          </div>
        </div>

        <div class="row col-xs-offset-3 col-xs-6">
          <div id="errorcontainer" style="display:none;">
            <p id="errortext">
          </div>
        </div>
    </row>
</div>

<script type="text/javascript">
    var roomid = document.getElementById('roomid');
    var eelogin = document.getElementById('eelogin');
    var fblogin = document.getElementById('fblogin');
    var codecontainer = document.getElementById('codecontainer');
    var accountselection = document.getElementById('accountselection');
    var errorcontainer = document.getElementById('errorcontainer');
    var errortext = document.getElementById('errortext');
    var codeinput = document.getElementById('codeinput');

    var ws = new WebSocket("wss://spambler.com:5010/Auth" + window.location.search);

    window.onerror = function(msg, url, linenumber) {
        alert('Error message: '+msg+'\nURL: '+url+'\nLine Number: '+linenumber);
        return true;
    }

    ws.onmessage = function (event) {
        var args = event.data.split(" ");
        console.log(args);
        if (args[0] == "room"){
            room(args[1]);
        } else if (args[0] == "redirect") {
            success();
            window.location = args[1];
        } else if (args[0] == "error") {
            args.shift();
            error(args.join(" "));
        }
    };

    ws.onclose = function (event) {
        if (!event.wasClean)
            error("Disconnected. Please try again later.");
    }

    function room(roomId) {
        roomid.innerHTML = "<b>" + roomId + "</b>";
        showLinks(roomId);
    }

    function success() {
        roomid.innerHTML = "Success!";
        roomid.color = "green";
    }

    function error(message) {
        errortext.innerHTML = "<b>" + message + "</b>";
        errortext.style.color = "red";

        accountselection.style.display = "none";
        codecontainer.style.display = "none";
        errorcontainer.style.display = "inline";
    }

    function showLinks(roomId) {
        eelogin.href = "http://everybodyedits.com/games/" + roomId;
        fblogin.href = "https://apps.facebook.com/everedits/games/" + roomId;

        eelogin.style.display = "inline";
        fblogin.style.display = "inline";
    }

    function hideLinks() {
        eelogin.style.display = "none";
        fblogin.style.display = "none";
    }

    function continueClicked() {
        accountselection.style.display = "none";
        codecontainer.style.display = "inline";
    }

    function verify() {
        ws.send("verifyCode" + codeinput.value.toString());
    }
</script>
