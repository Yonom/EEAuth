<div style="text-align: center">
    <h1></h1>
    <row class="row">
        <h1 class="cover-heading"><b><?= $data['name'] ?></b><br/>
        <small> would like to verify your identity.</small></h1>
        <p>Please log into EE and join the following room:</p>
        <div class="row">
            <div class="card col-xs-offset-3 col-xs-6" id="roomid">
                Generating room id, please wait...
            </div>
        </div>
        <p><i>(keep this tab open in the background)</i></p>
        <div class="links">
            <a target="_blank" id="eelogin">
                <img src="/images/ee_login.png" alt="Direct link for everybodyedits.com users.">
            </a>
            <a target="_blank" id="fblogin">
                <img src="/images/facebook_login.png" alt="Direct link for facebook.com users.">
            </a>
        </div>
        <div id="codecontainer" class="code-container">
            <p>and chat the following code in the room</p>
            <h2 id="code"></h2>
        </div>
    </row>
</div>

<script type="text/javascript">
    var roomid = document.getElementById('roomid');
    var eelogin = document.getElementById('eelogin');
    var fblogin = document.getElementById('fblogin');
    var code = document.getElementById('code');
    var codecontainer = document.getElementById('codecontainer');
    var ws = new WebSocket("ws://spambler.com:5010/Auth" + window.location.search);

    ws.onmessage = function (event) {
        var args = event.data.split(" ");
        console.log(args);
        if (args[0] == "room"){
            room(args[1], args[2]);
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

    function room(roomId, token) {
        roomid.innerHTML = roomId;
        code.innerHTML = token;
        showLinks(roomId);
    }

    function success() {
        roomid.innerHTML = "Success!";
        roomid.color = "green";
        hideLinks();
    }

    function error(message) {
        roomid.innerHTML = message;
        roomid.style.color = "red";
        hideLinks();
    }

    function showLinks(roomId) {
        eelogin.href = "http://everybodyedits.com/games/" + roomId;
        fblogin.href = "https://apps.facebook.com/everedits/games/" + roomId;

        eelogin.style.display = "inline";
        fblogin.style.display = "inline";
        codecontainer.style.display = "inline";
    }

    function hideLinks() {
        eelogin.style.display = "none";
        fblogin.style.display = "none";
        codecontainer.style.display = "none";
    }
</script>
