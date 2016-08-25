<div style="text-align: center">
    <row class="row">
        <h1 class="cover-heading"><b><?= $data['name'] ?></b><br/>
        <small> would like to verify your identity.</small></h1>

        <br>

        <div class="row col-xs-offset-2 col-xs-8">
          <div id="accountselection">
            <p>Click to open new page:</p>
            <div class="links">
                <a target="_blank" id="eelogin" onclick="continueClicked();" data-toggle="tooltip" title="Login with everybodyedits.com">
                    <img src="/images/ee_login.png">
                </a>
                <a target="_blank" id="fblogin" onclick="continueClicked();" data-toggle="tooltip" title="Login with facebook.com">
                    <img src="/images/facebook_login.png">
                </a>
            </div>
            <br><br>
            <p>You can also join this world by clicking the "ID" button in the lobby and by entering this world id: <p id="roomid"></p></p>
            <br>
            <button type="button" class="btn btn-success" onclick="continueClicked();">Enter Authentication Code »</button>
          </div>
        </div>

        <div class="row col-xs-offset-3 col-xs-6">
          <div id="codecontainer" class="code-container" style="display:none;">

              <p>Enter your authentication code here:</p>
              <form onsubmit="verify(); return false;">
                <input class="form-control" type="text" id="codeinput" maxlength="16" value=""><br>
                <button type="submit" class="btn btn-success">Submit Authentication Code »</button>
              </form>
          </div>
        </div>

        <div class="row col-xs-offset-3 col-xs-6">
          <div id="errorcontainer" style="display:none;">
            <p id="errortext">
          </div>
        </div>

        <div class="tooltip top" role="tooltip">
          <div class="tooltip-arrow"></div>
          <div class="tooltip-inner">
            Some tooltip text!
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
        roomid.innerHTML = "<h4 class=\"cover-heading\"><i>" + roomId + "</i></h4>";
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
