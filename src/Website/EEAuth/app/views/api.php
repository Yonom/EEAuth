<div style="text-align: left;">
    <h1>API Reference</h1>
    <article>
        <h2>Getting Started</h2>
        <p>
            This tutorial guides you through setting up EEAuth login on your server. Make sure to follow these instructions closely.
        </p>
    </article>
    <article>
        <h2>Step 1: Generating your API key</h2>
        <p>
            EEAuth's api is based on a private/public keypair. The public key can be shared with anyone, but the private key <b>MUST be kept secret</b>.
            Because only you and the EEAuth server know the private key, it can be used to verify that the requests actually come from the other party.
        </p>
        <p>
            <a href="/getkey">Click here to get a random API key</a>
        </p>
    </article>
    <article>
        <h2>Step 2: API Implementations</h2>
        <p>To communicate with EEAuth from your server, you need an implementation of EEAuth's <a href="https://github.com/Spambler/EEAuth/blob/master/HttpSpec.md">HTTP Spec</a>.</p>
        <p>We provide an official library for PHP, which you can download here:<br><a href="https://raw.githubusercontent.com/Spambler/EEAuth/master/PHP/EEAuth.php">https://raw.githubusercontent.com/Spambler/EEAuth/master/PHP/EEAuth.php</a></p>
    </article>
    <article>
        <h2>Step 3: Using the PHP library</h2>
        Please take a look at <a href="https://github.com/Spambler/EEAuth/blob/master/PHP/example.php">this example</a> to learn how to use the PHP library.
    </article>
</div>
